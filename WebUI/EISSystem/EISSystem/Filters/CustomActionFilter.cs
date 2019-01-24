
using EIS.WebApp.Models;
using EIS.WebApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace EIS.WebApp.Filters
{
    public class CustomActionFilter : IActionFilter
    {
        public RedisAgent Cache;
        public CustomActionFilter()
        {
            Cache = new RedisAgent();
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            string actionName = context.RouteData.Values["action"].ToString();
            string controllerName = context.RouteData.Values["controller"].ToString();
            string access = "/" + controllerName + "/" + actionName;

            var data = Cache.GetStringValue("Access");
            if (data != null)
            {
                List<Navigation> Access = JsonConvert.DeserializeObject<List<Navigation>>(data);
                var check = Access.Find(x => x.URL == access);
                //if (check == null)
                    //context.Result = new RedirectResult("/Account/AccessDenied");
            }
           

        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
           
        }

    }
}
