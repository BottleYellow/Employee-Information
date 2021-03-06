﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace EIS.WebApp.Filters
{
    public class ErrorHandlingFilter : ExceptionFilterAttribute
    {
        private readonly ILoggerFactory loggerFactory;
        public ErrorHandlingFilter(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
        }
        public override void OnException(ExceptionContext context)
        {
            HandleExceptionAsync(context,loggerFactory);
            context.ExceptionHandled = true;
        }
        private static void HandleExceptionAsync(ExceptionContext context,ILoggerFactory loggerFactory)
        {
            Exception exception = context.Exception;
            if (exception is UnauthorizedAccessException)
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
            }
            else
            {             
                    ILogger logger = loggerFactory.CreateLogger("Serilog Global exception logger");
                    logger.LogError(500, exception, exception.Message);
                
                int code = context.HttpContext.Response.StatusCode;
                context.Result = new RedirectToActionResult("ErrorPage", "Account", new { Errordata = exception.Message, StackTrace= exception.StackTrace});
            }
        }
    }
}
