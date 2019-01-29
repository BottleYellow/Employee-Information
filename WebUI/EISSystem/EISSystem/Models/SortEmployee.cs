using System.Collections.Generic;

namespace EIS.WebApp.Models
{
    public class SortEmployee
    {
        public string SortColumn { get; set; }
        public string SortColumnDirection { get; set; }
        public int Skip { get; set; }
        public int PageSize { get; set; }
        public string Search { get; set; }
        public bool? IsActive { get; set; }
    }

    public class ViewModel<T>
    {
        public int TotalCount { get; set; }
        public IList<T> collection { get; set; }
    }
}
