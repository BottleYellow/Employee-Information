using EIS.Repositories.IRepository;
using EIS.WebApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
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
            string data = Cache.GetStringValue("Access");
            if (data != null)
            {
                var Data = JsonConvert.DeserializeObject<List<string>>(data);
                CultureInfo culture = new CultureInfo("en-US");
                if (!Data.Contains(access))
                {
                    context.Result = new RedirectResult("/Account/AccessDenied");
                }
            }
            //else if (!("/Account/Login","/Account/ForgotPassword").ToString().Contains(access))
            //{
            //    context.Result = new RedirectToActionResult("Login", "Account", routeValues: null);
            //}
            if (context.HttpContext.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
                context.Result = new RedirectToActionResult("Login", "Account", routeValues:null);
           
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            string actionName = context.RouteData.Values["action"].ToString();
            string controllerName = context.RouteData.Values["controller"].ToString();
            Cache.SetStringValue("Action", actionName);
            Cache.SetStringValue("Controller", controllerName);
        }

    }
}
