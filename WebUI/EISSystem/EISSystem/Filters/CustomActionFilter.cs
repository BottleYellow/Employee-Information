
using EIS.Entities.OtherEntities;
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
           

        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            string val = context.HttpContext.Session.GetString("CookieData");
            CookieModel Cookies = val != null ? JsonConvert.DeserializeObject<CookieModel>(val) : new CookieModel();
            string actionName = context.RouteData.Values["action"].ToString();
            string controllerName = context.RouteData.Values["controller"].ToString();
            string access = "/" + controllerName + "/" + actionName;
            if (access == "/Account/Login" || access=="/Account/Logout")
            {
                return;
            }
            string AppUrl = MyHttpContext.AppBaseUrl;
            string data = Cookies.Access;
            if (data != null)
            {
                List<Navigation> Access = JsonConvert.DeserializeObject<List<Navigation>>(data);
                Navigation check = Access.Find(x => x.URL == access);
                if (check == null)
                    context.Result = new RedirectResult(""+AppUrl+"/Account/AccessDenied");
            }

        }

    }
}
