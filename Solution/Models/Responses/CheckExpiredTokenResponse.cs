namespace Models.Responses
{
    public class CheckExpiredTokenResponse : DefaultResponse
    {
        public string? Minutes { get; set; }
        public TimeSpan? Ticks { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? IdNumber { get; set; }
    }
}
