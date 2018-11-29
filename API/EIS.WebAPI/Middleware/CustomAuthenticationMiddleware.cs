using EIS.Data.Context;
using EIS.Repositories.Repository;
using EIS.WebAPI.Values;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EIS.WebAPI.Middleware
{
    public class CustomAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        public CustomAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments(SkipSecurity.Login))
            {
                await _next.Invoke(context);
            }
            else
            {
                try
                {
                    Exception ex;
                    var key = context.Request.Headers["Authorization"].First();

                    if (!IsValid(key, out ex))
                    {

                        context.Response.StatusCode = 401; //Unauthorized
                        await context.Response.WriteAsync(ex.Message, new System.Threading.CancellationToken());
                        return;
                        //filterContext.Result = new CustomUnauthorizedResult(ex.Message);
                    }
                    else
                    {

                        await _next.Invoke(context);
                    }
                }
                catch (InvalidOperationException)
                {
                    context.Response.StatusCode = 401; //Unauthorized
                    return;
                }
            }
        }
        private bool IsValid(string key, out Exception ex)
        {
            var tokenHandler = new RepositoryWrapper(new ApplicationDbContext(new DbContextOptions<ApplicationDbContext>()));
            var validationParameters = GetValidationParameters();
            var token = key.Substring("Bearer ".Length).Trim();
            if (tokenHandler.Users.IsValidToken(token, out ex))
            {
                return true;
            }
            return false;

        }

        private static TokenValidationParameters GetValidationParameters()
        {
            return new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("askjdkasdakjsdaksdasdjaksjdadfgdfgkjdda")),
                ValidIssuer = "mysite.com",
                ValidAudience = "mysite.com",
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true
            };
        }
    }
}
