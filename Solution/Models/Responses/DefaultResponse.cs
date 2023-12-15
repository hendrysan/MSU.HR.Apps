namespace Models.Responses
{
    public class DefaultResponse
    {
        public System.Net.HttpStatusCode StatusCode { get; set; } = System.Net.HttpStatusCode.OK;
        public string Message { get; set; } = string.Empty;

    }
}
