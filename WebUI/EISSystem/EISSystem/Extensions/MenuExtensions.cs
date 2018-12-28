using EIS.WebApp.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EIS.WebApp.Services;
using Newtonsoft.Json;
using System.Text;

namespace EIS.WebApp.Extensions
{
    public static class MenuExtensions
    {
        public static RedisAgent Cache = new RedisAgent();
        
        public static IHtmlContent HtmlMenu(this IHtmlHelper htmlHelper)
        {
            var ac = Cache.GetStringValue("Access");
            List<Navigation> Access = new List<Navigation>();
            if (ac != null)
                Access = JsonConvert.DeserializeObject<List<Navigation>>(ac);
            StringBuilder output = new StringBuilder();
            //if (Access.Count > 0)
            //{
            //    output.Append("<ul>");

            //    foreach (MyObject subItem in _object.listOfObjects)
            //    {
            //        output.Append("<li>");
            //        output.Append(_object.Title);
            //        output.Append(html.ShowSubItems(subItem.listOfObjects);
            //        output.Append("</li>");
            //    }
            //    output.Append("</ul>");
            //}
            return new HtmlString(output.ToString());
        }
         //@foreach (var Menu in Access)
         //           {
         //               <li>
         //                   @if (Menu.AccessLinkName == Menu.ParentLinkName)
         //                   {
                                
         //                       <a class="menu-toggle">                                
         //                           <span>@Menu.ParentLinkName</span>
         //                       </a>
         //                       <ul class="ml-menu">
         //                           @foreach (var Menu2 in Access)
         //                           {
         //                               if(Menu2.ParentLinkName==Menu.ParentLinkName)
         //                               { 
         //                                    <li>
         //                                        <a href="@Menu2.AccessLink">@Menu2.AccessLinkName</a>
         //                                    </li>
         //                               }
                                       
         //                           }
         //                       </ul>
                                
         //                   }
         //               </li>
         //            }
        

    }
}
