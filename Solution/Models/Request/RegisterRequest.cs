namespace Models.Request
{
    public class RegisterRequest
    {
        public RegisterVerify RegisterVerify { get; set; }
        public string? IdNumber { get; set; }
        public string? FullName { get; set; }
        public string? UserInput { get; set; }
        public string? Password { get; set; }
    }

    public enum RegisterVerify
    {
        Email,
        PhoneNumber
    }
}
