﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EIS.WebApp.Models
{
    public class Navigation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
        public int ParentId { get; set; }

    }
}
