namespace Models.Requests
{
    public class LoginRequest
    {
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
