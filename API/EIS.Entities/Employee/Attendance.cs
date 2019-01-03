﻿using EIS.Entities.Generic;
using System;
using System.ComponentModel.DataAnnotations;

namespace EIS.Entities.Employee
{
    public class Attendance : BaseEntity<int>
    {
        public int PersonId { get; set; }
        [DataType(DataType.Date)]
        public DateTime DateIn { get; set; }
        [DataType(DataType.Time)]
        public TimeSpan TimeIn { get; set; }
        [DataType(DataType.Date)]
        public DateTime DateOut { get; set; }
        [DataType(DataType.Time)]
        public TimeSpan TimeOut { get; set; }
        public TimeSpan TotalHours { get; set; }
        public virtual Person Person { get; set; }
    }
}
