namespace WebClient.ViewModels.Auth
{
    public class OtpVerifyFormRequest
    {
        public string Requester { get; set; } = string.Empty;
        public string IdNumber { get; set; } = string.Empty;
        public string OtpSecure { get; set; } = string.Empty;
    }
}
