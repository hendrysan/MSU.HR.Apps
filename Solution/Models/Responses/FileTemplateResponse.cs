namespace Models.Responses
{
    public class FileTemplateResponse : DefaultResponse
    {
        public byte[]? FileBytes { get; set; }
        public string? FileType { get; set; }
        public string? FileName { get; set; }
    }
}
