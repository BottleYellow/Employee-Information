using EIS.WebApp.Models;
using EIS.WebApp.Services;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EIS.WebApp.TagHelpers
{
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
    [HtmlTargetElement("DynamicMenu")]
    //[HtmlTargetElement("HeaderWithButton")]
    public class MenuTagHelper : TagHelper
    {
        public static RedisAgent Cache = new RedisAgent();
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var ac = Cache.GetStringValue("Access");
            List<Navigation> Access = new List<Navigation>();
            if (ac != null)
                Access = JsonConvert.DeserializeObject<List<Navigation>>(ac);

            String[] ParentMenus = new String[7] { "Attendance Management", "Role Management", "Leave Management", "User Management", "", "Task", "Holidays" };
            String[] SubMenus = { "List Of Employees", "leave Policies", "View all requests", "leave Credits", "Show my leaves", "List of Roles", "Create New Attendance", "List Of Users", "Manage Roles", "Attendance Reports", "My Attendance History", "Show Employees Requests", "Employee Attendance History", "Add Task", "List of Holidays","Past Leaves","Attendance Datewise" };
            if (Cache.GetStringValue("Role") == "Admin")
                ParentMenus.SetValue("Employee Management", 4);

            foreach (var menu in Access)
            {
                if (ParentMenus.Contains(menu.Name))
                {
                    output.TagName = "li";
                    output.TagMode = TagMode.StartTagAndEndTag;
                    if (menu.ParentId == 0)
                    {
                        output.Content.AppendHtml("<a class='menu-toggle'><span>" + menu.Name + "</span></a>");
                        output.Content.AppendHtml("<ul class='ml-menu'>");
                        var subMenuList = from i in Access
                                          where i.ParentId == menu.Id
                                          select i;
                        foreach (var submenu in subMenuList)
                        {
                            if(SubMenus.Contains(submenu.Name))
                                output.Content.AppendHtml("<li><a href=" + submenu.URL + ">" + submenu.Name + "</a></li>");
                        }
                        output.Content.AppendHtml("</ul>");
                    }
                }
            }

        }
    }
}
