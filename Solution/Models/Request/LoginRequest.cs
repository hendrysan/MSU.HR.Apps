namespace Models.Request
{
    public class LoginRequest
    {
        public LoginMethod LoginMethod { get; set; }
        public string? UserInput { get; set; }
        public string? Password { get; set; }
    }

    public enum LoginMethod
    {
        Email,
        PhoneNumber,
        IdNumber
    }
}
