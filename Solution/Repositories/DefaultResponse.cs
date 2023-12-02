namespace Repositories
{
    public class DefaultResponse
    {
        public System.Net.HttpStatusCode StatusCode { get; set; }
        public string? Message { get; set; }
        public dynamic? Data { get; set; }
    }
}
