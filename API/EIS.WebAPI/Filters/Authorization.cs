using EIS.Data.Context;
using EIS.Repositories.IRepository;
using EIS.Repositories.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
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
        public readonly IDistributedCache distributedCache;
        public readonly IRepositoryWrapper repositoryWrapper;
        public Authorization(IDistributedCache _distributedCache, IRepositoryWrapper _repositoryWrapper)
        {
            distributedCache = _distributedCache;
            repositoryWrapper = _repositoryWrapper;
        }
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            string actionName = filterContext.HttpContext.Request.Headers["Action"].ToString();
            string controllerName = filterContext.HttpContext.Request.Headers["Controller"].ToString();
            //Authentication
            bool skipAuthorization = filterContext.Filters.Any(item => item is IAllowAnonymousFilter);
            if (skipAuthorization)
            {
                return;
            }
            try
            { 
                string token = distributedCache.GetString("TokenValue");
                if (token == null)
                {
                    // unauthorized!
                    filterContext.Result = new UnauthorizedResult();
                }
                //else
                //{
                //    string access = "/" + controllerName + "/" + actionName;
                //    string role = "Manager";
                //    var data = repositoryWrapper.RoleManager.FindByCondition(r => r.Name == role).Access;
                //    if (!data.Contains(access))
                //    {
                //        filterContext.Result = new UnauthorizedResult();
                //    }
                //}

            }
            catch (InvalidOperationException)
            {
                filterContext.Result = new UnauthorizedResult();
            }
        }
        
    }
}
