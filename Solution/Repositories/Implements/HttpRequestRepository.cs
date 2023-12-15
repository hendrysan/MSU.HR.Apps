using Repositories.Interfaces;

namespace Repositories.Implements
{
    public class HttpRequestRepository : IHttpRequestRepository
    {
        public async Task<string?> GetRequest(string url)
        {
            using var client = new HttpClient();
            var response = await client.GetAsync(url);

            var result = await response.Content.ReadAsStringAsync();
            return result;
        }
    }
}
