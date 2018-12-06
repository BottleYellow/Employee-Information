using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using EIS.Entities.User;
using EIS.WebApp.IServices;
using EIS.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EIS.WebApp.Controllers
{
    public class RoleController : Controller
    {
        private readonly IControllerService _controllerService;
        private readonly IEISService _service;

        public RoleController(IControllerService controllerService, IEISService service)
        {
            _service = service;
            _controllerService = controllerService;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            ViewData["Controllers"] = _controllerService.GetControllers();

            return View();
        }

        // POST: Role/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RoleViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Controllers"] = _controllerService.GetControllers();
                return View(viewModel);
            }

            var role = new Role
            {
                Name = viewModel.Name,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                IsActive = true
            };
            List<string> access = new List<string>();
            if (viewModel.SelectedControllers != null && viewModel.SelectedControllers.Any())
            {
                foreach (var controller in viewModel.SelectedControllers)
                    foreach (var action in controller.Actions)
                    {
                        action.ControllerId = controller.Id;
                        access.Add("/" + controller.Name + "/" + action.Name);
                        //access += "/" + controller.Name + "/" + action.Name;
                        //access += ",";
                    }

                var accessJson = JsonConvert.SerializeObject(viewModel.SelectedControllers);
                role.Access = JsonConvert.SerializeObject(access);
            }

            HttpClient client = _service.GetService();
            //string stringData = JsonConvert.SerializeObject(role);
            //var contentData = new StringContent(stringData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsJsonAsync("api/role", role).Result;
            ViewBag.Message = response.Content.ReadAsStringAsync().Result;
            Response.StatusCode = (int)response.StatusCode;
            ViewData["Controllers"] = _controllerService.GetControllers();

            return View(viewModel);
        }
    }
}
