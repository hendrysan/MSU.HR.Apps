namespace Models.Requests
{
    public class DataTableRequest
    {
        public string? draw { get; set; }
        public string? SortColumn { get; set; }
        public string? SortColumnDirection { get; set; }
        public string? SearchValue { get; set; }
        public int PageSize { get; set; } = 10;
        public int Skip { get; set; } = 0;
    }
}
