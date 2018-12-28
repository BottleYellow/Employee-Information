using EIS.Entities.Address;
using EIS.Entities.Employee;
using EIS.WebApp.IServices;
using EIS.WebApp.Models;
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
using System.Text;
using System.Threading.Tasks;

namespace EIS.WebApp.Controllers
{
    [DisplayName("Employee Management")]
    public class PeopleController : Controller
    {

        #region Declarations
        public readonly IEISService<Person> service;
        public readonly IEISService<Permanent> perService;
        public readonly IEISService<Current> currentService;
        public readonly IEISService<Emergency> emergencyService;
        public readonly IEISService<Designation> designationService;
        private readonly IControllerService _controllerService;
        public static HttpResponseMessage response;
        public static List<Person> data;
        public static List<Designation> DesignationsList;
        static string imageBase64Data;
        #endregion

        #region Controller
        [DisplayName("Employee Management")]
        public PeopleController(IControllerService _controllerService,IEISService<Person> service, IEISService<Permanent> perService, IEISService<Current> currentService, IEISService<Emergency> emergencyService, IEISService<Designation> designationService)
        {
            this._controllerService = _controllerService;
            this.service = service;
            this.perService = perService;
            this.currentService = currentService;
            this.emergencyService = emergencyService;
            this.designationService = designationService;
            HttpResponseMessage response = designationService.GetResponse("api/Employee/Designations");
            string stringData = response.Content.ReadAsStringAsync().Result;
            DesignationsList = JsonConvert.DeserializeObject<List<Designation>>(stringData);
        }
        #endregion

        #region Employee
        [DisplayName("List Of Employees")]
        public IActionResult Index()
        {
            data = EmployeeData();
            Response.StatusCode = (int)response.StatusCode;
          
            return View(data);
        }

        public IActionResult Profile(int PersonId)
        {
            response = service.GetResponse("api/employee/" + PersonId + "");
            string stringData = response.Content.ReadAsStringAsync().Result;
            string permanent = perService.GetResponse("api/PermanentAddress/" + PersonId + "").Content.ReadAsStringAsync().Result;
            string current = currentService.GetResponse("api/CurrentAddress/" + PersonId + "").Content.ReadAsStringAsync().Result;
            string emergency = emergencyService.GetResponse("api/EmergencyAddress/" + PersonId + "").Content.ReadAsStringAsync().Result;
            string other = perService.GetResponse("api/OtherAddress/" + PersonId + "").Content.ReadAsStringAsync().Result;
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
            ViewBag.Designations = DesignationsList;
            var data = from p in EmployeeData()
                    select new Person { Id=p.Id,FirstName = p.FirstName+" "+p.LastName };
            ViewBag.Persons = data;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("IdCard,PanCard,AadharCard,Image,FirstName,MiddleName,LastName,JoinDate,LeavingDate,MobileNumber,DateOfBirth,EmailAddress,Salary,Description,Gender,ReportingPersonId,DesignationId,Id")]Person person, IList<IFormFile> file)
        {
            ViewBag.Designations = DesignationsList;
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
                    HttpResponseMessage response = service.PostResponse("api/employee", person);
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
            string stringData = service.GetResponse("api/employee/" + id + "").Content.ReadAsStringAsync().Result;
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
                    HttpResponseMessage response = service.PutResponse("api/employee/" + id + "", person);
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
            HttpClient client = service.GetService();
            response = client.GetAsync("api/employee/" + id + "").Result;
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
            HttpClient client = service.GetService();
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
            response = service.GetResponse("api/employee");
            string stringData = response.Content.ReadAsStringAsync().Result;
            data = JsonConvert.DeserializeObject<List<Person>>(stringData);

            HttpResponseMessage response2 = service.GetResponse("api/employee/Designations");
            string stringData1 = response2.Content.ReadAsStringAsync().Result;
            var data2 = JsonConvert.DeserializeObject<List<Designation>>(stringData1);

            foreach (var p in data)
            {
                foreach (var d in data2)
                {
                    if (p.DesignationId == d.Id)
                        p.Designation = d;
                }
            }
            return data;
        }

        #endregion

        #region Designations
        [DisplayName("Manage Designations")]
        public IActionResult Designations()
        {
            response = service.GetResponse("api/employee/Designations");
            string stringData = response.Content.ReadAsStringAsync().Result;
            List<Designation> data = JsonConvert.DeserializeObject<List<Designation>>(stringData);
            return View(data);
        }

        [DisplayName("Add Designation")]
        public IActionResult AddDesignation()
        {
            response = service.GetResponse("api/employee/Designations");
            string stringData = response.Content.ReadAsStringAsync().Result;
            List<Designation> data = JsonConvert.DeserializeObject<List<Designation>>(stringData);
            ViewBag.Designations = data;
            ViewData["Controllers"] = _controllerService.GetControllers();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddDesignation(RoleViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Controllers"] = _controllerService.GetControllers();
                return View(viewModel);
            }
            var designation = new Designation
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
                designation.Access = JsonConvert.SerializeObject(UserAccess);
            }
            HttpResponseMessage response = designationService.PostResponse("api/Employee/AddDesignation", designation);
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
        [DisplayName("Update Designation")]
        public ActionResult EditDesignation(int id)
        {
            ViewData["Controllers"] = _controllerService.GetControllers();
            HttpResponseMessage response = service.GetResponse("api/Employee/Designations/" + id + "");
            string stringData = response.Content.ReadAsStringAsync().Result;
            Designation data = JsonConvert.DeserializeObject<Designation>(stringData);
            var access = JsonConvert.DeserializeObject<List<Navigation>>(data.Access);
            var accessList = from p in access
                             select p.URL;
            ViewData["Access"] = accessList.ToList();
            ViewData["RID"] = id;
            var viewModel = new RoleViewModel
            {
                Name = data.Name
            };
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditDesignation(int id, RoleViewModel viewModel)
        {
            HttpClient client = service.GetService();
            if (!ModelState.IsValid)
            {
                ViewData["Controllers"] = _controllerService.GetControllers();
                return View(viewModel);
            }
            string stringData = service.GetResponse("api/Employee/Designations/" + id + "").Content.ReadAsStringAsync().Result;
            Designation designation = JsonConvert.DeserializeObject<Designation>(stringData);
            designation.Name = viewModel.Name;
            designation.UpdatedDate = DateTime.Now.Date;
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
                designation.Access = JsonConvert.SerializeObject(UserAccess);
            }
            if (ModelState.IsValid)
            {
                HttpResponseMessage response = designationService.PutResponse("api/Employee/UpdateDesignation", designation);
                ViewBag.Message = response.Content.ReadAsStringAsync().Result;
                return RedirectToAction("Designations", "People");
            }
            return View(designation);
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
                HttpResponseMessage response = perService.PostResponse("api/PermanentAddress", permanent);
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
                HttpResponseMessage response = perService.PutResponse("api/PermanentAddress", permanent);
                ViewBag.Message = response.Content.ReadAsStringAsync().Result;
            }
            return RedirectToAction("Profile", "People", new { id = pid });
        }
        [DisplayName("Delete Permanent Address")]
        public IActionResult DeletePermanentAddress(int perid)
        {
            HttpResponseMessage response = perService.DeleteResponse("api/PermanentAddress/" + perid + "");
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
                HttpResponseMessage response = currentService.PostResponse("api/CurrentAddress", current);
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
                HttpResponseMessage response = currentService.PutResponse("api/CurrentAddress", current);
                ViewBag.Message = response.Content.ReadAsStringAsync().Result;
            }
            return RedirectToAction("Profile", "People", new { id = pid });
        }
        [DisplayName("Delete Current Address")]
        public IActionResult DeleteCurrentAddress(int cid)
        {
            HttpResponseMessage response = currentService.DeleteResponse("api/CurrentAddress/" + cid + "");
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
                HttpResponseMessage response = emergencyService.PostResponse("api/EmergencyAddress", emergency);
                ViewBag.Message = response.Content.ReadAsStringAsync().Result;
            }
            return RedirectToAction("Profile", "People", new { id = pid });
        }
        [DisplayName("Update Emergency Address")]
        [HttpGet]
        public IActionResult EditEmergencyAddress(int eid)
        {
            HttpResponseMessage response = emergencyService.GetResponse("api/EmergencyAddress/Get/"+eid+"");
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
                HttpResponseMessage response = emergencyService.PutResponse("api/EmergencyAddress", emergency);
                ViewBag.Message = response.Content.ReadAsStringAsync().Result;
            }
            return RedirectToAction("Profile", "People", new { id = pid });
        }
        [DisplayName("Delete Emergency Address")]
        public IActionResult DeleteEmergencyAddress(int eid)
        {
            HttpResponseMessage response = emergencyService.DeleteResponse("api/EmergencyAddress/" + eid + "");
            ViewBag.Message = response.Content.ReadAsStringAsync().Result;
            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}