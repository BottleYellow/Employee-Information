using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace EIS.WebApp.Filters
{
    public class ErrorHandlingFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            HandleExceptionAsync(context);
            context.ExceptionHandled = true;
        }
        private static void HandleExceptionAsync(ExceptionContext context)
        {
            var exception = context.Exception;
            if (exception is UnauthorizedAccessException)
            {
                context.Result = new RedirectResult("/Account/AccessDenied");
            }
            else
            {
                context.HttpContext.Response.WriteAsync("An unexpected fault happened. Status Code occurred");
            }
        }
    }
}
