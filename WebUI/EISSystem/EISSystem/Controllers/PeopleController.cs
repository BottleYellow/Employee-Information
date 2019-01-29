using EIS.Entities.Address;
using EIS.Entities.Employee;
using EIS.WebApp.IServices;
using EIS.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace EIS.WebApp.Controllers
{
    [DisplayName("Employee Management")]
    public class PeopleController : BaseController<Person>
    {
        private readonly IHostingEnvironment _environment;
        #region Declarations
        public readonly IServiceWrapper _services;
        private readonly IControllerService _controllerService;
        public static HttpResponseMessage response;
        public static List<Person> data;
        public static List<Role> rolesList;

        #endregion 

        #region Controller
        [DisplayName("Employee Management")]
        public PeopleController(IEISService<Person> service, IControllerService controllerService, IServiceWrapper services, IHostingEnvironment environment) : base(service)
        {
            _environment = environment;
            _controllerService = controllerService;
            _services = services;
            HttpResponseMessage response = _services.Roles.GetResponse("api/Employee/Designations" );
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
        public IActionResult LoadData(bool type)
        {
            return LoadData<Person>("api/employee/data",type);

        }
        public IActionResult Profile(string PersonId)
        {
            response = _services.Employee.GetResponse("api/employee/Profile/" + PersonId + "" );
            string stringData = response.Content.ReadAsStringAsync().Result;
            Person data = JsonConvert.DeserializeObject<Person>(stringData);
            ViewBag.ImagePath = data.Image;
             ViewBag.Name = data.FullName ;
            Attendance attendance = data.Attendance.Where(x => x.DateIn.Date == DateTime.Now.Date).FirstOrDefault();
            ViewBag.TimeIn = (attendance == null) ? new TimeSpan(0, 0, 0) : attendance.TimeIn;
            ViewBag.TimeOut = (attendance == null) ? new TimeSpan(0, 0, 0) : attendance.TimeOut;
            ViewBag.TotalHrs = (attendance == null) ? new TimeSpan(0, 0, 0) : attendance.TotalHours;
            if (attendance == null)
            {
                ViewBag.EstimatedTimeOut = new TimeSpan(0, 0, 0);
            }
            else
            {
                var estTimeOut = attendance.TimeIn + new TimeSpan(9, 0, 0);
                ViewBag.EstimatedTimeOut = estTimeOut;
            }
            
            if (data.PermanentAddress == null)
                data.PermanentAddress = new Permanent() { PersonId = data.Id };
            var cur = new Current() { PersonId = data.Id };
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
                       where p.Role.Name != "Employee"
                       select new Person { Id = p.Id, FirstName = p.FirstName + " " + p.LastName + " (" + p.Role.Name + ")" };
            ViewBag.Persons = data;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeCode,PanCard,AadharCard,FirstName,MiddleName,LastName,JoinDate,LeavingDate,MobileNumber,DateOfBirth,EmailAddress,Salary,Description,Gender,ReportingPersonId,RoleId,Id")]Person person, IFormFile file)
        {
            var tId = Cache.GetStringValue("TenantId");
            ViewBag.Designations = rolesList;

            var data = from p in EmployeeData()
                       where p.Role.Name != "Employee"
                       select new Person { Id = p.Id, FirstName = p.FirstName + " " + p.LastName + " (" + p.Role.Name + ")" };
            ViewBag.Persons = data;
           
            if (ModelState.IsValid)
            {
               
                var rootPath = _environment.WebRootPath;
                var filePath = "//EmployeeData//" + tId+  person.EmployeeCode + "//Image//";
                if (!Directory.Exists(rootPath + "//EmployeeData//"))
                {
                    Directory.CreateDirectory(rootPath + "//EmployeeData//");
                }
                if (!Directory.Exists(rootPath + filePath))
                {
                    Directory.CreateDirectory(rootPath + filePath);
                }
                var uploadPath = rootPath + filePath;
                if (file != null && file.Length > 0)
                {
                    var fileExtension = Path.GetExtension(file.FileName).ToLower();
                    
                    if ((fileExtension == ".gif" || fileExtension == ".png" || fileExtension == ".bmp" || fileExtension == ".jpeg" || fileExtension == ".jpg") && file.Length <= 500000)
                    {                                          
                        var fileName = person.FirstName+ "_" + Guid.NewGuid().ToString().Substring(0, 4) + fileExtension;
                        using (var fileStream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);                            
                            person.Image = fileName;
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Image", "Please select Image of type GIF, PNG, JPG, JPEG and BMP and size must be below 500 kb");
                    }
                }
                else
                {
                    System.IO.File.Copy(rootPath+"//EmployeeData//DefaultImage//Default.png",uploadPath+"Default.png");
                    person.Image = "Default.png";

                }
                if(string.IsNullOrEmpty(person.MiddleName))
                {
                    person.MiddleName = "";
                }

                person.CreatedDate = DateTime.Now.Date;
                HttpResponseMessage response = _services.Employee.PostResponse("api/employee", person );
                var data1 = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode == true)
                {
                    ViewBag.Message = "Record has been successfully saved.";
                    return View("Index");
                }
            }
            if (file != null)
            {
                ModelState.AddModelError("Image", "Please upload the file");
                ViewBag.ImageExist = "Validation failed... Please Upload the file again";
            }

            return View(person);
        }

        [DisplayName("Update Employee")]
        public IActionResult Edit(string EmployeeCode)
        {
            ViewBag.Designations = rolesList;

            var data1 = from p in EmployeeData()
                       select new Person { Id = p.Id, FirstName = p.FirstName + " " + p.LastName };
            ViewBag.Persons = data1;
            string stringData = _services.Employee.GetResponse("api/employee/Person/" + EmployeeCode + "" ).Content.ReadAsStringAsync().Result;
            Person data = EmployeeData().Find(x => x.EmployeeCode == EmployeeCode);         
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Person person, IFormFile file)
        {
            var tId = Cache.GetStringValue("TenantId");
            if (id != person.Id)
            {
                return NotFound();
            }
            ViewBag.Designations = rolesList;
            var data1 = from p in EmployeeData()
                        select new Person { Id = p.Id, FirstName = p.FirstName + " " + p.LastName };
            ViewBag.Persons = data1;
            person.UpdatedDate = DateTime.Now.Date;
            if (ModelState.IsValid)
            {
                try
                {
                    var rootPath = _environment.WebRootPath;
                    var filePath = "//EmployeeData//" + tId + person.EmployeeCode + "//Image//";
                    var uploadPath = rootPath + filePath;
                    if (file != null && file.Length > 0)
                    {
                        var fileExtension = Path.GetExtension(file.FileName).ToLower();
                        if ((fileExtension == ".gif" || fileExtension == ".png" || fileExtension == ".bmp" || fileExtension == ".jpeg" || fileExtension == ".jpg") && file.Length <= 500000)
                        {
                            string[] files = Directory.GetFiles(rootPath + filePath);                            
                            System.IO.File.Delete(files[0]);                            
                            var fileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(file.FileName);
                            using (var fileStream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create))
                            {
                               await file.CopyToAsync(fileStream);
                                person.Image = fileName;
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(person.MiddleName))
                    {
                        person.MiddleName = "";
                    }
                    person.IsActive = true;
                    HttpResponseMessage response = _services.Employee.PutResponse("api/employee/" + id + "", person );
                    ViewBag.Message = response.Content.ReadAsStringAsync().Result;
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
                return RedirectToAction("Profile","People", new { PersonId = person.EmployeeCode });
            }
            return View(person);
        }
        [AllowAnonymous]
        public List<Person> EmployeeData()
        {
            response = _services.Employee.GetResponse("api/employee" );
            string stringData = response.Content.ReadAsStringAsync().Result;
            data = JsonConvert.DeserializeObject<List<Person>>(stringData);

            HttpResponseMessage response2 = _services.Roles.GetResponse("api/employee/Designations" );
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

        public void DeleteConfirmed(int id)
        {
            HttpClient client = _services.Employee.GetService();
            response = client.DeleteAsync("api/employee/" + id + "").Result;
        }
        public IActionResult ActivateEmployee(string EmployeeCode)
        {
            response = _services.Employee.GetResponse("api/employee/ActivatePerson/" + EmployeeCode + "" );
            if(response.IsSuccessStatusCode)
                ViewBag.Message = "Information activated successfully!";
            return View("Index");
        }
        #endregion

        #region Roles
        [DisplayName("Manage Roles")]
        public IActionResult Roles()
        {
            response = _services.Roles.GetResponse("api/employee/Designations" );
            string stringData = response.Content.ReadAsStringAsync().Result;
            List<Role> data = JsonConvert.DeserializeObject<List<Role>>(stringData);
            return View(data);
        }

        [DisplayName("Add Role")]
        public IActionResult AddRole()
        {
            response = _services.Roles.GetResponse("api/employee/Designations" );
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
            HttpResponseMessage response = _services.Roles.PostResponse("api/Employee/AddDesignation", role );
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var Message = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
                ViewData["Controllers"] = _controllerService.GetControllers();
                ModelState.AddModelError("Name", Message.ToString());
                return View(viewModel);
            }

            Response.StatusCode = (int)response.StatusCode;
            ViewData["Controllers"] = _controllerService.GetControllers();
            return RedirectToAction("Roles", "People");
        }
        [DisplayName("Update Role")]
        public ActionResult EditRole(int id)
        {
            ViewData["Controllers"] = _controllerService.GetControllers();
            HttpResponseMessage response = _services.Roles.GetResponse("api/Employee/Designations/" + id + "" );
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
            string stringData = _services.Roles.GetResponse("api/Employee/Designations/" + id + "" ).Content.ReadAsStringAsync().Result;
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
                HttpResponseMessage response = _services.Roles.PutResponse("api/Employee/UpdateDesignation", role );
                if (Cache.GetStringValue("Role") == role.Name)
                {
                    Cache.DeleteStringValue("Access");
                    Cache.SetStringValue("Access", role.Access);
                }
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
                HttpResponseMessage response = _services.PermanentAddress.PostResponse("api/PermanentAddress", permanent );
                ViewBag.Message = response.Content.ReadAsStringAsync().Result;
                return RedirectToAction("Profile", "People", new { PersonId = pid });
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
                HttpResponseMessage response = _services.PermanentAddress.PutResponse("api/PermanentAddress", permanent );
                ViewBag.Message = response.Content.ReadAsStringAsync().Result;
            }
            return RedirectToAction("Profile", "People", new { PersonId = pid });
        }
        [DisplayName("Delete Permanent Address")]
        public IActionResult DeletePermanentAddress(int perid)
        {
            HttpResponseMessage response = _services.PermanentAddress.DeleteResponse("api/PermanentAddress/" + perid + "" );
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
                HttpResponseMessage response = _services.CurrentAddress.PostResponse("api/CurrentAddress", current );
                ViewBag.Message = response.Content.ReadAsStringAsync().Result;
            }
            return RedirectToAction("Profile", "People", new { PersonId = pid });
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
                HttpResponseMessage response = _services.CurrentAddress.PutResponse("api/CurrentAddress", current );
                ViewBag.Message = response.Content.ReadAsStringAsync().Result;
            }
            return RedirectToAction("Profile", "People", new { PersonId = pid });
        }
        [DisplayName("Delete Current Address")]
        public IActionResult DeleteCurrentAddress(int cid)
        {
            HttpResponseMessage response = _services.CurrentAddress.DeleteResponse("api/CurrentAddress/" + cid + "" );
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
                HttpResponseMessage response = _services.EmergencyAddress.PostResponse("api/EmergencyAddress", emergency );
                ViewBag.Message = response.Content.ReadAsStringAsync().Result;
            }
            return RedirectToAction("Profile", "People", new { PersonId = pid });
        }
        [DisplayName("Update Emergency Address")]
        [HttpGet]
        public IActionResult EditEmergencyAddress(int eid)
        {
            HttpResponseMessage response = _services.EmergencyAddress.GetResponse("api/EmergencyAddress/Get/" + eid + "" );
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
                HttpResponseMessage response = _services.EmergencyAddress.PutResponse("api/EmergencyAddress", emergency );
                ViewBag.Message = response.Content.ReadAsStringAsync().Result;
            }
            return RedirectToAction("Profile", "People", new { PersonId = pid });
        }
        [DisplayName("Delete Emergency Address")]
        public IActionResult DeleteEmergencyAddress(int eid)
        {
            HttpResponseMessage response = _services.EmergencyAddress.DeleteResponse("api/EmergencyAddress/" + eid + "" );
            ViewBag.Message = response.Content.ReadAsStringAsync().Result;
            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}