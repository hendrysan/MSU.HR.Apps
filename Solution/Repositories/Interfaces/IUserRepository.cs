using Models.Request;

namespace Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> Register(RegisterRequest request);
        Task<bool> Login();
        Task<bool> ResetPassword();
        Task<bool> ChangePassword();
        Task<bool> Delete();
        Task<bool> ConfirmPassword();

    }
}
