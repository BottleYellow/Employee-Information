using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;

namespace EIS.WebApp.Filters
{
    public static class ExceptionHandlerExtension
    {
        public static IApplicationBuilder UseWebAppExceptionHandler(this IApplicationBuilder app)
        {
            ILoggerFactory loggerFactory = app.ApplicationServices.GetService(typeof(ILoggerFactory)) as ILoggerFactory;

            return app.UseExceptionHandler(HandleAppException(loggerFactory));
        }

        public static Action<IApplicationBuilder> HandleAppException(ILoggerFactory loggerFactory)
        {
            return appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    IExceptionHandlerFeature exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();

                    if (exceptionHandlerFeature != null)
                    {
                        ILogger logger = loggerFactory.CreateLogger("Serilog Global exception logger");
                        logger.LogError(500, exceptionHandlerFeature.Error, exceptionHandlerFeature.Error.Message);
                    }

                    int code = context.Response.StatusCode;
                    //context.Response.Redirect("/Account/ErrorPage");
                    await context.Response.WriteAsync("An unexpected fault happened. Status Code " + code + " occurred \n"+ exceptionHandlerFeature.Error.Message +"\n"+exceptionHandlerFeature.Error.StackTrace);
                   
                });
            };
        }
    }
}
