using Models.Responses;

namespace Repositories.Interfaces
{
    public interface IMailRepository
    {
        public Task<DefaultResponse> SendEmailRegister(string idNumber, string requester, string? remarks = null);
    }
}
