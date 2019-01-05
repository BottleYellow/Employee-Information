
using EIS.WebApp.Models;
using EIS.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;

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
                if (check==null)
                    context.Result = new RedirectResult("/Account/AccessDenied");
            }
           
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {

        }

    }
}
