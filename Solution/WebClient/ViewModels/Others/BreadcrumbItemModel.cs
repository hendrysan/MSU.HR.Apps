namespace WebClient.ViewModels.Others
{
    public class BreadcrumbItemModel
    {
        public int Sequence { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public bool IsActive { get; set; } = false;
    }
}
