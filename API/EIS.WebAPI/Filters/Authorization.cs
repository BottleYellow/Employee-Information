using EIS.Data.Context;
using EIS.Repositories.IRepository;
using EIS.Repositories.Methods;
using EIS.Repositories.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EIS.WebAPI.Filters
{
    public class Authorization : AuthorizeAttribute,IAuthorizationFilter
    {
       
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            
            bool skipAuthorization = filterContext.Filters.Any(item => item is IAllowAnonymousFilter);
           
            if (skipAuthorization)
            {
                return;
            }
            try
            {
                Exception ex;
                var key = filterContext.HttpContext.Request.Headers["Authorization"].First();
                
                if (!IsValid(key,out ex))
                {
                    // Unauthorized!

                    filterContext.Result = new CustomUnauthorizedResult(ex.Message);
                }
            }
            catch (InvalidOperationException)
            {
                filterContext.Result = new UnauthorizedResult();
            }
        }

        private bool IsValid(string key,out Exception ex)
        {
            var tokenHandler = new RepositoryWrapper(new ApplicationDbContext(new DbContextOptions<ApplicationDbContext>()));
            var validationParameters = GetValidationParameters();
            var token = key.Substring("Bearer ".Length).Trim();
            if (tokenHandler.Users.IsValidToken(token,out ex))
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
