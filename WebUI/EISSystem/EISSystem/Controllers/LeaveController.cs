using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using EIS.Entities.Employee;
using EIS.Entities.Leave;
using EIS.Entities.Models;
using EIS.WebApp.Filters;
using EIS.WebApp.IServices;
using EIS.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EIS.WebApp.Controllers
{
    [SessionTimeOut]
    [DisplayName("Leave Management")]
    public class LeaveController : BaseController<EmployeeLeaves>
    {
        #region Declarations
        HttpResponseMessage response;
        List<LeaveRules> data;
        private IServiceWrapper _services;
        #endregion
        
        #region Controller
        public LeaveController(IServiceWrapper services, IEISService<EmployeeLeaves> service) :base(service)
        {
            _services = services;
        }
        #endregion

        #region Requests
        //[DisplayName("View all requests")]
        //public IActionResult EmployeeLeaveRequests()
        //{
        //    ViewBag.Locations = GetLocations();
        //    return View();
        //}

        //[ActionName("EmployeeLeaveRequests")]
        //[HttpPost]
        //public IActionResult GetEmployeeLeaveRequests(int LocationId)
        //{
        //    ArrayList arrayData = new ArrayList();
        //    return LoadData<LeaveRequest>(ApiUrl + "/api/LeaveRequest/GetLeaveRequests/", null, LocationId);
        //}

        [DisplayName("View all requests")]
        public IActionResult EmployeeLeaveHistory()
        {
            ViewBag.Locations = GetLocations();
            return View();
        }

        [ActionName("EmployeeLeaveHistory")]
        [HttpPost]
        public IActionResult GetEmployeeLeaveHistory(int locationId,int employeeId, string leaveType, string type,string value)
        {
            int month=0;
            int year=0;
            string[] dateSplit = new string[1];
            if(type=="month")
            {
                dateSplit = value.Split('/');
                month =Convert.ToInt32(dateSplit[0]);
                year = Convert.ToInt32(dateSplit[1]);

            }
            else
                if(type=="year")
            {
                year = Convert.ToInt32(value);
            }
            HttpResponseMessage response = _service.GetResponse(ApiUrl + "/api/LeaveRequest/GetLeaveHistory/" + locationId + "/" + employeeId + "/" + month + "/" + year+"/"+ leaveType);
           string stringData= response.Content.ReadAsStringAsync().Result;
            List<LeaveRequestViewModel> leave = JsonConvert.DeserializeObject<List<LeaveRequestViewModel>>(stringData);
            return Json(leave);
        }

        [DisplayName("Show Employees Requests")]
        public IActionResult LeaveRequestsUnderMe()
        {
      
            return View();
        }

        [HttpPost]
        [ActionName("LeaveRequestsUnderMe")]
        public IActionResult GetLeaveRequestsUnderMe()
        {
            int pid = Convert.ToInt32(GetSession().PersonId);
            ArrayList arrayData = new ArrayList();
            return LoadData<LeaveRequest>(ApiUrl + "/api/LeaveRequest/RequestsUnderMe/" + pid + "", null, null);
        }

        [DisplayName("Show my leaves")]
        public IActionResult ShowMyLeaves()
        {
            return View();
        }
        
        [HttpPost]
        [ActionName("ShowMyLeaves")]
        public IActionResult GetMyLeaves()
        {
            //int pid = Convert.ToInt32(Cache.GetStringValue("PersonId"));
            int pid = Convert.ToInt32(GetSession().PersonId);
            ArrayList arrayData = new ArrayList();
            return LoadData<LeaveRequest>(ApiUrl + "/api/LeaveRequest/Employee/" + pid + "", null, null);

        }

        [DisplayName("Request for leave")]
        public IActionResult RequestLeave()
        {
            response = _services.LeaveRules.GetResponse(ApiUrl+ "/api/LeavePolicy/GetPolicyByLocation/"+GetSession().PersonId);
            string stringData = response.Content.ReadAsStringAsync().Result;
            data = JsonConvert.DeserializeObject<List<LeaveRules>>(stringData);
            if (data.Count == 0) { 
                ViewBag.Status = "NoData";
                ViewBag.ListOfPolicy = data;
            }
            else
                ViewBag.ListOfPolicy = data;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RequestLeave(LeaveRequest request)
        {
            request.CreatedDate = DateTime.Now;
            request.AppliedDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                request.CreatedBy = Convert.ToInt32(GetSession().PersonId);
                request.IsActive = true;
                request.Id = 0;
                HttpResponseMessage response = _services.LeaveRequest.PostResponse(ApiUrl+"/api/LeaveRequest/Future", request );
                if (response.IsSuccessStatusCode == true)
                {
                    return View();
                }
            }
            return View("RequestLeave", request);
        }
        [DisplayName("Edit Leave Request")]
        public IActionResult EditLeaveRequest(int id)
        {
            response = _services.LeaveRules.GetResponse(ApiUrl+"/api/LeavePolicy" );
            string stringData1 = response.Content.ReadAsStringAsync().Result;
            data = JsonConvert.DeserializeObject<List<LeaveRules>>(stringData1);
            ViewBag.ListOfPolicy = data;
            string stringData = _services.LeaveRequest.GetResponse(ApiUrl+"/api/LeaveRequest/" + id + "" ).Content.ReadAsStringAsync().Result;
            LeaveRequest leave = JsonConvert.DeserializeObject<LeaveRequest>(stringData);
            return View(leave);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditLeaveRequest(int id,LeaveRequest request)
        {
            request.UpdatedDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                request.UpdatedBy = Convert.ToInt32(GetSession().PersonId);
                request.IsActive = true;
                HttpResponseMessage response = _services.LeaveRequest.PutResponse(ApiUrl+"/api/LeaveRequest/"+id, request );
                if (response.IsSuccessStatusCode == true)
                {
                    return View();
                }
            }
            return View("RequestLeave", request);
        }
        //[DisplayName("Past Leaves")]
        //public IActionResult PastLeaves()
        //{
        //    ViewBag.Locations = GetLocations();
        //    return View();
        //}

        //[ActionName("PastLeaves")]
        //[HttpPost]
        //public IActionResult GetPastLeaves(int LocationId)
        //{
        //    int PersonId = 0;
        //    if(GetSession().Role=="Employee")
        //    {
        //        PersonId = Convert.ToInt32(GetSession().PersonId);
        //    }
        //    return LoadData<PastLeaves>(ApiUrl + "/api/LeaveRequest/PastLeaves/" + PersonId, null, LocationId);

        //}
        [DisplayName("Add Past Leave")]
        public IActionResult AddPastLeave()
        {
            response = _services.LeaveRules.GetResponse(ApiUrl + "/api/LeavePolicy/GetPolicyByLocation/" + GetSession().PersonId);
            string stringData = response.Content.ReadAsStringAsync().Result;
            data = JsonConvert.DeserializeObject<List<LeaveRules>>(stringData);
            if (data.Count == 0)
            {
                ViewBag.Status = "NoData";
                ViewBag.ListOfPolicy = data;
            }
            else
                ViewBag.ListOfPolicy = data;
            return PartialView("AddPastLeave");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddPastLeave(LeaveRequest request)
        {
            request.CreatedDate = DateTime.Now;
            request.AppliedDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                request.CreatedBy = Convert.ToInt32(GetSession().PersonId);
                request.LeaveType = request.LeaveType + "(Past)";
                request.IsActive = true;
                request.Id = 0;
                HttpResponseMessage response = _services.LeaveRequest.PostResponse(ApiUrl + "/api/LeaveRequest/Past", request);
                if (response.IsSuccessStatusCode == true)
                {
                    return View();
                }
            }
            else Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return PartialView("AddPastLeave", request);
            //Leave.CreatedDate = DateTime.Now;
            //Leave.UpdatedDate = DateTime.Now;
            //if (ModelState.IsValid)
            //{
            //    Leave.CreatedBy = Convert.ToInt32(GetSession().PersonId);
            //    Leave.RequestedDays = Convert.ToInt32((Leave.ToDate - Leave.FromDate).TotalDays) + 1;
            //    Leave.IsActive = true;
            //    Leave.PersonId = Convert.ToInt32(GetSession().PersonId);
            //    HttpResponseMessage response = _services.PastLeave.PostResponse(ApiUrl+"/api/LeaveRequest/AddPastLeave", Leave);
            //    string stringData = response.Content.ReadAsStringAsync().Result;
            //    LeaveRules LeaveRules = JsonConvert.DeserializeObject<LeaveRules>(stringData);
            //    if (response.IsSuccessStatusCode == true)
            //    {
            //        return View();
            //    }
            //}
            //else Response.StatusCode = (int)HttpStatusCode.BadRequest;
            //return PartialView("AddPastLeave", Leave);

        }

        #endregion

        #region Leave Policy
        [DisplayName("leave Policies")]
        public IActionResult LeavePolicies()
        {
            ViewBag.Locations = GetLocations();
            return View();
        }

        [HttpPost]
        [ActionName("LeavePolicies")]
        public IActionResult GetLeavePolicy(int LocationId)
        {
            ArrayList arrayData = new ArrayList();
            return LoadData<LeaveRules>(ApiUrl + "/api/LeavePolicy/GetLeavePolicies", null, LocationId);
        }

        [DisplayName("Add Leave Rule")]
        public IActionResult AddPolicy()
        {
            ViewBag.Locations = GetLocations();
            LeaveRules leaveRule = new LeaveRules();
            return PartialView("AddPolicy", leaveRule);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddPolicy(LeaveRules Leave)
        {
            ViewBag.Locations = GetLocations();
            Leave.CreatedDate = DateTime.Now;
            Leave.UpdatedDate = DateTime.Now;
            if (Leave.LocationId == 0) ModelState.AddModelError("LocationId", "Please Select Location");
            if (ModelState.IsValid)
            {
                Leave.CreatedBy = Convert.ToInt32(GetSession().PersonId);
                Leave.IsActive = true;
                HttpResponseMessage response = _services.LeaveRules.PostResponse(ApiUrl+"/api/LeavePolicy/"+0, Leave );
                string stringData = response.Content.ReadAsStringAsync().Result;
                LeaveRules LeaveRules = JsonConvert.DeserializeObject<LeaveRules>(stringData);
                if (response.IsSuccessStatusCode == true)
                {
                    HttpResponseMessage response2 = _services.LeaveRules.PostResponse(ApiUrl+"/api/LeaveCredit/AddCredits", LeaveRules );
                    if (response2.IsSuccessStatusCode == true)
                    {
                        return View();
                    }
                }
            }
            else Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return PartialView("AddPolicy", Leave);

        }
        [DisplayName("Edit Leave Rule")]
        public IActionResult EditPolicy(int Id)
        {
            ViewBag.Locations = GetLocations();
            string stringData = _services.LeaveRules.GetResponse(ApiUrl + "/api/LeavePolicy/GetPolicyById/" + Id + "").Content.ReadAsStringAsync().Result;
            LeaveRules leaveRule = JsonConvert.DeserializeObject<LeaveRules>(stringData);
            return PartialView("EditPolicy", leaveRule);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditPolicy(LeaveRules Leave)
        {
            ViewBag.Locations = GetLocations();
            Leave.UpdatedDate = DateTime.Now;
            Leave.UpdatedBy = Convert.ToInt32(GetSession().PersonId);
            if (ModelState.IsValid)
            {
                Leave.IsActive = true;
                HttpResponseMessage response = _services.LeaveRules.PostResponse(ApiUrl + "/api/LeavePolicy/"+Leave.Id, Leave);
                string stringData = response.Content.ReadAsStringAsync().Result;
                LeaveRules LeaveRules = JsonConvert.DeserializeObject<LeaveRules>(stringData);
                if (response.IsSuccessStatusCode == true)
                {
                    //HttpResponseMessage response2 = _services.LeaveRules.PostResponse(ApiUrl + "/api/LeaveCredit/AddCredits", LeaveRules);
                    //if (response2.IsSuccessStatusCode == true)
                    //{
                        return View();
                    //}
                }
            }
            else Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return PartialView("EditPolicy", Leave);

        }
        public void DeletePolicy(int id)
        {
            HttpClient client = _services.LeaveRules.GetService();
            HttpResponseMessage response = _services.LeaveRules.PostResponse(ApiUrl + "/api/LeavePolicy/PolicyDelete/" + id, null);
            //response = _services.Employee.PostResponse(ApiUrl + "/api/employee/Delete/" + id + "/" + op, person);
            if (response != null)
            {

            }

        }
        #endregion

        #region Credits
        [DisplayName("leave Credits")]
        public IActionResult LeaveCredits()
        {
            ViewBag.Locations = GetLocations();
            return View();
        }

        [ActionName("LeaveCredits")]
        [HttpPost]
        public IActionResult GetLeaveCredits(int LocationId)
        {
            ArrayList arrayData = new ArrayList();
            return LoadData<LeaveCredit>(ApiUrl + "/api/LeaveCredit/GetLeaveCredits", null, LocationId);
        }



        [DisplayName("Add Leave Credit")]
        public IActionResult AddCredit()
        {
            ViewBag.Locations = GetLocations();
            response = _services.LeaveRules.GetResponse(ApiUrl+"/api/LeavePolicy" );
            string stringData1 = response.Content.ReadAsStringAsync().Result;
            data = JsonConvert.DeserializeObject<List<LeaveRules>>(stringData1);
            if (data.Count == 0)
                ViewBag.ListOfPolicy = new List<LeaveRules>();
            else
                ViewBag.ListOfPolicy = data;
            HttpResponseMessage response1 = _services.Employee.GetResponse(ApiUrl+"/api/employee" );
            string stringData = response1.Content.ReadAsStringAsync().Result;
            List<Person> data1 = JsonConvert.DeserializeObject<List<Person>>(stringData);
            ViewBag.Persons = data1;
            LeaveCredit leaveCredit = new LeaveCredit();
            return PartialView("AddCredit", leaveCredit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddCredit(LeaveCredit Credit)
        {
            ViewBag.Locations = GetLocations();
            Credit.CreatedDate = DateTime.Now;
            Credit.UpdatedDate = DateTime.Now;
            Credit.Available = Credit.AllotedDays;
            if (ModelState.IsValid)
            {
                Credit.CreatedBy = Convert.ToInt32(GetSession().PersonId);
                Credit.IsActive = true;
                HttpResponseMessage response = _services.LeaveCredit.PostResponse(ApiUrl + "/api/LeaveCredit/" + 0, Credit);
                if (response.IsSuccessStatusCode == true)
                {
                    return RedirectToAction("LeaveCredits", "Leave");
                }
            }
            return View("AddCredit", Credit);

        }
        [DisplayName("Edit Leave Credit")]
        public IActionResult EditCredit(int Id)
        {
            ViewBag.Locations = GetLocations();
            string stringData = _services.LeaveCredit.GetResponse(ApiUrl + "/api/LeaveCredit/GetCreditById/" + Id + "").Content.ReadAsStringAsync().Result;
            LeaveCredit credit = JsonConvert.DeserializeObject<LeaveCredit>(stringData);
            return PartialView("EditCredit", credit);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCredit(LeaveCredit credit)
        {
            ViewBag.Locations = GetLocations();
            credit.UpdatedDate = DateTime.Now;
            credit.UpdatedBy = Convert.ToInt32(GetSession().PersonId);
            if (ModelState.IsValid)
            {
                credit.IsActive = true;
                HttpResponseMessage response = _services.LeaveCredit.PostResponse(ApiUrl + "/api/LeaveCredit/" + credit.Id, credit);
                string stringData = response.Content.ReadAsStringAsync().Result;
                LeaveCredit LeaveRules = JsonConvert.DeserializeObject<LeaveCredit>(stringData);
                if (response.IsSuccessStatusCode == true)
                {
                    //HttpResponseMessage response2 = _services.LeaveRules.PostResponse(ApiUrl + "/api/LeaveCredit/AddCredits", LeaveRules);
                    //if (response2.IsSuccessStatusCode == true)
                    //{
                    return View();
                    //}
                }
            }
            else Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return PartialView("EditCredit", credit);

        }
        public void DeleteCredit(int id)
        {
            HttpClient client = _services.LeaveCredit.GetService();
            HttpResponseMessage response = _services.LeaveCredit.PostResponse(ApiUrl + "/api/LeaveCredit/CreditDelete/" + id, null);
            //response = _services.Employee.PostResponse(ApiUrl + "/api/employee/Delete/" + id + "/" + op, person);
            if (response != null)
            {

            }

        }
        #endregion
    }
}
