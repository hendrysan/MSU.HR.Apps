namespace Models.Responses
{
    public class SearchWorkDayResponse : DefaultResponse
    {
        public List<DetailWorkDay>? DetailWorkDays { get; set; }
    }

    public class DetailWorkDay
    {
        public DateTime? Day { get; set; }
        public string? Value { get; set; }
    }
}
