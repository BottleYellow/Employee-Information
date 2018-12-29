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
            bool skipAuthorization = filterContext.Filters.Any(item => item is IAllowAnonymousFilter);
            if (skipAuthorization)
            {
                return;
            }
            try
            {
                string token = Cache.GetStringValue("TokenValue");
                if (token == null)
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
