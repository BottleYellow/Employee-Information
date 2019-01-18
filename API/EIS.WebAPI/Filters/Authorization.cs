using EIS.WebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace EIS.WebAPI.Filters
{
    public class Authorization : AuthorizeAttribute,IAuthorizationFilter
    {
        public RedisAgent Cache;
        public Authorization()
        {
            Cache = new RedisAgent();
        }
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            bool skipAuthorization = filterContext.Filters.Any(item => item is IAllowAnonymousFilter);
            if (skipAuthorization)
            {
                return;
            }
            try
            {
                string token = filterContext.HttpContext.Request.Headers["Token"].ToString();
                string Access = Cache.GetStringValue("TokenValue");

                if (token != Access)
                {
                    // unauthorized!
                    filterContext.Result = new UnauthorizedResult();
                }
                
            }
            catch (InvalidOperationException)
            {
                filterContext.Result = new UnauthorizedResult();
            }
        }
        
    }
}
