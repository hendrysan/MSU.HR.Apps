using Models.Request;

namespace Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<DefaultResponse> Register(RegisterRequest request);
        Task<DefaultResponse> Login(string userName, string password);
    }
}
