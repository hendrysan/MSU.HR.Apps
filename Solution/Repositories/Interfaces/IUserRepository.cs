using Models.Requests;
using Models.Responses;

namespace Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<LoginResponse> Login(LoginRequest request);
        Task<DefaultResponse> Register(RegisterRequest request);
        Task<DefaultResponse> EmailVerify(string tokenSecure, string requester);
        Task<DefaultResponse> PhoneNumberVerify(string tokenSecure, string requester, string idNumber);
        Task<DefaultResponse> AllowLogin(Guid userId, string IdNumber, bool isActive);
        Task<DefaultResponse> CheckExpiredToken(string requester, string idNumber);

    }
}
