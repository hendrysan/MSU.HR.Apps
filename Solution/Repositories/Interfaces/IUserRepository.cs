using Models.Request;

namespace Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<DefaultResponse> Login(LoginRequest request);
        Task<DefaultResponse> Register(RegisterRequest request);
        Task<DefaultResponse> EmailVerify(string tokenSecure, string idNumber);
        Task<DefaultResponse> PhoneNumberVerify(string tokenSecure, string idNumber);
        Task<DefaultResponse> AllowLogin(Guid userId, string IdNumber, bool isActive);

    }
}
