using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using EIS.Entities.Employee;
using EIS.Entities.Leave;
using EIS.WebApp.IServices;
using EIS.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EIS.WebApp.Controllers
{
    [DisplayName("Leave Management")]
    public class LeaveController : Controller
    {
        #region Declarations
        HttpResponseMessage response;
        RedisAgent Cache = new RedisAgent();
        List<LeaveRules> data;
        private IServiceWrapper _services;
        #endregion
        
        #region Controller
        public LeaveController(IServiceWrapper services)
        {
            _services = services;
            response = _services.LeaveRules.GetResponse("api/LeavePolicy");
            string stringData = response.Content.ReadAsStringAsync().Result;
            data = JsonConvert.DeserializeObject<List<LeaveRules>>(stringData);
        }
        #endregion

        #region Requests
        [DisplayName("View all requests")]
        public IActionResult EmployeeLeaveRequests()
        {
            response = _services.LeaveRules.GetResponse("api/LeaveRequest");
            string stringData = response.Content.ReadAsStringAsync().Result;
            List<LeaveRequest> Requests = JsonConvert.DeserializeObject<List<LeaveRequest>>(stringData);
            return View(Requests);
        }

        [DisplayName("Show Employees Requests")]
        public IActionResult LeaveRequestsUnderMe()
        {
            response = _services.LeaveRules.GetResponse("api/LeaveRequest/RequestsUnderMe");
            string stringData = response.Content.ReadAsStringAsync().Result;
            List<LeaveRequest> Requests = JsonConvert.DeserializeObject<List<LeaveRequest>>(stringData);
            return View(Requests);
        }

        [DisplayName("Show my leaves")]
        public IActionResult ShowMyLeaves()
        {
            int pid = Convert.ToInt32(Cache.GetStringValue("PersonId"));
            response = _services.LeaveRules.GetResponse("api/LeaveRequest/Employee/" + pid + "");
            string stringData = response.Content.ReadAsStringAsync().Result;
            List<LeaveRequest> Requests = JsonConvert.DeserializeObject<List<LeaveRequest>>(stringData);
            return View(Requests);
        }
        [DisplayName("Request for leave")]
        public IActionResult RequestLeave()
        {
            if (data.Count == 0)
                ViewBag.ListOfPolicy = null;
            else
                ViewBag.ListOfPolicy = data;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RequestLeave(LeaveRequest request)
        {
            request.CreatedDate = DateTime.Now.Date;
            request.UpdatedDate = DateTime.Now.Date;
            request.AppliedDate = DateTime.Now.Date;
            if (ModelState.IsValid)
            {
                request.IsActive = true;
                request.Id = 0;
                HttpResponseMessage response = _services.LeaveRequest.PostResponse("api/LeaveRequest", request);
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
            ViewBag.ListOfPolicy = data;
            string stringData = _services.LeaveRequest.GetResponse("api/LeaveRequest/" + id + "").Content.ReadAsStringAsync().Result;
            LeaveRequest leave = JsonConvert.DeserializeObject<LeaveRequest>(stringData);
            return View(leave);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditLeaveRequest(int id,LeaveRequest request)
        {
            request.UpdatedDate = DateTime.Now.Date;
            if (ModelState.IsValid)
            {
                request.IsActive = true;
                HttpResponseMessage response = _services.LeaveRequest.PutResponse("api/LeaveRequest/"+id, request);
                if (response.IsSuccessStatusCode == true)
                {
                    return View();
                }

            }

            return View("RequestLeave", request);
        }
        #endregion

        #region Leave Type
        [DisplayName("leave Policies")]
        public IActionResult LeavePolicies()
        {
            return View(data);
        }
        [DisplayName("Add Leave Rule")]
        public IActionResult AddPolicy()
        {
            var model = new LeaveRules();
            return PartialView("AddPolicy", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddPolicy(LeaveRules Leave)
        {
            Leave.CreatedDate = DateTime.Now.Date;
            Leave.UpdatedDate = DateTime.Now.Date;
            if (ModelState.IsValid)
            {
                Leave.IsActive = true;
                HttpResponseMessage response = _services.LeaveRules.PostResponse("api/LeavePolicy", Leave);
                string stringData = response.Content.ReadAsStringAsync().Result;
                LeaveRules LeaveRules = JsonConvert.DeserializeObject<LeaveRules>(stringData);
                if (response.IsSuccessStatusCode == true)
                {
                    HttpResponseMessage response2 = _services.LeaveRules.PostResponse("api/LeaveCredit/AddCredits", LeaveRules);
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
            response = _services.LeaveCredit.GetResponse("api/LeaveCredit");
            string stringData = response.Content.ReadAsStringAsync().Result;
            List<LeaveCredit> Credits = JsonConvert.DeserializeObject<List<LeaveCredit>>(stringData);
            return View(Credits);
        }

        [DisplayName("Add Leave Credit")]
        public IActionResult AddCredit()
        {
            if (data.Count == 0)
                ViewBag.ListOfPolicy = null;
            else
                ViewBag.ListOfPolicy = data;
            HttpResponseMessage response = _services.Employee.GetResponse("api/employee");
            string stringData = response.Content.ReadAsStringAsync().Result;
            List<Person> data1 = JsonConvert.DeserializeObject<List<Person>>(stringData);
            ViewBag.Persons = data1;
            var model = new LeaveCredit();
            return PartialView("AddCredit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddCredit(LeaveCredit Credit)
        {
            Credit.CreatedDate = DateTime.Now.Date;
            Credit.UpdatedDate = DateTime.Now.Date;
            Credit.Available = Credit.AllotedDays;
            if (ModelState.IsValid)
            {
                Credit.IsActive = true;
                HttpResponseMessage response = _services.LeaveCredit.PostResponse("api/LeaveCredit", Credit);
                string stringData = response.Content.ReadAsStringAsync().Result;
                LeaveCredit LeaveRules = JsonConvert.DeserializeObject<LeaveCredit>(stringData);
                if (response.IsSuccessStatusCode == true)
                {              
                     return View();
                }
            }
            return View("AddCredit", Credit);

        }
        #endregion
    }
}
