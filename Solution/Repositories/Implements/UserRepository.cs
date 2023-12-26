using Commons.Loggers;
using Commons.Utilities;
using Discord.Net;
using Infrastructures;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using Models.Requests;
using Models.Responses;
using Repositories.Interfaces;
using System;
using System.Net;

namespace Repositories.Implements
{
    public class UserRepository(ConnectionContext context, IMailRepository mailRepository) : IUserRepository
    {
        private readonly string repositoryName = "UserRepository";
        private readonly ConnectionContext _context = context;
        private readonly IMailRepository _mailRepository = mailRepository;

        //private async Task CleanUserAsync(string idNumber)
        //{
        //    var users = await _context.MasterUsers.Where(i => i.IdNumber == idNumber && !i.IsActive).ToListAsync();

        //    if (users.Any())
        //    {
        //        _context.RemoveRange(users);
        //        await _context.SaveChangesAsync();
        //    }
        //}

        private async Task SendCodeRegister(string idNumber, string requester)
        {
            var staging = new StagingVerify();
            try
            {
                Guid id = Guid.NewGuid();
                string tokenSecure = id.ToString().Replace("-", "")[..4].ToUpper();
                int expired = 7;

                string message = $"JANGAN BERIKAN KODE OTP ke siapapun, Kode OTP anda {tokenSecure}, berlaku {expired} menit";

                staging = new StagingVerify()
                {
                    Remarks = message,
                    CreateDate = DateTime.Now,
                    ExpiredToken = DateTime.Now.AddMinutes(expired),
                    Id = id,
                    IdNumber = idNumber,
                    IsUsed = false,
                    Requester = requester,
                    TokenSecure = tokenSecure,
                };

                _context.Add(staging);
                await _context.SaveChangesAsync();

                await WhatsAppUtility.SendAsync(requester, message);
            }
            catch (Exception ex)
            {
                await DiscordLogger.SendAsync(repositoryName, ex, null, staging);
                throw new NullReferenceException(ex.Message, ex.InnerException);
            }
        }

        public async Task<DefaultResponse> Register(RegisterRequest request)
        {
            DefaultResponse response = new();

            try
            {
                if (request == null)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = "Parameter is Requaired";
                    return response;
                }

                //await CleanUserAsync(request.IdNumber);
                string passwordHash = await SecureUtility.AesEncryptAsync(value: request.Password ?? string.Empty);

                var user = await _context.MasterUsers
                    .Where(i => i.IdNumber == request.IdNumber && i.PasswordHash == passwordHash)
                    .FirstOrDefaultAsync();

                var employee = await _context.MasterEmployees
                    .Where(i => i.IdNumber == request.IdNumber && i.IsActive)
                    .FirstOrDefaultAsync();

                if (employee == null)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = "Id Number not register";
                    return response;
                }

                string requester = request.UserInput ?? string.Empty;

                switch (request.RegisterVerify)
                {
                    case RegisterVerify.Email:
                        if (user == null)
                        {
                            user = new MasterUser()
                            {
                                Id = Guid.NewGuid(),
                                FullName = request.FullName,
                                Email = requester,
                                IdNumber = request.IdNumber,
                                PasswordHash = passwordHash,
                                IsActive = false,
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now
                            };
                            _context.Add(user);
                        }
                        else
                        {
                            user.Email = requester;
                            _context.Update(user);
                        }

                        await _context.SaveChangesAsync();

                        await _mailRepository.SendEmailRegister(idNumber: request.IdNumber, requester: requester);
                        break;
                    case RegisterVerify.PhoneNumber:
                        if (user == null)
                        {
                            user = new MasterUser()
                            {
                                Id = Guid.NewGuid(),
                                FullName = request.FullName,
                                PhoneNumber = requester,
                                IdNumber = request.IdNumber,
                                PasswordHash = passwordHash,
                                IsActive = false,
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now
                            };
                            _context.Add(user);
                        }
                        else
                        {
                            user.PhoneNumber = requester;
                            _context.Update(user);
                        }

                        await _context.SaveChangesAsync();

                        await SendCodeRegister(idNumber: request.IdNumber, requester: requester);
                        break;
                }

                response.StatusCode = HttpStatusCode.Created;
                response.Message = "Register successfuly, please check your email or whatsapp";
                //user.PasswordHash = string.Empty;
                //response.Data = user;
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
                await DiscordLogger.SendAsync(repositoryName, ex, null, request);
                throw new NullReferenceException(ex.Message, ex.InnerException);

            }
            return response;
        }

        public async Task<DefaultResponse> AllowLogin(Guid userId, string IdNumber, bool isActive)
        {
            DefaultResponse response = new();

            try
            {
                var user = await _context.MasterUsers
                    .Where(i => i.Id == userId && i.IdNumber == IdNumber)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = "User not found";
                    return response;
                }

                user.IsActive = isActive;

                _context.Update(user);
                await _context.SaveChangesAsync();
                //response.Data = user;
                response.Message = "Status updated";
                response.StatusCode = HttpStatusCode.OK;

            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
                await DiscordLogger.SendAsync(repositoryName, ex);
                throw new NullReferenceException(ex.Message, ex.InnerException);
            }
            return response;
        }

        public async Task<LoginResponse> Login(LoginRequest request)
        {
            LoginResponse response = new();

            try
            {
                string passwordHash = await SecureUtility.AesEncryptAsync(request.Password ?? string.Empty);

                var masterUser = await _context.MasterUsers
                            .Where(i =>
                            (i.Email == request.UserInput || i.PhoneNumber == request.UserInput || i.IdNumber == request.UserInput)
                            && i.PasswordHash == passwordHash)
                            .OrderByDescending(i => i.UpdatedAt)
                            .Include(i => i.Role)
                            .FirstOrDefaultAsync();

                if (masterUser == null)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = "User or Password invalid";
                    return response;
                }

                if (!masterUser.IsActive)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = "Account user not active, call administrator";
                    return response;
                }

                var masterEmployee = await _context.MasterEmployees
                    .Where(i => i.IdNumber == masterUser.IdNumber && i.IsActive)
                    .FirstOrDefaultAsync();

                if (masterEmployee == null)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = "Employee not active, call administrator";
                }

                var grants = await _context.GrantAccesses.Where(i => i.Role == masterUser.Role).ToListAsync();
                if (grants == null)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = "Grant access user not found, call administrator";
                }

                response.StatusCode = HttpStatusCode.OK;
                masterUser.PasswordHash = string.Empty;
                response.MasterUser = masterUser;
                response.MasterEmployee = masterEmployee;
                response.Grants = grants;

            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
                await DiscordLogger.SendAsync(repositoryName, ex, null, request);
                throw new NullReferenceException(ex.Message, ex.InnerException);
            }
            return response;
        }

        public async Task<DefaultResponse> EmailVerify(string tokenSecure, string requester)
        {
            DefaultResponse response = new();

            try
            {
                var staging = await _context.StagingVerifies
                    .Where(i =>
                    i.TokenSecure == tokenSecure
                    && i.Requester == requester
                    && !i.IsUsed)
                    .FirstOrDefaultAsync();

                if (staging == null)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = "Token not found";
                    return response;
                }

                if (staging.ExpiredToken <= DateTime.Now)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = "Token Email is expired";
                    return response;
                }

                staging.IsUsed = true;
                _context.Update(staging);
                await _context.SaveChangesAsync();

                var user = await _context.MasterUsers
                    .Where(i => i.IdNumber == staging.IdNumber)
                    .FirstOrDefaultAsync();

                if (user != null)
                {
                    user.IsActive = true;
                    user.Email = requester;
                    user.EmailConfirmed = true;
                    _context.Update(user);
                    await _context.SaveChangesAsync();

                    response.StatusCode = HttpStatusCode.OK;
                    response.Message = "Email Success Confirmed";
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
                await DiscordLogger.SendAsync(repositoryName, ex);
                throw new NullReferenceException(ex.Message, ex.InnerException);
            }

            return response;
        }

        public async Task<DefaultResponse> PhoneNumberVerify(string tokenSecure, string requester, string idNumber)
        {
            DefaultResponse response = new();

            try
            {
                var staging = await _context.StagingVerifies
                    .Where(i => i.Requester == requester && i.IdNumber == idNumber)
                    .OrderByDescending(i => i.CreateDate)
                    .FirstOrDefaultAsync();

                if (staging == null)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = "Token not found";
                    return response;
                }

                if (staging.TokenSecure != tokenSecure)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = "Token incorrect";
                    return response;
                }

                if (staging.ExpiredToken <= DateTime.Now)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = "Token is expired";
                    return response;
                }

                staging.IsUsed = true;
                _context.Update(staging);
                await _context.SaveChangesAsync();

                var user = await _context.MasterUsers
                    .Where(i => i.IdNumber == staging.IdNumber && i.PhoneNumber == requester)
                    .FirstOrDefaultAsync();

                if (user != null)
                {
                    user.IsActive = true;
                    user.PhoneNumber = requester;
                    user.PhoneNumberConfirmed = true;
                    _context.Update(user);
                    await _context.SaveChangesAsync();

                    user.PasswordHash = string.Empty;
                    response.StatusCode = HttpStatusCode.OK;
                    //response.Data = user;
                    response.Message = "Phone Number Success Confirmed";
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
                await DiscordLogger.SendAsync(repositoryName, ex);
                throw new NullReferenceException(ex.Message, ex.InnerException);
            }

            return response;
        }

        public async Task<CheckExpiredTokenResponse> CheckExpiredToken(string requester, string idNumber)
        {
            CheckExpiredTokenResponse response = new();

            try
            {
                var user = await _context.MasterEmployees
                    .Where(i => i.IdNumber == idNumber)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = "Id Number not found";
                    return response;
                }

                var staging = await _context.StagingVerifies
                    .Where(i => i.IdNumber == idNumber
                    && i.Requester == requester
                    && i.ExpiredToken >= DateTime.Now
                    && !i.IsUsed)
                    .FirstOrDefaultAsync();

                if (staging == null)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = "Data not found";
                    return response;
                }

                TimeSpan duration = new(staging.ExpiredToken.Ticks - DateTime.Now.Ticks);
                string minutes = string.Format("{0}:{1:00}", (int)duration.TotalMinutes, duration.Seconds);

                response.Minutes = minutes;
                response.Ticks = duration;
                response.FullName = user.FullName;
                response.IdNumber = user.IdNumber;
                response.PhoneNumber = staging.Requester;

            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = ex.Message;
                await DiscordLogger.SendAsync(repositoryName, ex);
                throw new NullReferenceException(ex.Message, ex.InnerException);
            }

            return response;
        }
    }
}
