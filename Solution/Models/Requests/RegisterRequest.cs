namespace Models.Requests
{
    public class RegisterRequest
    {

        public RegisterVerify RegisterVerify { get; set; }
        public required string IdNumber { get; set; }
        public required string FullName { get; set; }
        public required string UserInput { get; set; }
        public required string Password { get; set; }
    }

    public enum RegisterVerify
    {
        Email,
        PhoneNumber
    }
}
