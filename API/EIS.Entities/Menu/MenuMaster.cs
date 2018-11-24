using EIS.Entities.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace EIS.Entities.Menu
{
    public class MenuMaster : BaseEntity<int>
    {
        public string MenuId { get; set; }
        public string MenuName { get; set; }
        public string ParentMenuId { get; set; }
        public string MenuFileName { get; set; }
        public string MenuURL { get; set; }
        public char USE_YN { get; set; }
        public string RoleId { get; set; }
    }
}
