using EIS.Entities.Address;
using EIS.Entities.Employee;
using EIS.Entities.OtherEntities;
using EIS.Entities.SP;
using EIS.WebApp.Filters;
using EIS.WebApp.IServices;
using EIS.WebApp.Models;
using EIS.WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace EIS.WebApp.Controllers
{
    [SessionTimeOut]
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
        public MyHttpContext httpContext;

        #endregion 

        #region Controller
        [DisplayName("Employee Management")]
        public PeopleController(IEISService<Person> service, IControllerService controllerService, IServiceWrapper services, IHostingEnvironment environment) : base(service)
        {
            _environment = environment;
            _controllerService = controllerService;
            _services = services;
            HttpResponseMessage response = _services.Roles.GetResponse(ApiUrl+"/api/Employee/Designations" );
            string stringData = response.Content.ReadAsStringAsync().Result;
            rolesList = JsonConvert.DeserializeObject<List<Role>>(stringData);
        }
        #endregion

        #region Employee
        [DisplayName("List Of Employees")]
        public IActionResult Index()
        {
            ViewBag.Locations = GetLocations();
            return View();
        }
        [HttpPost]
        public IActionResult Index(bool type, int LocationId)
        {
            HttpResponseMessage response = _services.SP_GetEmployee.GetResponse(ApiUrl + "/api/employee/Data/"+type+"/"+LocationId);
            //return LoadData<Person>(ApiUrl + "/api/employee/data", type, LocationId);
            string stringData = response.Content.ReadAsStringAsync().Result;
            List<SP_GetEmployee> sP_GetEmployee = JsonConvert.DeserializeObject<List<SP_GetEmployee>>(stringData);
            return Json(sP_GetEmployee);

        }
        public IActionResult Profile(string PersonId)
        {
            response = _services.Employee.GetResponse(ApiUrl+"/api/employee/Profile/" + PersonId + "" );
            string stringData = response.Content.ReadAsStringAsync().Result;
            Person data = JsonConvert.DeserializeObject<Person>(stringData);
            Attendance attendance = data.Attendance.Where(x => x.DateIn.Date == DateTime.Now.Date).FirstOrDefault();
            if (attendance != null)
            {
                ViewBag.TimeIn = attendance.TimeIn;
                ViewBag.TimeOut = attendance.TimeOut.GetValueOrDefault();
                ViewBag.TotalHrs = attendance.TotalHours.GetValueOrDefault();
                ViewBag.EstimatedTimeOut = attendance.TimeIn + new TimeSpan(9, 0, 0);
            }
            else
            {
                ViewBag.TimeIn = new TimeSpan();
                ViewBag.TimeOut = new TimeSpan();
                ViewBag.TotalHrs = new TimeSpan();
                ViewBag.EstimatedTimeOut = new TimeSpan();
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
            ViewBag.Locations = GetLocations();
            ViewBag.WeeklyOffs = GetWeeklyOffs();
            ViewBag.Designations = rolesList;
            var data = from p in EmployeeData()
                       where p.Role.Name != "Employee"
                       select new Person { Id = p.Id, FirstName = p.FirstName + " " + p.LastName + " (" + p.Role.Name + ")" };
            ViewBag.Persons = data;
            ViewBag.Dob = DateTime.Now.ToShortDateString();
            return View("CreateEmployee");
        }
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEmployee createEmployee, IFormFile file)
        {
            if (createEmployee.people.DateOfBirth != null)
            {
                var d = DateTime.Parse(createEmployee.people.DateOfBirth.ToString());
                createEmployee.people.DateOfBirth = d;
            }
            if (createEmployee.people.RoleId == 0) ModelState.AddModelError("RoleId", "Please Select Role");
            if (createEmployee.people.LocationId == 0) ModelState.AddModelError("LocationId", "Please Select Location");
            if (createEmployee.people.WeeklyOffId == 0) ModelState.AddModelError("WeeklyOffId", "Please Select Weekly Off Type");
            var tId = GetSession().TenantId;
            ViewBag.Locations = GetLocations();
            ViewBag.WeeklyOffs = GetWeeklyOffs();
            //var tId = Cache.GetStringValue("TenantId");
            ViewBag.Designations = rolesList;

            var data = from p in EmployeeData()
                       where p.Role.Name != "Employee"
                       select new Person { Id = p.Id, FirstName = p.FirstName + " " + p.LastName + " (" + p.Role.Name + ")" };
            ViewBag.Persons = data;

            if (ModelState.IsValid)
            {
                createEmployee.people.CreatedBy = Convert.ToInt32(GetSession().PersonId);
                string rootPath = _environment.WebRootPath;
                string filePath = "//EmployeeData//" + tId + createEmployee.people.EmployeeCode + "//Image//";
                if (!Directory.Exists(rootPath + "//EmployeeData//"))
                {
                    Directory.CreateDirectory(rootPath + "//EmployeeData//");
                }
                if (!Directory.Exists(rootPath + filePath))
                {
                    Directory.CreateDirectory(rootPath + filePath);
                }
                else
                {
                    string[] files = Directory.GetFiles(rootPath + filePath);
                    if (files.Length > 0)
                    {
                        for (int i = 0; i < files.Length; i++)
                        {
                            System.IO.File.Delete(files[i]);
                        }
                    }
                }
                string uploadPath = rootPath + filePath;
                if (file != null && file.Length > 0)
                {
                    var fileExtension = Path.GetExtension(file.FileName).ToLower();

                    if ((fileExtension == ".gif" || fileExtension == ".png" || fileExtension == ".bmp" || fileExtension == ".jpeg" || fileExtension == ".jpg") && file.Length <= 500000)
                    {

                        var fileName = createEmployee.people.FirstName + "_" + Guid.NewGuid().ToString().Substring(0, 4) + fileExtension;
                        using (var fileStream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                            createEmployee.people.Image = fileName;
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Image", "Please select Image of type GIF, PNG, JPG, JPEG and BMP and size must be below 500 kb");
                    }
                }
                else
                {
                    System.IO.File.Copy(rootPath + "//EmployeeData//DefaultImage//Default.png", uploadPath + "Default.png");
                    createEmployee.people.Image = "Default.png";

                }
                if (string.IsNullOrEmpty(createEmployee.people.MiddleName))
                {
                    createEmployee.people.MiddleName = "";
                }

                createEmployee.people.CreatedDate = DateTime.Now.Date;
                HttpResponseMessage response = _services.Employee.PostResponse(ApiUrl + "/api/employee/" + 0, createEmployee.people);
                string data1 = response.Content.ReadAsStringAsync().Result;
                Person pdata = JsonConvert.DeserializeObject<Person>(data1);
                CreatePermanentAddress(pdata.Id,pdata.EmployeeCode,createEmployee.permanent);
                CreateCurrentAddress(pdata.Id, pdata.EmployeeCode, createEmployee.current);
                CreateEmergencyAddress(pdata.Id, pdata.EmployeeCode, createEmployee.emergencyAddressFirst);
                CreateEmergencyAddress(pdata.Id, pdata.EmployeeCode, createEmployee.emergencyAddressSecond);
                if (response.IsSuccessStatusCode == true)
                {
                    ViewBag.Message = "Record has been successfully saved.";
                    TempData["success"] = "Record created successfully.";
                    return RedirectToAction("Index");
                }
            }
            if (file != null)
            {
                ModelState.AddModelError("Image", "Please upload the file");
                ViewBag.ImageExist = "Validation failed... Please Upload the file again";
            }
            
            return View("CreateEmployee");
        }

        [DisplayName("Update Employee")]
        public IActionResult Edit(string EmployeeCode)
        {
            ViewBag.EmployeeCode = EmployeeCode;
            ViewBag.Designations = rolesList;
            ViewBag.WeeklyOffs = GetWeeklyOffs();
            ViewBag.Locations = GetLocations();
            var data1 = from p in EmployeeData()
                        where p.EmployeeCode!=EmployeeCode && p.Role.Name!="Employee"
                        select new Person { Id = p.Id, FirstName = p.FirstName + " " + p.LastName + " (" + p.Role.Name + ")" };
            ViewBag.Persons = data1;
            string stringData = _services.Employee.GetResponse(ApiUrl+"/api/employee/Profile/" + EmployeeCode + "" ).Content.ReadAsStringAsync().Result;
            var data = JsonConvert.DeserializeObject<Person>(stringData);
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Person person, IFormFile file)
        {
            var tId = GetSession().TenantId;
            if (id != person.Id)
            {
                return NotFound();
            }
            ViewBag.WeeklyOffs = GetWeeklyOffs();
            ViewBag.Designations = rolesList;
            ViewBag.Locations = GetLocations();
            var data1 = from p in EmployeeData()
                        where p.EmployeeCode != person.EmployeeCode && p.Role.Name != "Employee"
                        select new Person { Id = p.Id, FirstName = p.FirstName + " " + p.LastName + " (" + p.Role.Name + ")" };
            ViewBag.Persons = data1;
            person.UpdatedDate = DateTime.Now.Date;
            if (person.RoleId == 0) ModelState.AddModelError("RoleId", "Please Select Role");
            if (person.LocationId == 0) ModelState.AddModelError("LocationId", "Please Select Location");
            if (ModelState.IsValid)
            {
                try
                {
                    person.UpdatedBy = Convert.ToInt32(GetSession().PersonId);
                    person.UpdatedDate = DateTime.Now;
                    var rootPath = _environment.WebRootPath;
                    var filePath = "//EmployeeData//" + tId + person.EmployeeCode + "//Image//";
                    if (!Directory.Exists(rootPath + "//EmployeeData//"))
                    {
                        Directory.CreateDirectory(rootPath + "//EmployeeData//");
                    }
                    if (!Directory.Exists(rootPath + filePath))
                    {
                        Directory.CreateDirectory(rootPath + filePath);
                    }
                    else
                    {
                        string[] files = Directory.GetFiles(rootPath + filePath);
                        if (files.Length > 0)
                        {
                            for (int i = 0; i < files.Length; i++)
                            {
                                System.IO.File.Delete(files[i]);
                            }
                        }
                    }
                    var uploadPath = rootPath + filePath;
                    if (file != null && file.Length > 0)
                    {
                        var fileExtension = Path.GetExtension(file.FileName).ToLower();
                        if ((fileExtension == ".gif" || fileExtension == ".png" || fileExtension == ".bmp" || fileExtension == ".jpeg" || fileExtension == ".jpg") && file.Length <= 500000)
                        {
                            var fileName = person.FirstName + "_" + Guid.NewGuid().ToString().Substring(0, 4) + fileExtension;
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
                    HttpResponseMessage response = _services.Employee.PostResponse(ApiUrl + "/api/employee/" + id, person);
                    if(response.IsSuccessStatusCode) TempData["success"] = "Record updated successfully.";
                    if (response.IsSuccessStatusCode && id == Convert.ToInt32(GetSession().PersonId))
                    {
                        HttpContext.Session.Remove("ImagePath");
                        HttpContext.Session.Remove("IdCard");
                        HttpContext.Session.SetString("IdCard", person.EmployeeCode);
                        HttpContext.Session.SetString("ImagePath", "EmployeeData/" + person.TenantId + person.EmployeeCode + "/Image/" + person.Image);

                    }
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
                
                return RedirectToAction("Profile", "People", new { PersonId = person.EmployeeCode });
            }
            return View(person);
        }
        [AllowAnonymous]
        public List<Person> EmployeeData()
        {
            response = _services.Employee.GetResponse(ApiUrl+"/api/employee" );
            string stringData = response.Content.ReadAsStringAsync().Result;
            data = JsonConvert.DeserializeObject<List<Person>>(stringData);

            HttpResponseMessage response2 = _services.Roles.GetResponse(ApiUrl+"/api/employee/Designations" );
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
        [DisplayName("Delete Employee")]
        public void DeleteConfirmed(string id)
        {
            Person person = new Person();
            HttpClient client = _services.Employee.GetService();
            HttpResponseMessage response = _services.Employee.PostResponse(ApiUrl + "/api/Employee/PersonDelete/" + id,null);
            if (response!=null)
            {

            }

        }
        [DisplayName("Activate Employee")]
        public IActionResult ActivateEmployee(string EmployeeCode)
        {
            response = _services.Employee.GetResponse(ApiUrl+"/api/employee/ActivatePerson/" + EmployeeCode + "" );
            if(response.IsSuccessStatusCode)
                ViewBag.Message = "Information activated successfully!";
            return RedirectToAction("Index", "People");
        }
        #endregion

        #region Roles
        [DisplayName("Manage Roles")]
        public IActionResult Roles()
        {
            response = _services.Roles.GetResponse(ApiUrl+"/api/employee/Designations" );
            string stringData = response.Content.ReadAsStringAsync().Result;
            List<Role> data = JsonConvert.DeserializeObject<List<Role>>(stringData);
            return View(data);
        }

        [DisplayName("Add Role")]
        public IActionResult AddRole()
        {
            response = _services.Roles.GetResponse(ApiUrl+"/api/employee/Designations" );
            string stringData = response.Content.ReadAsStringAsync().Result;
            List<Role> data = JsonConvert.DeserializeObject<List<Role>>(stringData);
            ViewBag.Designations = data;
            ViewData["Controllers"] = _controllerService.GetControllers().OrderBy(x=>x.DisplayName);
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddRole(RoleViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Controllers"] = _controllerService.GetControllers().OrderBy(x => x.DisplayName);
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
                role.Access = JsonConvert.SerializeObject(UserAccess);
            }
            role.CreatedBy = Convert.ToInt32(GetSession().PersonId);
            HttpResponseMessage response = _services.Roles.PostResponse(ApiUrl + "/api/Employee/AddDesignation/" + 0, role);
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
            ViewData["Controllers"] = _controllerService.GetControllers().OrderBy(x => x.DisplayName);
            HttpResponseMessage response = _services.Roles.GetResponse(ApiUrl+"/api/Employee/Designations/" + id + "" );
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
                ViewData["Controllers"] = _controllerService.GetControllers().OrderBy(x => x.DisplayName);
                return View(viewModel);
            }
            string stringData = _services.Roles.GetResponse(ApiUrl+"api/Employee/Designations/" + id + "" ).Content.ReadAsStringAsync().Result;
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
                role.Access = JsonConvert.SerializeObject(UserAccess);
            }
            if (ModelState.IsValid)
            {
                role.UpdatedBy = Convert.ToInt32(GetSession().PersonId);
                HttpResponseMessage response = _services.Roles.PostResponse(ApiUrl + "/api/Employee/AddDesignation/" + role.Id, role);
                if (response.IsSuccessStatusCode && GetSession().Role == role.Name)
                {
                    CookieModel cookieModel = GetSession();
                    cookieModel.Access = role.Access;
                    string CookieJson = JsonConvert.SerializeObject(cookieModel);
                    HttpContext.Session.Remove("CookieData");
                    HttpContext.Session.SetString("CookieData", CookieJson);
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
        public IActionResult CreatePermanentAddress(int pid,string EmployeeCode ,[Bind("Address,City,State,Country,PinCode,PhoneNumber")]Permanent permanent)
        {

            permanent.PersonId = pid;
            permanent.CreatedDate = DateTime.Now.Date;
            permanent.UpdatedDate = DateTime.Now.Date;
            permanent.IsActive = true;
            if (ModelState.IsValid)
            {
                permanent.CreatedBy = Convert.ToInt32(GetSession().PersonId);
                HttpResponseMessage response = _services.PermanentAddress.PostResponse(ApiUrl+ "/api/PermanentAddress/AddPermanent/" + 0, permanent );
                ViewBag.Message = response.Content.ReadAsStringAsync().Result;
                TempData["success"] = "Permanent Address created successfully.";
                return RedirectToAction("Profile", "People", new { PersonId = EmployeeCode });
            }
            return View();
        }
        [DisplayName("Update Permanent Address")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditPermanentAddress(int pid,string EmployeeCode, Permanent permanent)
        {
            permanent.PersonId = pid;
            permanent.UpdatedDate = DateTime.Now.Date;
            permanent.IsActive = true;
            if (ModelState.IsValid)
            {
                permanent.UpdatedBy = Convert.ToInt32(GetSession().PersonId);
                HttpResponseMessage response = _services.PermanentAddress.PostResponse(ApiUrl+ "/api/PermanentAddress/AddPermanent/" + permanent.Id, permanent );
                ViewBag.Message = response.Content.ReadAsStringAsync().Result;
                TempData["success"] = "Permanent Address updated successfully.";
            }
            return RedirectToAction("Profile", "People", new { PersonId = EmployeeCode });
        }
        [DisplayName("Delete Permanent Address")]
        public IActionResult DeletePermanentAddress(int perid)
        {
            HttpResponseMessage response = _services.PermanentAddress.PostResponse(ApiUrl + "/api/PermanentAddress/DeletePermanent/" + perid + "", null);
            ViewBag.Message = response.Content.ReadAsStringAsync().Result;
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Current Address
        [DisplayName("Add Current Address")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateCurrentAddress(int pid, string EmployeeCode, [Bind("Address,City,State,Country,PinCode,PhoneNumber")]Current current)
        {
            current.PersonId = pid;
            current.CreatedDate = DateTime.Now.Date;
            current.UpdatedDate = DateTime.Now.Date;
            current.IsActive = true;
            if (ModelState.IsValid)
            {
                current.CreatedBy = Convert.ToInt32(GetSession().PersonId);
                HttpResponseMessage response = _services.CurrentAddress.PostResponse(ApiUrl + "/api/CurrentAddress/AddCurrent/" + 0, current);
                ViewBag.Message = response.Content.ReadAsStringAsync().Result;
                TempData["success"] = "Current Address created successfully.";
            }
            return RedirectToAction("Profile", "People", new { PersonId = EmployeeCode });
        }
        [DisplayName("Update Current Address")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCurrentAddress(int pid,string EmployeeCode, Current current)
        {
            current.PersonId = pid;
            current.UpdatedDate = DateTime.Now.Date;
            current.IsActive = true;
            if (ModelState.IsValid)
            {
                current.UpdatedBy = Convert.ToInt32(GetSession().PersonId);
                HttpResponseMessage response = _services.CurrentAddress.PostResponse(ApiUrl + "/api/CurrentAddress/AddCurrent/" + current.Id, current);
                ViewBag.Message = response.Content.ReadAsStringAsync().Result;
                TempData["success"] = "Current Address updated successfully.";
            }
            return RedirectToAction("Profile", "People", new { PersonId = EmployeeCode });
        }
        [DisplayName("Delete Current Address")]
        public IActionResult DeleteCurrentAddress(int cid)
        {
            HttpResponseMessage response = _services.CurrentAddress.PostResponse(ApiUrl+"/api/CurrentAddress/DeleteCurrent/" + cid + "",null );
            ViewBag.Message = response.Content.ReadAsStringAsync().Result;
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Emergency Address
        [DisplayName("Add Emergency Address")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateEmergencyAddress(int pid, string EmployeeCode, [Bind("FirstName,LastName,Address,City,State,Country,PinCode,PhoneNumber,MobileNumber,Relation")]Emergency emergency)
        {

            emergency.PersonId = pid;
            emergency.CreatedDate = DateTime.Now.Date;
            emergency.UpdatedDate = DateTime.Now.Date;
            emergency.IsActive = true;
            if (ModelState.IsValid)
            {
                emergency.CreatedBy = Convert.ToInt32(GetSession().PersonId);
                HttpResponseMessage response = _services.EmergencyAddress.PostResponse(ApiUrl + "/api/EmergencyAddress/AddEmergency/" + 0, emergency);
                ViewBag.Message = response.Content.ReadAsStringAsync().Result;
                TempData["success"] = "Emergency Address created successfully.";
            }
            return RedirectToAction("Profile", "People", new { PersonId = EmployeeCode });
        }
        [DisplayName("Update Emergency Address")]
        [HttpGet]
        public IActionResult EditEmergencyAddress(int eid)
        {
            HttpResponseMessage response = _services.EmergencyAddress.GetResponse(ApiUrl+"/api/EmergencyAddress/Get/" + eid + "" );
            ViewBag.Message = response.Content.ReadAsStringAsync().Result;
            string emergency = response.Content.ReadAsStringAsync().Result;
            var data = JsonConvert.DeserializeObject<Emergency>(emergency);
            return PartialView("EditEmergencyAddress", data);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditEmergencyAddress(int pid, string EmployeeCode, Emergency emergency)
        {
            emergency.UpdatedDate = DateTime.Now.Date;
            emergency.IsActive = true;
            if (ModelState.IsValid)
            {
                emergency.UpdatedBy = Convert.ToInt32(GetSession().PersonId);
                HttpResponseMessage response = _services.EmergencyAddress.PostResponse(ApiUrl+ "/api/EmergencyAddress/AddEmergency/"+emergency.Id, emergency );
                ViewBag.Message = response.Content.ReadAsStringAsync().Result;
                TempData["success"] = "Emergency Address updated successfully.";
            }
            return RedirectToAction("Profile", "People", new { PersonId = EmployeeCode });
        }
        [DisplayName("Delete Emergency Address")]
        public IActionResult DeleteEmergencyAddress(int eid)
        {
            HttpResponseMessage response = _services.EmergencyAddress.PostResponse(ApiUrl+ "/api/EmergencyAddress/DeleteEmergency/" + eid + "",null );
            ViewBag.Message = response.Content.ReadAsStringAsync().Result;
            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}