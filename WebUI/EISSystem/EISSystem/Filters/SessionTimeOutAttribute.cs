using EIS.Entities.OtherEntities;
using EIS.WebApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EIS.WebApp.Filters
{
    public class SessionTimeOutAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string val = filterContext.HttpContext.Session.GetString("CookieData");
            CookieModel Cookies = val != null ? JsonConvert.DeserializeObject<CookieModel>(val) : new CookieModel();
            string AppUrl = MyHttpContext.AppBaseUrl;
            if (val == null)
            {
                filterContext.Result = new RedirectResult(AppUrl);
                return;
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
