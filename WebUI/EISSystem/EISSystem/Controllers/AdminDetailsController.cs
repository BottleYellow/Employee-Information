using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using EIS.Entities.Employee;
using EIS.Entities.SP;
using EIS.WebApp.Filters;
using EIS.WebApp.IServices;
using EIS.WebApp.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EIS.WebApp.Controllers
{
    [SessionTimeOut]
    [DisplayName("Admin Management")]
    public class AdminDetailsController : BaseController<Person>
    {
        private readonly IHostingEnvironment _environment;
        #region Declarations
        public readonly IServiceWrapper _services;
        private readonly IControllerService _controllerService;
        public static HttpResponseMessage response;
        public MyHttpContext httpContext;

        #endregion 

        #region Controller
        public AdminDetailsController(IEISService<Person> service, IControllerService controllerService, IServiceWrapper services, IHostingEnvironment environment) : base(service)
        {
            _environment = environment;
            _controllerService = controllerService;
            _services = services;
        }
        #endregion

        [DisplayName("List of Admins")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(bool type)
        {
            HttpResponseMessage response = _services.SP_GetEmployee.GetResponse(ApiUrl + "/api/employee/AdminData/" + type);
            //return LoadData<Person>(ApiUrl + "/api/employee/data", type, LocationId);
            string stringData = response.Content.ReadAsStringAsync().Result;
            List<SP_GetEmployee> sP_GetEmployee = JsonConvert.DeserializeObject<List<SP_GetEmployee>>(stringData);
            return Json(sP_GetEmployee);

        }
    }
}
