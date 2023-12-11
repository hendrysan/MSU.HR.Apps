namespace Models.Entities
{
    public class StagingVerify
    {
        public Guid Id { get; set; }
        public DateTime CreateDate { get; set; }
        public string? IdNumber { get; set; }
        public string? Requester { get; set; }
        public string? TokenSecure { get; set; }
        public DateTime ExpiredToken { get; set; }
        public bool IsUsed { get; set; } = false;
        public string? Remarks { get; set; }
    }
}
