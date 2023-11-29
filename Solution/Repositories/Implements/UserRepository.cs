﻿using Infrastructures;
using Models.Request;
using Repositories.Interfaces;

namespace Repositories.Implements
{
    public class UserRepository : IUserRepository
    {
        private readonly ConnectionContext _context;

        public UserRepository(ConnectionContext context)
        {
            _context = context;
        }

        public async Task<bool> Register(RegisterRequest request)
        {
            try
            {
                var entity = new Models.Entities.User()
                {
                    FullName= request.Name,
                    PasswordHash = request.Password,
                    Email = request.Email,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.Users.Add(entity);
                var task = await _context.SaveChangesAsync();
                return task > 0;

            }
            catch (Exception e)
            {
                throw new NullReferenceException(e.Message, e.InnerException);
            }
        }

        public Task<bool> ChangePassword()
        {
            throw new NotImplementedException();
        }

        public Task<bool> ConfirmPassword()
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete()
        {
            throw new NotImplementedException();
        }

        public Task<bool> Login()
        {
            throw new NotImplementedException();
        }



        public Task<bool> ResetPassword()
        {
            throw new NotImplementedException();
        }
    }
}
