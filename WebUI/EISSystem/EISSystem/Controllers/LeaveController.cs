using EIS.Entities.Leave;
using EIS.WebApp.IServices;
using EIS.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EIS.WebApp.Controllers
{
    [DisplayName("Leave Management")]
    public class LeaveController : Controller
    {
        #region Declarations
        HttpResponseMessage response;
        RedisAgent Cache = new RedisAgent();
        List<LeaveMaster> data;
        private IEISService<LeaveRequest> LeaveRequestService;
        private IEISService<LeaveMaster> LeaveMasterService;
        private IEISService<LeaveCredit> LeaveCreditService;
        #endregion
        
        #region Controller
        public LeaveController(IEISService<LeaveRequest> LeaveRequestService, IEISService<LeaveMaster> LeavePolicyService,IEISService<LeaveCredit> LeaveCreditService)
        {
            this.LeaveRequestService = LeaveRequestService;
            this.LeaveMasterService = LeavePolicyService;
            this.LeaveCreditService = LeaveCreditService;
            response = LeaveMasterService.GetResponse("api/LeavePolicy");
            string stringData = response.Content.ReadAsStringAsync().Result;
            data = JsonConvert.DeserializeObject<List<LeaveMaster>>(stringData);
        }
        #endregion

        #region Requests
        [DisplayName("View all requests")]
        public IActionResult EmployeeLeaveRequests()
        {
            response = LeaveMasterService.GetResponse("api/LeaveRequest");
            string stringData = response.Content.ReadAsStringAsync().Result;
            List<LeaveRequest> Requests = JsonConvert.DeserializeObject<List<LeaveRequest>>(stringData);
            return View(Requests);
        }

        [DisplayName("Show my leaves")]
        public IActionResult ShowMyLeaves()
        {
            int pid = Convert.ToInt32(Cache.GetStringValue("PersonId"));
            response = LeaveMasterService.GetResponse("api/LeaveRequest/Employee/" + pid + "");
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
                HttpResponseMessage response = LeaveRequestService.PostResponse("api/LeaveRequest", request);
                if (response.IsSuccessStatusCode == true)
                {                 
                    return View();
                }

            }

            return View("AddPolicy", request);
        }
        #endregion

        #region Leave Master
        [DisplayName("leave Policies")]
        public IActionResult LeavePolicies()
        {
            return View(data);
        }
        public IActionResult AddPolicy()
        {
            var model = new LeaveMaster();
            return PartialView("AddPolicy", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddPolicy(LeaveMaster Leave)
        {
            Leave.CreatedDate = DateTime.Now.Date;
            Leave.UpdatedDate = DateTime.Now.Date;
            if (ModelState.IsValid)
            {
                //DateTime ValidTo = Leave.ValidFrom.AddYears(1).AddDays(-1);
                //Leave.ValidTo = ValidTo;
                Leave.IsActive = true;
                HttpResponseMessage response = LeaveMasterService.PostResponse("api/LeavePolicy", Leave);
                string stringData = response.Content.ReadAsStringAsync().Result;
                LeaveMaster LeaveMaster = JsonConvert.DeserializeObject<LeaveMaster>(stringData);
                if (response.IsSuccessStatusCode == true)
                {
                    HttpResponseMessage response2 = LeaveMasterService.PostResponse("api/LeaveCredit/AddCredits", LeaveMaster);
                    if (response2.IsSuccessStatusCode == true)
                    {
                        return View();
                    }
                }

            }

            return View("AddPolicy", Leave);

        }

        #endregion

        #region Credits
        [DisplayName("leave Credits")]
        public IActionResult LeaveCredits()
        {
            response = LeaveMasterService.GetResponse("api/LeaveCredit");
            string stringData = response.Content.ReadAsStringAsync().Result;
            List<LeaveCredit> Credits = JsonConvert.DeserializeObject<List<LeaveCredit>>(stringData);
            return View(Credits);
        }

        #endregion
    }
}
