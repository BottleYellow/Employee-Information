using System;
using System.Collections;
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
            return View();
        }

        [ActionName("EmployeeLeaveRequests")]
        [HttpPost]
        public IActionResult GetEmployeeLeaveRequests()
        {
            ArrayList arrayData = new ArrayList();
            arrayData = LoadData<LeaveRequest>("api/LeaveRequest/GetLeaveRequests");
            int recordsTotal = JsonConvert.DeserializeObject<int>(arrayData[0].ToString());
            IList<LeaveRequest> data = JsonConvert.DeserializeObject<IList<LeaveRequest>>(arrayData[1].ToString());
            return Json(new { recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
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
            ArrayList arrayData = new ArrayList();
            arrayData = LoadData<LeaveRequest>("api/LeaveRequest/RequestsUnderMe");
            int recordsTotal = JsonConvert.DeserializeObject<int>(arrayData[0].ToString());
            IList<LeaveRequest> data = JsonConvert.DeserializeObject<IList<LeaveRequest>>(arrayData[1].ToString());
            return Json(new { recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
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
            int pid = Convert.ToInt32(Cache.GetStringValue("PersonId"));
            ArrayList arrayData = new ArrayList();
            arrayData = LoadData<LeaveRequest>("api/LeaveRequest/Employee/" + pid + "");

            int recordsTotal = JsonConvert.DeserializeObject<int>(arrayData[0].ToString());
            IList<LeaveRequest> data = JsonConvert.DeserializeObject<IList<LeaveRequest>>(arrayData[1].ToString());
            return Json(new { recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

        [DisplayName("Request for leave")]
        public IActionResult RequestLeave()
        {
            response = _services.LeaveRules.GetResponse("api/LeavePolicy" );
            string stringData = response.Content.ReadAsStringAsync().Result;
            data = JsonConvert.DeserializeObject<List<LeaveRules>>(stringData);
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
                HttpResponseMessage response = _services.LeaveRequest.PostResponse("api/LeaveRequest", request );
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
            response = _services.LeaveRules.GetResponse("api/LeavePolicy" );
            string stringData1 = response.Content.ReadAsStringAsync().Result;
            data = JsonConvert.DeserializeObject<List<LeaveRules>>(stringData1);
            ViewBag.ListOfPolicy = data;
            string stringData = _services.LeaveRequest.GetResponse("api/LeaveRequest/" + id + "" ).Content.ReadAsStringAsync().Result;
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
                HttpResponseMessage response = _services.LeaveRequest.PutResponse("api/LeaveRequest/"+id, request );
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
            return View();
        }

        [ActionName("PastLeaves")]
        [HttpPost]
        public IActionResult GetPastLeaves()
        {
            ArrayList arrayData = new ArrayList();
            arrayData = LoadData<PastLeaves>("api/LeaveRequest/PastLeaves");
            int recordsTotal = JsonConvert.DeserializeObject<int>(arrayData[0].ToString());
            IList<PastLeaves> data = JsonConvert.DeserializeObject<IList<PastLeaves>>(arrayData[1].ToString());
            return Json(new { recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }
        [DisplayName("Add Past Leave")]
        public IActionResult AddPastLeave()
        {
            var model = new PastLeaves();
            return PartialView("AddPastLeave", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddPastLeave(PastLeaves Leave)
        {
            Leave.CreatedDate = DateTime.Now.Date;
            Leave.UpdatedDate = DateTime.Now.Date;
            if (ModelState.IsValid)
            {
                Leave.RequestedDays = Convert.ToInt32((Leave.ToDate - Leave.FromDate).TotalDays) + 1;
                Leave.IsActive = true;
                Leave.PersonId = Convert.ToInt32(Cache.GetStringValue("PersonId"));
                HttpResponseMessage response = _services.PastLeave.PostResponse("api/LeaveRequest/AddPastLeave", Leave);
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
            return View();
        }

        [HttpPost]
        [ActionName("LeavePolicies")]
        public IActionResult GetLeavePolicy()
        {
            ArrayList arrayData = new ArrayList();
            arrayData = LoadData<LeaveRules>("api/LeavePolicy/GetLeavePolicies");

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
                HttpResponseMessage response = _services.LeaveRules.PostResponse("api/LeavePolicy", Leave );
                string stringData = response.Content.ReadAsStringAsync().Result;
                LeaveRules LeaveRules = JsonConvert.DeserializeObject<LeaveRules>(stringData);
                if (response.IsSuccessStatusCode == true)
                {
                    HttpResponseMessage response2 = _services.LeaveRules.PostResponse("api/LeaveCredit/AddCredits", LeaveRules );
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
            return View();
        }

        [ActionName("LeaveCredits")]
        [HttpPost]
        public IActionResult GetLeaveCredits()
        {
            ArrayList arrayData = new ArrayList();
            arrayData = LoadData<LeaveRules>("api/LeaveCredit/GetLeaveCredits");

            int recordsTotal = JsonConvert.DeserializeObject<int>(arrayData[0].ToString());
            IList<LeaveCredit> data = JsonConvert.DeserializeObject<IList<LeaveCredit>>(arrayData[1].ToString());
            return Json(new { recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }



        [DisplayName("Add Leave Credit")]
        public IActionResult AddCredit()
        {
            response = _services.LeaveRules.GetResponse("api/LeavePolicy" );
            string stringData1 = response.Content.ReadAsStringAsync().Result;
            data = JsonConvert.DeserializeObject<List<LeaveRules>>(stringData1);
            if (data.Count == 0)
                ViewBag.ListOfPolicy = null;
            else
                ViewBag.ListOfPolicy = data;
            HttpResponseMessage response1 = _services.Employee.GetResponse("api/employee" );
            string stringData = response1.Content.ReadAsStringAsync().Result;
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
