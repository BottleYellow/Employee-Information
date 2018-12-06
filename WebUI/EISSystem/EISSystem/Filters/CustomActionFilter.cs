using EIS.Repositories.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace EIS.WebApp.Filters
{
    public class CustomActionFilter : IActionFilter, IResultFilter
    {
        IRepositoryWrapper repository;
        IDistributedCache distributedCache;
        public CustomActionFilter(IRepositoryWrapper repository, IDistributedCache distributedCache)
        {
            this.repository = repository;
            this.distributedCache = distributedCache;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.HttpContext.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
                context.Result = new RedirectToActionResult("Login", "Account", routeValues:null);
           
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            string actionName = context.RouteData.Values["action"].ToString();
            string controllerName = context.RouteData.Values["controller"].ToString();
            var options = new DistributedCacheEntryOptions();
            options.SetAbsoluteExpiration(TimeSpan.FromSeconds(15));
            distributedCache.SetString("Action", actionName,options);
            distributedCache.SetString("Controller", controllerName,options);
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {

        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
           
        }
    }
}
