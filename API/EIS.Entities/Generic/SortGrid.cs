﻿using System;
using System.Collections.Generic;
using System.Text;

namespace EIS.Entities.Generic
{
    public class SortGrid
    {
        public string SortColumn { get; set; }
        public string SortColumnDirection { get; set; }
        public int Skip { get; set; }
        public int PageSize { get; set; }
    }
}
