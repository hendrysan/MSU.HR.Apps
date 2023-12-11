﻿using Commons.Utilities;
using Infrastructures;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using Models.Request;
using Repositories.Interfaces;
using System.Net;

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

#pragma warning disable CS8604 // Possible null reference argument.
                await CleanUserAsync(request.IdNumber);
#pragma warning restore CS8604 // Possible null reference argument.
                var user = await _context.MasterUsers.FirstOrDefaultAsync(i => i.IdNumber == request.IdNumber && i.IsActive);

                if (user != null)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = "Id Number has been registered";
                    return response;
                }

#pragma warning disable CS8604 // Possible null reference argument.
                string passwordHash = await SecureUtility.AesEncryptAsync(value: request.Password);
#pragma warning restore CS8604 // Possible null reference argument.

                switch (request.RegisterVerify)
                {
                    case RegisterVerify.Email:
                        var entityEmail = new MasterUser()
                        {
                            Id = Guid.NewGuid(),
                            FullName = request.FullName,
                            Email = request.UserInput,
                            IdNumber = request.IdNumber,
                            PasswordHash = passwordHash,
                            IsActive = false,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        };

                        _context.Add(entityEmail);

                        await _context.SaveChangesAsync();
                        response.StatusCode = HttpStatusCode.Created;
                        await _mailRepository.SendEmailRegister(idNumber: request.IdNumber, requester: request.UserInput);
                        break;
                    case RegisterVerify.PhoneNumber:
                        var entityPhone = new MasterUser()
                        {
                            Id = Guid.NewGuid(),
                            FullName = request.FullName,
                            Email = request.UserInput,
                            IdNumber = request.IdNumber,
                            PasswordHash = passwordHash,
                            IsActive = false,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        };

                        _context.Add(entityPhone);
                        await _context.SaveChangesAsync();
                        response.StatusCode = HttpStatusCode.Created;
                        break;
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
                }
                else
                {
                    user.IsActive = isActive;

                    _context.Update(user);
                    await _context.SaveChangesAsync();
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

        public async Task<DefaultResponse> Login(LoginRequest request)
        {
            DefaultResponse response = new();
            response.Data = response;

            try
            {
                MasterUser? masterUsers = default;
                switch (request.LoginMethod)
                {
                    case LoginMethod.Email:
                        masterUsers = await _context.MasterUsers.FirstOrDefaultAsync(i => i.Email == request.UserInput);
                        if (masterUsers == null)
                        {
                            response.StatusCode = HttpStatusCode.BadRequest;
                            response.Message = "Email not register";
                            return response;
                        }

                        if (!masterUsers.IsActive)
                        {
                            response.StatusCode = HttpStatusCode.BadRequest;
                            response.Message = "Email not active, call administrator";
                            masterUsers = null;
                            return response;
                        }
                        break;
                    case LoginMethod.PhoneNumber:
                        masterUsers = await _context.MasterUsers.FirstOrDefaultAsync(i => i.PhoneNumber == request.UserInput);
                        if (masterUsers == null)
                        {
                            response.StatusCode = HttpStatusCode.BadRequest;
                            response.Message = "Phone Number not register";
                            return response;
                        }

                        if (!masterUsers.IsActive)
                        {
                            response.StatusCode = HttpStatusCode.BadRequest;
                            response.Message = "Phone Number not active, call administrator";
                            masterUsers = null;
                            return response;
                        }
                        break;
                    case LoginMethod.IdNumber:
                        masterUsers = await _context.MasterUsers.FirstOrDefaultAsync(i => i.IdNumber == request.UserInput && i.IsActive);
                        if (masterUsers == null)
                        {
                            response.StatusCode = HttpStatusCode.BadRequest;
                            response.Message = "Id Number not register";
                            masterUsers = null;
                            return response;
                        }

                        if (!masterUsers.IsActive)
                        {
                            response.StatusCode = HttpStatusCode.BadRequest;
                            response.Message = "Id Number not active, call administrator";
                            masterUsers = null;
                            return response;
                        }
                        break;
                    default:
                        response.StatusCode = HttpStatusCode.BadRequest;
                        response.Message = "Login Method not found";
                        masterUsers = null;
                        break;
                }

                if (masterUsers != null)
                {
                    response.StatusCode = HttpStatusCode.OK;
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

        public async Task<DefaultResponse> PhoneNumberVerify(string tokenSecure, string idNumber)
        {
            DefaultResponse response = new()
            {
                Data = tokenSecure
            };

            try
            {
                var staging = await _context.StagingVerifies.FirstOrDefaultAsync(i => i.TokenSecure == tokenSecure);
                if (staging == null)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = "Token not found";
                    return response;
                }

                if (staging.IsUsed)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = "Token is used";
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

                var user = await _context.MasterUsers.FirstOrDefaultAsync(i => i.PhoneNumber == staging.Requester && i.IsActive);
                if (user != null)
                {
                    user.EmailConfirmed = true;
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    response.Data = user;

                    response.Message = "Email Confirmed";
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

        public async Task<DefaultResponse> EmailVerify(string tokenSecure, string idNumber)
        {
            DefaultResponse response = new()
            {
                Data = tokenSecure
            };

            try
            {
                var staging = await _context.StagingVerifies.FirstOrDefaultAsync(i => i.TokenSecure == tokenSecure && !i.IsUsed);
                if (staging == null)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.Message = "Token not found";
                }
                else
                {
                    if (staging.ExpiredToken <= DateTime.Now)
                    {
                        response.StatusCode = HttpStatusCode.BadRequest;
                        response.Message = "Token is expired";
                    }
                    else
                    {
                        staging.IsUsed = true;
                        _context.Update(staging);
                        await _context.SaveChangesAsync();

                        var user = await _context.MasterUsers.FirstOrDefaultAsync(i => i.Email == staging.Requester && i.IsActive);
                        if (user != null)
                        {
                            user.EmailConfirmed = true;
                            _context.Update(user);
                            await _context.SaveChangesAsync();
                            response.Data = user;
                        }
                    }
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
