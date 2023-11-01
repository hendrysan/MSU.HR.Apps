namespace Models.Others
{
    public class ErrorModel
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public int ErrorCode { get; set; }
        public object? Data { get; set; }
    }
}
