using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace EIS.WebApp.Filters
{
    public class CustomActionFilter : IActionFilter, IResultFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.HttpContext.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
                context.Result = new RedirectResult("/Login/Login");
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {

        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
           
        }
    }
}
