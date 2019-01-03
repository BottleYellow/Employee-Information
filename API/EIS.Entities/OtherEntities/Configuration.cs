using EIS.Entities.Generic;
using System;

namespace EIS.Entities.OtherEntities
{
    public class Configuration:BaseEntity<int>
    {
        public string Code { get; set; }
        public string Value { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidUpTo { get; set; }
    }
}
