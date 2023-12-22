namespace Models.Responses
{
    public class DataTableResponse
    {
        public string? Draw { get; set; }
        public int? RecordsTotal { get; set; }
        public int? RecordsFiltered { get; set; }
        public object? Data { get; set; }
    }
}
