namespace Repositories.Interfaces
{
    public interface IHttpRequestRepository
    {
        Task<string?> GetRequest(string url);
    }
}
