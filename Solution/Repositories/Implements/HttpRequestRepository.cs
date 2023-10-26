using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Implements
{
    public class HttpRequestRepository : IHttpRequestRepository
    {
        public async Task<string?> GetRequest(string url)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);

                var result = await response.Content.ReadAsStringAsync();
                return result;

            }
        }
    }
}
