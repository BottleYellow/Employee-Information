namespace EIS.WebApp.Models
{
    public class SortEmployee
    {
        public string SortColumn { get; set; }
        public string SortColumnDirection { get; set; }
        public int Skip { get; set; }
        public int PageSize { get; set; }
    }
}
