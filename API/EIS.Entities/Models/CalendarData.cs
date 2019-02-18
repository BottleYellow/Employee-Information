using System;

namespace EIS.Entities.Models
{
        public class CalendarData
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public string Color { get; set; }
            public string MyProperty { get; set; }
            public bool IsFullDay { get; set; }
        }
}
