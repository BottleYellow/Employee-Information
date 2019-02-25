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
        }      
    }
}
