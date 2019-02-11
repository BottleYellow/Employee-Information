using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using EIS.Entities.Employee;
using EIS.Entities.Leave;
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
        [DisplayName("View all requests")]
        public IActionResult EmployeeLeaveRequests()
        {
            ViewBag.Locations = GetLocations();
            return View();
        }

        [ActionName("EmployeeLeaveRequests")]
        [HttpPost]
        public IActionResult GetEmployeeLeaveRequests(int LocationId)
        {
            ArrayList arrayData = new ArrayList();
            return LoadData<LeaveRequest>(ApiUrl + "/api/LeaveRequest/GetLeaveRequests", null, LocationId);
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
            if (data.Count == 0)
                ViewBag.ListOfPolicy = data;
            else
                ViewBag.ListOfPolicy = data;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RequestLeave(LeaveRequest request)
        {
            request.CreatedDate = DateTime.Now;
            request.UpdatedDate = DateTime.Now;
            request.AppliedDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                request.CreatedBy = Convert.ToInt32(GetSession().PersonId);
                request.IsActive = true;
                request.Id = 0;
                HttpResponseMessage response = _services.LeaveRequest.PostResponse(ApiUrl+"/api/LeaveRequest", request );
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
        [DisplayName("Past Leaves")]
        public IActionResult PastLeaves()
        {
            ViewBag.Locations = GetLocations();
            return View();
        }

        [ActionName("PastLeaves")]
        [HttpPost]
        public IActionResult GetPastLeaves(int LocationId)
        {
            int PersonId = 0;
            if(GetSession().Role=="Employee")
            {
                PersonId = Convert.ToInt32(GetSession().PersonId);
            }
            return LoadData<PastLeaves>(ApiUrl + "/api/LeaveRequest/PastLeaves/" + PersonId, null, LocationId);

        }
        [DisplayName("Add Past Leave")]
        public IActionResult AddPastLeave()
        {
            PastLeaves model = new PastLeaves();
            return PartialView("AddPastLeave", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddPastLeave(PastLeaves Leave)
        {
            Leave.CreatedDate = DateTime.Now;
            Leave.UpdatedDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                Leave.CreatedBy = Convert.ToInt32(GetSession().PersonId);
                Leave.RequestedDays = Convert.ToInt32((Leave.ToDate - Leave.FromDate).TotalDays) + 1;
                Leave.IsActive = true;
                Leave.PersonId = Convert.ToInt32(GetSession().PersonId);
                HttpResponseMessage response = _services.PastLeave.PostResponse(ApiUrl+"/api/LeaveRequest/AddPastLeave", Leave);
                string stringData = response.Content.ReadAsStringAsync().Result;
                LeaveRules LeaveRules = JsonConvert.DeserializeObject<LeaveRules>(stringData);
                if (response.IsSuccessStatusCode == true)
                {
                    return View();
                }
            }
            else Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return PartialView("AddPastLeave", Leave);

        }

        #endregion

        #region Leave Type
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
                HttpResponseMessage response = _services.LeaveRules.PostResponse(ApiUrl+"/api/LeavePolicy", Leave );
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
                HttpResponseMessage response = _services.LeaveCredit.PostResponse(ApiUrl+"/api/LeaveCredit", Credit);
                if (response.IsSuccessStatusCode == true)
                {
                    return RedirectToAction("LeaveCredits", "Leave");
                }
            }
            return View("AddCredit", Credit);

        }
        #endregion
    }
}
