namespace WebClient.ViewModels.WorkDay
{
    public class CalendarDataRequest
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }

    public class CalendarDataResponse
    {
        public string? Title { get; set; }
        public DateTime Start { get; set; }
    }
}
