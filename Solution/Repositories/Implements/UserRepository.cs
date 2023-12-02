using Infrastructures;
using Microsoft.EntityFrameworkCore;
using Models.Request;
using Repositories.Interfaces;
using System.Net;

namespace Repositories.Implements
{
    public class UserRepository(ConnectionContext context) : IUserRepository
    {
        private readonly ConnectionContext _context = context;

        public Task<DefaultResponse> Login(string userName, string password)
        {
            throw new NotImplementedException();
        }

        public async Task<DefaultResponse> Register(RegisterRequest request)
        {
            DefaultResponse response = new();
            response.Data = response;
            try
            {
                var user = await _context.Users.Where(i => i.Email == request.Email && i.IsActive).FirstOrDefaultAsync();

                if (user != null)
                {
                    response.Message = "Email is registered";
                    response.StatusCode = HttpStatusCode.BadRequest;
                    return response;
                }

                var entity = new Models.Entities.User()
                {
                    FullName = request.Name,
                    PasswordHash = request.Password,
                    Email = request.Email,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.Users.Add(entity);
                var task = await _context.SaveChangesAsync();

                if (task > 0)
                    response.StatusCode = HttpStatusCode.Created;
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
