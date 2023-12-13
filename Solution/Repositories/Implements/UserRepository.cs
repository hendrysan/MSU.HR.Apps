using Commons.Utilities;
using Infrastructures;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using Models.Request;
using Repositories.Interfaces;
using System.Net;
using System.Web;

namespace Repositories.Implements
{
    public class UserRepository(ConnectionContext context, IMailRepository mailRepository) : IUserRepository
    {
        private readonly ConnectionContext _context = context;
        private readonly IMailRepository _mailRepository = mailRepository;

        private async Task CleanUserAsync(string idNumber)
        {
            var users = await _context.MasterUsers.Where(i => i.IdNumber == idNumber).ToListAsync();

            if (users.Any())
            {
                _context.RemoveRange(users);
                await _context.SaveChangesAsync();
            }
        }

        private async Task SendCodeRegister(string idNumber, string requester)
        {
            Guid id = Guid.NewGuid();
            string tokenSecure = id.ToString().Replace("-", "").Substring(0, 4).ToUpper();
            int expired = 5;
            var staging = new StagingVerify()
            {
                Remarks = null,
                CreateDate = DateTime.Now,
                ExpiredToken = DateTime.Now.AddMinutes(expired),
                Id = id,
                IdNumber = idNumber,
                IsUsed = false,
                Requester = requester,
                TokenSecure = tokenSecure
            };

            _context.Add(staging);
            await _context.SaveChangesAsync();

            string message = $"JANGAN BERIKAN KODE OTP ke siapapun, Kode OTP anda {tokenSecure}, berlaku {expired} menit";
            await WhatsAppUtility.SendAsync(requester, message);
        }

        public async Task<DefaultResponse> Register(RegisterRequest request)
        {
            DefaultResponse response = new()
            {
                Data = request
            };

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

                var user = await _context.MasterUsers.FirstOrDefaultAsync(i => i.IdNumber == request.IdNumber && i.PasswordHash == passwordHash);


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
                            user.PhoneNumber = requester;
                            _context.Update(user);
                        }

                        await _context.SaveChangesAsync();

                        await SendCodeRegister(idNumber: request.IdNumber, requester: requester);
                        break;
                }

                response.StatusCode = HttpStatusCode.Created;
                response.Message = "Register successfuly, please check your email or whatsapp";
                user.PasswordHash = string.Empty;
                response.Data = user;
            }
            catch (Exception e)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = e.Message;
                throw new NullReferenceException(e.Message, e.InnerException);
            }
            return response;
        }

        public async Task<DefaultResponse> AllowLogin(Guid userId, string IdNumber, bool isActive)
        {
            DefaultResponse response = new()
            {
                Data = null
            };

            try
            {
                var user = await _context.MasterUsers.FirstOrDefaultAsync(i => i.Id == userId && i.IdNumber == IdNumber);
                if (user == null)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = "User not found";
                    return response;
                }

                user.IsActive = isActive;

                _context.Update(user);
                await _context.SaveChangesAsync();
                response.Data = user;
                response.Message = "Status updated";
                response.StatusCode = HttpStatusCode.OK;

            }
            catch (Exception e)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = e.Message;
                throw new NullReferenceException(e.Message, e.InnerException);
            }
            return response;
        }

        public async Task<DefaultResponse> Login(LoginRequest request)
        {
            DefaultResponse response = new();
            response.Data = response;

            try
            {
                string passwordHash = await SecureUtility.AesEncryptAsync(request.Password ?? string.Empty);
                var masterUsers = await _context.MasterUsers
                            .Where(i =>
                            (i.Email == request.UserInput || i.PhoneNumber == request.UserInput || i.IdNumber == request.UserInput)
                            && i.PasswordHash == passwordHash)
                            .OrderByDescending(i => i.UpdatedAt)
                            .FirstOrDefaultAsync();

                if (masterUsers == null)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = "User or Password invalid";
                    return response;
                }

                if (!masterUsers.IsActive)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = "Account not active, call administrator";
                    masterUsers = null;
                    return response;
                }

                if (masterUsers != null)
                {
                    response.StatusCode = HttpStatusCode.OK;
                    masterUsers.PasswordHash = string.Empty;
                    response.Data = masterUsers;
                }
            }
            catch (Exception e)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = e.Message;
                throw new NullReferenceException(e.Message, e.InnerException);
            }
            return response;
        }

        public async Task<DefaultResponse> EmailVerify(string tokenSecure, string requester)
        {
            DefaultResponse response = new()
            {
                Data = tokenSecure
            };

            try
            {
                var staging = await _context.StagingVerifies.Where(i => i.TokenSecure == tokenSecure && i.Requester == requester)
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
                    response.Message = "Token is expired";
                    return response;
                }

                staging.IsUsed = true;
                _context.Update(staging);
                await _context.SaveChangesAsync();

                var user = await _context.MasterUsers.FirstOrDefaultAsync(i => i.IdNumber == staging.IdNumber);
                if (user != null)
                {
                    user.IsActive = true;
                    user.Email = requester;
                    user.EmailConfirmed = true;
                    _context.Update(user);
                    await _context.SaveChangesAsync();

                    user.PasswordHash = string.Empty;
                    response.StatusCode = HttpStatusCode.OK;
                    response.Data = user;
                    response.Message = "Email Success Confirmed";
                }
            }
            catch (Exception e)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = e.Message;
                throw new NullReferenceException(e.Message, e.InnerException);
            }

            return response;
        }

        public async Task<DefaultResponse> PhoneNumberVerify(string tokenSecure, string requester)
        {
            DefaultResponse response = new()
            {
                Data = tokenSecure
            };

            try
            {
                var staging = await _context.StagingVerifies.FirstOrDefaultAsync(i => i.TokenSecure == tokenSecure && i.Requester == requester);
                if (staging == null)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = "Token not found";
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

                var user = await _context.MasterUsers.FirstOrDefaultAsync(i => i.IdNumber == staging.IdNumber);
                if (user != null)
                {
                    user.IsActive = true;
                    user.PhoneNumber = requester;
                    user.PhoneNumberConfirmed = true;
                    _context.Update(user);
                    await _context.SaveChangesAsync();

                    user.PasswordHash = string.Empty;
                    response.StatusCode = HttpStatusCode.OK;
                    response.Data = user;
                    response.Message = "Phone Number Success Confirmed";
                }
            }
            catch (Exception e)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = e.Message;
                throw new NullReferenceException(e.Message, e.InnerException);
            }

            return response;
        }


    }
}
