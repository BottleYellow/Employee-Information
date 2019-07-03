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
        public string Access
        {
            get;
            set;
        }
        public string Role
        {
            get;
            set;
        }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            string ac = Access;
            List<Navigation> AccessList = new List<Navigation>();
            if (ac != null)
                AccessList = JsonConvert.DeserializeObject<List<Navigation>>(ac);
            string appBaseUrl = MyHttpContext.AppBaseUrl;

            String[] ParentMenus = new String[9] { "Attendance Management", "Role Management", "Leave Management", "User Management", "", "Task", "Holidays","Locations","Admin Management" };
            String[] SubMenus = { "List Of Employees", "Leave Policies", "View All Requests", "Leave Credits", "Show My Leaves", "List Of Roles", "Create New Attendance", "List Of Users", "Manage Roles", "Attendance Reports", "Brief Attendance Report", "My Attendance History", "Show Employees Requests", "Employee Attendance History", "Add Task", "List Of Holidays","Attendance Datewise","Manage Locations","Show Holidays", "Attendance Summary", "Attendance Modify","List Of Admins", "My Brief Attendance" };
            if (Role == "Admin" || Role=="HR" || Role == "Manager")
                ParentMenus.SetValue("Employee Management", 4);
            foreach (var menu in AccessList)
            {
                if (ParentMenus.Contains(menu.Name))
                {
                    output.TagName = "li";
                    output.TagMode = TagMode.StartTagAndEndTag;
                    if (menu.ParentId == 0)
                    {
                        List<Navigation> subMenuList = (from i in AccessList
                                                        where i.ParentId == menu.Id
                                                        select i).ToList();
                        List<Navigation> NewList = new List<Navigation>();
                        foreach (var submenu in subMenuList)
                            if (SubMenus.Contains(submenu.Name))
                                NewList.Add(submenu);
                        if (NewList.Count() == 1)
                        {
                            var submenu = NewList.FirstOrDefault();
                            output.Content.AppendHtml("<a href=" + appBaseUrl + submenu.URL + "><span>" + submenu.Name + "</span></a>");
                        }
                        else
                        {
                            int cnt = 0;
                            foreach (var submenu in subMenuList)
                                if (SubMenus.Contains(submenu.Name))
                                    cnt++;
                            if (cnt > 0)
                            {
                                output.Content.AppendHtml("<a class='menu-toggle'><span>" + menu.Name + "</span></a>");
                                output.Content.AppendHtml("<ul class='ml-menu'>");

                                foreach (var submenu in subMenuList)
                                {
                                    if (SubMenus.Contains(submenu.Name))
                                    {
                                        string href = appBaseUrl + submenu.URL;
                                        output.Content.AppendHtml("<li><a href=" + href + ">" + submenu.Name + "</a></li>");
                                    }
                                }
                                output.Content.AppendHtml("</ul>");
                            }
                        }
                    }
                }
            }

        }
    }
}
