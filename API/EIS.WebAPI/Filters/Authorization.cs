using EIS.Repositories.IRepository;
using EIS.WebAPI.RedisCache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Linq;

namespace EIS.WebAPI.Filters
{
    public class Authorization : AuthorizeAttribute,IAuthorizationFilter
    {
        public RedisAgent Cache;
        public readonly IDistributedCache distributedCache;
        public readonly IRepositoryWrapper repositoryWrapper;
        public Authorization(IDistributedCache _distributedCache, IRepositoryWrapper _repositoryWrapper)
        {
            distributedCache = _distributedCache;
            repositoryWrapper = _repositoryWrapper;
            Cache = new RedisAgent();
        }
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        { 
            var action = filterContext.ActionDescriptor.RouteValues["Action"].ToString();
            var controller= filterContext.ActionDescriptor.RouteValues["Controller"].ToString();
            var propInfo = action.GetType().GetProperty("DisplayName");
            bool skipAuthorization = filterContext.Filters.Any(item => item is IAllowAnonymousFilter);
            if (skipAuthorization)
            {
                return;
            }
            try
            {
                string token = filterContext.HttpContext.Request.Headers["Token"].ToString();
                token = "dsjfhsjadhfkjhajsfh";
                //if (filterContext.HttpContext.Request.Headers.TryGetValue("Custom", out string headerValues))
                //{
                //    string token = headerValues.First();
                //}
                //string token = Cache.GetStringValue("TokenValue");
                if (string.IsNullOrEmpty(token))
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
