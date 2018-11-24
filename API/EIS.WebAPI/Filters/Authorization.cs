using EIS.Repositories.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
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
            var key = filterContext.HttpContext.Request.Headers["Authorization"].First();
            if (!IsValid(key))
            {
                // Unauthorized!
                filterContext.Result = new UnauthorizedResult();
            }
        }

        private bool IsValid(string key)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetValidationParameters();
            var token = key.Substring("Bearer ".Length).Trim();
            SecurityToken validatedToken;
            IPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
            Thread.CurrentPrincipal = principal;
            return true;
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
