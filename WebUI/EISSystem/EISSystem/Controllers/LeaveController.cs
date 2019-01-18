using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using EIS.Entities.Leave;
using EIS.WebApp.IServices;
using EIS.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EIS.WebApp.Controllers
{
    [DisplayName("Leave Management")]
    public class LeaveController : BaseController<EmployeeLeaves>
    {
        #region Declarations
        HttpResponseMessage response;
        RedisAgent Cache = new RedisAgent();
        List<LeaveRules> data;
        private IServiceWrapper _services;
        #endregion
        
        #region Controller
        public LeaveController(IServiceWrapper services, IEISService<EmployeeLeaves> service) :base(service)
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
            return View();
        }

        [ActionName("EmployeeLeaveRequests")]
        [HttpPost]
        public IActionResult GetEmployeeLeaveRequests()
        {
            //response = _services.LeaveRules.GetResponse("api/LeaveRequest");
            //string stringData = response.Content.ReadAsStringAsync().Result;
            //List<LeaveRequest> Requests = JsonConvert.DeserializeObject<List<LeaveRequest>>(stringData);
            //return View(Requests);
            ArrayList arrayData = new ArrayList();
            arrayData = LoadData<LeaveRequest>("api/LeaveRequest");
            int recordsTotal = JsonConvert.DeserializeObject<int>(arrayData[0].ToString());
            IList<LeaveRequest> data = JsonConvert.DeserializeObject<IList<LeaveRequest>>(arrayData[1].ToString());
            return Json(new { recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

        [DisplayName("Show my leaves")]
        public IActionResult ShowMyLeaves()
        {
            return View();
        }

        [DisplayName("Show my leaves")]
        [HttpPost]
        [ActionName("ShowMyLeaves")]
        public IActionResult GetMyLeaves()
        {
            int pid = Convert.ToInt32(Cache.GetStringValue("PersonId"));
            //response = _services.LeaveRules.GetResponse("api/LeaveRequest/Employee/" + pid + "");
            //string stringData = response.Content.ReadAsStringAsync().Result;
            //List<LeaveRequest> Requests = JsonConvert.DeserializeObject<List<LeaveRequest>>(stringData);
            //return View(Requests);
            ArrayList arrayData = new ArrayList();
            arrayData = LoadData<LeaveRequest>("api/LeaveRequest/Employee/" + pid + "");

            int recordsTotal = JsonConvert.DeserializeObject<int>(arrayData[0].ToString());
            IList<LeaveRequest> data = JsonConvert.DeserializeObject<IList<LeaveRequest>>(arrayData[1].ToString());
            return Json(new { recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
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
        public IActionResult RequestLeave(DateTime FromDate)
        {
            //request.CreatedDate = DateTime.Now.Date;
            //request.UpdatedDate = DateTime.Now.Date;
            //request.AppliedDate = DateTime.Now.Date;
            //if (ModelState.IsValid)
            //{
            //    request.IsActive = true;
            //    request.Id = 0;
            //    HttpResponseMessage response = _services.LeaveRequest.PostResponse("api/LeaveRequest", request);
            //    if (response.IsSuccessStatusCode == true)
            //    {                 
            //        return View();
            //    }

            //}

            //return View("RequestLeave", request);
            var date1 = FromDate.ToShortTimeString();
            return View(date1);
        }
        #endregion

        #region Leave Type
        [DisplayName("leave Policies")]
        public IActionResult LeavePolicies()
        {
            return View();
        }

        [HttpPost]
        [ActionName("LeavePolicies")]
        public IActionResult GetLeavePolicy()
        {
            ArrayList arrayData = new ArrayList();
            arrayData = LoadData<LeaveRules>("api/LeavePolicy");

            int recordsTotal = JsonConvert.DeserializeObject<int>(arrayData[0].ToString());
            IList<LeaveRules> data = JsonConvert.DeserializeObject<IList<LeaveRules>>(arrayData[1].ToString());
            return Json(new { recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
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
            return PartialView("AddPolicy", Leave);

        }

        #endregion

        #region Credits
        [DisplayName("leave Credits")]
        public IActionResult LeaveCredits()
        {
            return View();
        }

        [ActionName("LeaveCredits")]
        [HttpPost]
        public IActionResult GetLeaveCredits()
        {
            ArrayList arrayData = new ArrayList();
            arrayData = LoadData<LeaveRules>("api/LeaveCredit");

            int recordsTotal = JsonConvert.DeserializeObject<int>(arrayData[0].ToString());
            IList<LeaveCredit> data = JsonConvert.DeserializeObject<IList<LeaveCredit>>(arrayData[1].ToString());
            return Json(new { recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }



        [DisplayName("Add Leave Credit")]
        public IActionResult AddCredit()
        {
            if (data.Count == 0)
                ViewBag.ListOfPolicy = null;
            else
                ViewBag.ListOfPolicy = data;
            ViewBag.Persons = PeopleController.data;
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
