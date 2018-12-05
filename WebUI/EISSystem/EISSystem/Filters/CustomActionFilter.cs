using EIS.Repositories.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace EIS.WebApp.Filters
{
    public class CustomActionFilter : IActionFilter, IResultFilter
    {
        IRepositoryWrapper repository;
        public CustomActionFilter(IRepositoryWrapper repository)
        {
            this.repository = repository;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.HttpContext.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
                context.Result = new RedirectResult("/Login/Login");
            //string role = "Manager";
            //var data = repository.RoleManager.FindByCondition(r => r.Name == role).Access;
            //if (!data.Contains(context.HttpContext.Request.Path))
            //{
            //    context.Result = new RedirectResult("/Login/Login");
            //}
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {

        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
           
        }
    }
}
