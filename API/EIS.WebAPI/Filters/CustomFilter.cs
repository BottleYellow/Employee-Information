using EIS.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EIS.WebAPI.Filters
{
    public class CustomFilter : ActionFilterAttribute, IActionFilter
    {

        private readonly IControllerService _controllerService;
        public CustomFilter(IControllerService controllerService)
        {
            _controllerService = controllerService;
        }
        public override void OnActionExecuted(ActionExecutedContext context)
        {
        //    string actionName = context.RouteData.Values["action"].ToString();
        //    string controllerName = context.RouteData.Values["controller"].ToString();
        //    var data = _controllerService.GetControllers();
        //    string displayName = null;
        //    foreach (var c in data)
        //    {
        //        string cc = c.Name;
        //        foreach (var a in c.Actions)
        //        {
        //            string ca = a.Name;
        //            if (cc == controllerName && ca == actionName)
        //            {
        //                displayName = a.DisplayName;
        //            }
        //        }
        //    }

        //    var access = Cache.GetStringValue("Access");
        //    if (displayName != "Logout")
        //    {
        //        if (displayName != null && access != null)
        //        {
        //            if (!access.Contains(displayName))
        //            {
        //                context.Result = new UnauthorizedResult();
        //            }
        //        }
        //        else
        //        {
        //            context.Result = new UnauthorizedResult();
        //        }
        //    }
        }      
    }
}
