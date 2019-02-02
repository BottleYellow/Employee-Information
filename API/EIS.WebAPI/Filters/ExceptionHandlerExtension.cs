using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace EIS.WebAPI.ExceptionHandle
{

    public static class ExceptionMiddlewareExtensions
    {
        public static void UseWebApiExceptionHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerFactory _logger;

        public ExceptionMiddleware(RequestDelegate next, ILoggerFactory logger)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                var logger = _logger.CreateLogger("Serilog Global exception logger");
                logger.LogError(500,ex, ex.Message);
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode status = HttpStatusCode.InternalServerError;
            string message;
            int code;

            var exceptionType = exception.GetType();
            if (exceptionType == typeof(UnauthorizedAccessException))
            {
                message = "Unauthorized Access";
                status = HttpStatusCode.Unauthorized;
                code = 404;
            }
            else if (exceptionType == typeof(NotImplementedException))
            {
                message = "A server error occurred.";
                status = HttpStatusCode.NotImplemented;
                code = 404;
            }
            else if (exceptionType == typeof(DivideByZeroException))
            {
                message = "Cannot divide by zero";
                status = HttpStatusCode.EarlyHints;
                code = 404;
            }

            else
            {
                message = exception.Message;
                status = HttpStatusCode.NotFound;
                code = 500;
            }

            context.Response.ContentType = "application/json";

            return context.Response.WriteAsync("An unexpected fault happened. Status Code: " +code+ " occurred \n Status:- " + status + " occurred.\n Message :" + message);
        }
    }
}