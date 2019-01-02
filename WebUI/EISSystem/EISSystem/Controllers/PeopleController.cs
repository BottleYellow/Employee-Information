using EIS.Entities.Address;
using EIS.Entities.Employee;
using EIS.WebApp.IServices;
using EIS.WebApp.Models;
using EIS.WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace EIS.WebApp.Controllers
{
    [DisplayName("Employee Management")]
    public class PeopleController : BaseController<Person>
    {

        #region Declarations
        public readonly IServiceWrapper _services;
        private readonly IControllerService _controllerService;
        public static HttpResponseMessage response;
        public static List<Person> data;
        public static List<Role> rolesList;
        static string imageBase64Data;
        RedisAgent Cache = new RedisAgent();
        #endregion

        #region Controller
        [DisplayName("Employee Management")]
<<<<<<< HEAD
        public PeopleController(IControllerService _controllerService,IEISService<Person> service, IEISService<Permanent> perService, IEISService<Current> currentService, IEISService<Emergency> emergencyService, IEISService<Designation> designationService):base(service)
=======
        public PeopleController(IControllerService _controllerService,IServiceWrapper services)
>>>>>>> eab0133b5e8f6e86eb09bb18611280e9b8dcee1c
        {
            this._controllerService = _controllerService;
            _services = services;
            HttpResponseMessage response = _services.Roles.GetResponse("api/Employee/Designations");
            string stringData = response.Content.ReadAsStringAsync().Result;
            rolesList = JsonConvert.DeserializeObject<List<Role>>(stringData);
        }
        #endregion

        #region Employee
        [DisplayName("List Of Employees")]
        public IActionResult Index()
        {          
            return View();
        }

        [HttpPost]
        public IActionResult LoadData()
        {
            return base.LoadData("api/employee/Data");
        }

            public IActionResult Profile(int PersonId)
        {
<<<<<<< HEAD
            PersonId = Convert.ToInt32(Cache.GetStringValue("PersonId"));
            response = service.GetResponse("api/employee/" + PersonId + "");
=======
            response = _services.Employee.GetResponse("api/employee/" + PersonId + "");
>>>>>>> eab0133b5e8f6e86eb09bb18611280e9b8dcee1c
            string stringData = response.Content.ReadAsStringAsync().Result;
            string permanent = _services.PermanentAddress.GetResponse("api/PermanentAddress/" + PersonId + "").Content.ReadAsStringAsync().Result;
            string current = _services.CurrentAddress.GetResponse("api/CurrentAddress/" + PersonId + "").Content.ReadAsStringAsync().Result;
            string emergency = _services.EmergencyAddress.GetResponse("api/EmergencyAddress/" + PersonId + "").Content.ReadAsStringAsync().Result;
            string other = _services.OtherAddress.GetResponse("api/OtherAddress/" + PersonId + "").Content.ReadAsStringAsync().Result;
            Person data = EmployeeData().Find(x => x.Id == PersonId);
            data.PermanentAddress = JsonConvert.DeserializeObject<Permanent>(permanent);
            data.CurrentAddress = JsonConvert.DeserializeObject<Current>(current);
            data.EmergencyAddress= JsonConvert.DeserializeObject<List<Emergency>>(emergency);
            data.OtherAddress = JsonConvert.DeserializeObject<List<Other>>(other);
            if (data != null)
            {
                imageBase64Data = Convert.ToBase64String(data.Image);
                string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
                ViewBag.ImageData = imageDataURL;
                ViewBag.Name = data.FirstName + " " + data.LastName;
            }
            if (data.PermanentAddress == null)
                data.PermanentAddress = new Permanent() { PersonId = PersonId };
            var cur = new Current() { PersonId = PersonId };
            if (data.CurrentAddress == null)
                data.CurrentAddress = cur;
            Response.StatusCode = (int)response.StatusCode;
            return View("Profile", data);
        }
        [DisplayName("Create Employee")]
        public IActionResult Create()
        {
            ViewBag.Designations = rolesList;
            var data = from p in EmployeeData()
                    select new Person { Id=p.Id,FirstName = p.FirstName+" "+p.LastName };
            ViewBag.Persons = data;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("IdCard,PanCard,AadharCard,Image,FirstName,MiddleName,LastName,JoinDate,LeavingDate,MobileNumber,DateOfBirth,EmailAddress,Salary,Description,Gender,ReportingPersonId,DesignationId,Id")]Person person, IList<IFormFile> file)
        {
            ViewBag.Designations = rolesList;
            var data = from p in EmployeeData()
                       select new Person { Id = p.Id, FirstName = p.FirstName + " " + p.LastName };
            ViewBag.Persons = data;
            person.CreatedDate = DateTime.Now.Date;
            IFormFile uploadedImage = file.FirstOrDefault();
            if (uploadedImage == null || uploadedImage.ContentType.ToLower().StartsWith("image/"))
            {
                MemoryStream ms = new MemoryStream();
                uploadedImage.OpenReadStream().CopyTo(ms);

                Image image = Image.FromStream(ms);
                person.Image = ms.ToArray();

                if (ModelState.IsValid)
                {
                    HttpResponseMessage response = _services.Employee.PostResponse("api/employee", person);
                    var data1 = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode == true)
                    {
                        ViewBag.Message = "Record has been successfully saved.";
                        return View("Index", EmployeeData());
                    }
                    
                }
            }
            return View();
        }
        [DisplayName("Update Employee")]
        public IActionResult Edit(int id)
        {
            string stringData = _services.Employee.GetResponse("api/employee/" + id + "").Content.ReadAsStringAsync().Result;
            Person data = EmployeeData().Find(x => x.Id == id);
            imageBase64Data = Convert.ToBase64String(data.Image);
            string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
            ViewBag.ImageData = imageDataURL;
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("IdCard,PanCard,AadharCard,Image,FirstName,MiddleName,LastName,JoinDate,LeavingDate,MobileNumber,DateOfBirth,EmailAddress,Salary,Description,Gender,Designation,Id,CreatedDate,UpdatedDate,IsActive,RowVersion")] Person person)
        {

            if (id != person.Id)
            {
                return NotFound();
            }

            byte[] imageBytes = Convert.FromBase64String(imageBase64Data);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);
            person.Image = ms.ToArray();
            person.UpdatedDate = DateTime.Now.Date;
            if (ModelState.IsValid)
            {
                try
                {
                    HttpResponseMessage response = _services.Employee.PutResponse("api/employee/" + id + "", person);
                    ViewBag.Message = response.Content.ReadAsStringAsync().Result;
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {

                    var entry = ex.Entries.Single();
                    var clientValues = (Person)entry.Entity;
                    var databaseValues = (Person)entry.GetDatabaseValues().ToObject();

                    if (databaseValues.FirstName != clientValues.FirstName)
                        ModelState.AddModelError("First Name", "Current value: "
                            + databaseValues.FirstName);
                    ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                        + "was modified by another user after you got the original value. The "
                        + "edit operation was canceled and the current values in the database "
                        + "have been displayed. If you still want to edit this record, click "
                        + "the Save button again. Otherwise click the Back to List hyperlink.");
                    person.RowVersion = databaseValues.RowVersion;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(person);
        }
        [DisplayName("Delete Employee")]
        public IActionResult Delete(int id)
        {
            response = _services.Employee.GetResponse("api/employee/" + id + "");
            string stringData = response.Content.ReadAsStringAsync().Result;
            Person data1 = JsonConvert.DeserializeObject<Person>(stringData);
            imageBase64Data = Convert.ToBase64String(data1.Image);
            string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
            ViewBag.ImageData = imageDataURL;
            return PartialView("DeletePartial",data1);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            HttpClient client = _services.Employee.GetService();
            response = client.DeleteAsync("api/employee/" + id + "").Result;
            if (response.IsSuccessStatusCode == true)
            {
                ViewBag.Message = "Record has been successfully deleted.";
                data = EmployeeData();
                return View("Index",data);
            }
            return View();
        }
        [AllowAnonymous]
        public List<Person> EmployeeData()
        {
            response = _services.Employee.GetResponse("api/employee");
            string stringData = response.Content.ReadAsStringAsync().Result;
            data = JsonConvert.DeserializeObject<List<Person>>(stringData);

            HttpResponseMessage response2 = _services.Roles.GetResponse("api/employee/Designations");
            string stringData1 = response2.Content.ReadAsStringAsync().Result;
            var data2 = JsonConvert.DeserializeObject<List<Role>>(stringData1);

            foreach (var p in data)
            {
                foreach (var d in data2)
                {
                    if (p.RoleId == d.Id)
                        p.Role = d;
                }
            }
            return data;
        }

        #endregion

        #region Roles
        [DisplayName("Manage Roles")]
        public IActionResult Roles()
        {
            response = _services.Roles.GetResponse("api/employee/Designations");
            string stringData = response.Content.ReadAsStringAsync().Result;
            List<Role> data = JsonConvert.DeserializeObject<List<Role>>(stringData);
            return View(data);
        }

        [DisplayName("Add Role")]
        public IActionResult AddRole()
        {
            response = _services.Roles.GetResponse("api/employee/Designations");
            string stringData = response.Content.ReadAsStringAsync().Result;
            List<Role> data = JsonConvert.DeserializeObject<List<Role>>(stringData);
            ViewBag.Designations = data;
            ViewData["Controllers"] = _controllerService.GetControllers();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddRole(RoleViewModel viewModel)
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
            List<Navigation> UserAccess = new List<Navigation>();
            if (viewModel.SelectedControllers != null && viewModel.SelectedControllers.Any())
            {
                int n = 1;
                int m = 0;
                foreach (var controller in viewModel.SelectedControllers)
                {
                    UserAccess.Add(
                        new Navigation()
                        {
                            Id = n,
                            Name = controller.DisplayName,
                            URL = "*"
                        });
                    m = n;
                    foreach (var action in controller.Actions)
                    {
                        action.ControllerId = controller.Id;
                        access.Add("/" + controller.Name + "/" + action.Name);
                        n++;
                        UserAccess.Add(
                            new Navigation()
                            {
                                ParentId = m,
                                Id = n,
                                Name = action.DisplayName,
                                URL = "/" + controller.Name + "/" + action.Name
                            });
                    }
                    n++;
                }

                var accessJson = JsonConvert.SerializeObject(viewModel.SelectedControllers);
                //role.Access = JsonConvert.SerializeObject(access);
                role.Access = JsonConvert.SerializeObject(UserAccess);
            }
            HttpResponseMessage response = _services.Roles.PostResponse("api/Employee/AddDesignation", role);
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var Message = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
                ViewData["Controllers"] = _controllerService.GetControllers();
                ModelState.AddModelError("Name", Message.ToString());
                return View(viewModel);
            }
            
            Response.StatusCode = (int)response.StatusCode;
            ViewData["Controllers"] = _controllerService.GetControllers();
            return RedirectToAction("Designations", "People");
        }
        [DisplayName("Update Role")]
        public ActionResult EditRole(int id)
        {
            ViewData["Controllers"] = _controllerService.GetControllers();
            HttpResponseMessage response = _services.Roles.GetResponse("api/Employee/Designations/" + id + "");
            string stringData = response.Content.ReadAsStringAsync().Result;
            Role role = JsonConvert.DeserializeObject<Role>(stringData);
            var access = JsonConvert.DeserializeObject<List<Navigation>>(role.Access);
            var accessList = from p in access
                             select p.URL;
            ViewData["Access"] = accessList.ToList();
            ViewData["RID"] = id;
            var viewModel = new RoleViewModel
            {
                Name = role.Name
            };
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditRole(int id, RoleViewModel viewModel)
        {
            HttpClient client = _services.Roles.GetService();
            if (!ModelState.IsValid)
            {
                ViewData["Controllers"] = _controllerService.GetControllers();
                return View(viewModel);
            }
            string stringData = _services.Roles.GetResponse("api/Employee/Designations/" + id + "").Content.ReadAsStringAsync().Result;
            Role role = JsonConvert.DeserializeObject<Role>(stringData);
            role.Name = viewModel.Name;
            role.UpdatedDate = DateTime.Now.Date;
            List<string> access = new List<string>();
            List<Navigation> UserAccess = new List<Navigation>();
            if (viewModel.SelectedControllers != null && viewModel.SelectedControllers.Any())
            {
                int n = 1;
                int m = 0;
                foreach (var controller in viewModel.SelectedControllers)
                {
                    UserAccess.Add(
                        new Navigation()
                        {
                            Id = n,
                            Name = controller.DisplayName,
                            URL = "*"
                        });
                    m = n;
                    foreach (var action in controller.Actions)
                    {
                        action.ControllerId = controller.Id;
                        access.Add("/" + controller.Name + "/" + action.Name);
                        n++;
                        UserAccess.Add(
                            new Navigation()
                            {
                                ParentId = m,
                                Id = n,
                                Name = action.DisplayName,
                                URL = "/" + controller.Name + "/" + action.Name
                            });
                    }
                    n++;
                }

                var accessJson = JsonConvert.SerializeObject(viewModel.SelectedControllers);
                //role.Access = JsonConvert.SerializeObject(access);
                role.Access = JsonConvert.SerializeObject(UserAccess);
            }
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = _services.Roles.PutResponse("api/Employee/UpdateDesignation", role);
                ViewBag.Message = response.Content.ReadAsStringAsync().Result;
                return RedirectToAction("Roles", "People");
            }
            return View(role);
        }
        #endregion

        #region Permanent Address
        [DisplayName("Add Permanent Address")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreatePermanentAddress(int pid, [Bind("Address,City,State,Country,PinCode,PhoneNumber")]Permanent permanent)
        {

            permanent.PersonId = pid;
            permanent.CreatedDate = DateTime.Now.Date;
            permanent.UpdatedDate = DateTime.Now.Date;
            permanent.IsActive = true;
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = _services.PermanentAddress.PostResponse("api/PermanentAddress", permanent);
                ViewBag.Message = response.Content.ReadAsStringAsync().Result;
                return RedirectToAction("Profile", "People", new { id = pid });
            }
            return View();          
        }
        [DisplayName("Update Permanent Address")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditPermanentAddress(int pid, Permanent permanent)
        {
            permanent.UpdatedDate = DateTime.Now.Date;
            permanent.IsActive = true;
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = _services.PermanentAddress.PutResponse("api/PermanentAddress", permanent);
                ViewBag.Message = response.Content.ReadAsStringAsync().Result;
            }
            return RedirectToAction("Profile", "People", new { id = pid });
        }
        [DisplayName("Delete Permanent Address")]
        public IActionResult DeletePermanentAddress(int perid)
        {
            HttpResponseMessage response = _services.PermanentAddress.DeleteResponse("api/PermanentAddress/" + perid + "");
            ViewBag.Message = response.Content.ReadAsStringAsync().Result;
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Current Address
        [DisplayName("Add Current Address")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateCurrentAddress(int pid, [Bind("Address,City,State,Country,PinCode,PhoneNumber")]Current current)
        {

            current.PersonId = pid;
            current.CreatedDate = DateTime.Now.Date;
            current.UpdatedDate = DateTime.Now.Date;
            current.IsActive = true;
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = _services.CurrentAddress.PostResponse("api/CurrentAddress", current);
                ViewBag.Message = response.Content.ReadAsStringAsync().Result;
            }
            return RedirectToAction("Profile", "People", new { id = pid });
        }
        [DisplayName("Update Current Address")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCurrentAddress(int pid, Current current)
        {
            current.UpdatedDate = DateTime.Now.Date;
            current.IsActive = true;
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = _services.CurrentAddress.PutResponse("api/CurrentAddress", current);
                ViewBag.Message = response.Content.ReadAsStringAsync().Result;
            }
            return RedirectToAction("Profile", "People", new { id = pid });
        }
        [DisplayName("Delete Current Address")]
        public IActionResult DeleteCurrentAddress(int cid)
        {
            HttpResponseMessage response = _services.CurrentAddress.DeleteResponse("api/CurrentAddress/" + cid + "");
            ViewBag.Message = response.Content.ReadAsStringAsync().Result;
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Emergency Address
        [DisplayName("Add Emergency Address")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateEmergencyAddress(int pid, [Bind("FirstName,LastName,Address,City,State,Country,PinCode,PhoneNumber,MobileNumber,Relation")]Emergency emergency)
        {

            emergency.PersonId = pid;
            emergency.CreatedDate = DateTime.Now.Date;
            emergency.UpdatedDate = DateTime.Now.Date;
            emergency.IsActive = true;
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = _services.EmergencyAddress.PostResponse("api/EmergencyAddress", emergency);
                ViewBag.Message = response.Content.ReadAsStringAsync().Result;
            }
            return RedirectToAction("Profile", "People", new { id = pid });
        }
        [DisplayName("Update Emergency Address")]
        [HttpGet]
        public IActionResult EditEmergencyAddress(int eid)
        {
            HttpResponseMessage response = _services.EmergencyAddress.GetResponse("api/EmergencyAddress/Get/"+eid+"");
            ViewBag.Message = response.Content.ReadAsStringAsync().Result;
            string emergency = response.Content.ReadAsStringAsync().Result;
            var data = JsonConvert.DeserializeObject<Emergency>(emergency);
            return PartialView("EditEmergencyAddress", data);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditEmergencyAddress(int pid, Emergency emergency)
        {
            emergency.UpdatedDate = DateTime.Now.Date;
            emergency.IsActive = true;
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = _services.EmergencyAddress.PutResponse("api/EmergencyAddress", emergency);
                ViewBag.Message = response.Content.ReadAsStringAsync().Result;
            }
            return RedirectToAction("Profile", "People", new { id = pid });
        }
        [DisplayName("Delete Emergency Address")]
        public IActionResult DeleteEmergencyAddress(int eid)
        {
            HttpResponseMessage response = _services.EmergencyAddress.DeleteResponse("api/EmergencyAddress/" + eid + "");
            ViewBag.Message = response.Content.ReadAsStringAsync().Result;
            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}