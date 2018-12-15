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
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EIS.WebApp.Controllers
{
    public class RoleController : Controller
    {
        private readonly IControllerService _controllerService;
        private readonly IEISService<Role> _service;
        public RoleController(IControllerService controllerService, IEISService<Role> service)
        {
            _service = service;
            _controllerService = controllerService;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            HttpResponseMessage response = _service.GetResponse("api/role");
            string stringData = response.Content.ReadAsStringAsync().Result;
            List<Role> data = JsonConvert.DeserializeObject<List<Role>>(stringData);
            Response.StatusCode = (int)response.StatusCode;
            return View(data);
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
                    }

                var accessJson = JsonConvert.SerializeObject(viewModel.SelectedControllers);
                role.Access = JsonConvert.SerializeObject(access);
            }
            HttpResponseMessage response = _service.PostResponse("api/role", role);
            ViewBag.Message = response.Content.ReadAsStringAsync().Result;
            Response.StatusCode = (int)response.StatusCode;
            ViewData["Controllers"] = _controllerService.GetControllers();
            return View(viewModel);
        }
        public ActionResult Edit(int id)
        {
            ViewData["Controllers"] = _controllerService.GetControllers();
            HttpResponseMessage response = _service.GetResponse("api/role/" + id + "");
            string stringData = response.Content.ReadAsStringAsync().Result;
            Role data = JsonConvert.DeserializeObject<Role>(stringData);
            var accessList= JsonConvert.DeserializeObject<List<string>>(data.Access);
            ViewData["Access"] = accessList;
            ViewData["RID"] = id;
            var viewModel = new RoleViewModel
            {
                Name = data.Name
            };
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, RoleViewModel viewModel)
        {
            HttpClient client = _service.GetService();
            if (!ModelState.IsValid)
            {
                ViewData["Controllers"] = _controllerService.GetControllers();
                return View(viewModel);
            }
            string stringData = _service.GetResponse("api/role/" + id + "").Content.ReadAsStringAsync().Result;
            Role role = JsonConvert.DeserializeObject<Role>(stringData);
            role.Name = viewModel.Name;
            role.UpdatedDate = DateTime.Now.Date;
            List<string> access = new List<string>();
            if (viewModel.SelectedControllers != null && viewModel.SelectedControllers.Any())
            {
                foreach (var controller in viewModel.SelectedControllers)
                    foreach (var action in controller.Actions)
                    {
                        access.Add("/" + controller.Name + "/" + action.Name);
                    }
                role.Access = JsonConvert.SerializeObject(access);
            }
            if (ModelState.IsValid)
            {
                try
                {                  
                    HttpResponseMessage response = _service.PutResponse("api/role/" + id + "", role);
                    ViewBag.Message = response.Content.ReadAsStringAsync().Result;
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {

                    var entry = ex.Entries.Single();
                    var clientValues = (Role)entry.Entity;
                    var databaseValues = (Role)entry.GetDatabaseValues().ToObject();

                    if (databaseValues.Name != clientValues.Name)
                        ModelState.AddModelError("First Name", "Current value: "
                            + databaseValues.Name);
                    ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                        + "was modified by another user after you got the original value. The "
                        + "edit operation was canceled and the current values in the database "
                        + "have been displayed. If you still want to edit this record, click "
                        + "the Save button again. Otherwise click the Back to List hyperlink.");
                    role.RowVersion = databaseValues.RowVersion;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(role);
        }
    }
}
