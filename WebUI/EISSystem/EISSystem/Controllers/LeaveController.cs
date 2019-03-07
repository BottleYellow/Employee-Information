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
using EIS.Entities.SP;
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
        List<LeaveCredit> data;
        private IServiceWrapper _services;
        #endregion
        
        #region Controller
        public LeaveController(IServiceWrapper services, IEISService<EmployeeLeaves> service) :base(service)
        {
            _services = services;
        }
        #endregion

        #region Requests
        [DisplayName("View All Requests")]
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

        //[DisplayName("Show Employees Requests")]
        //public IActionResult LeaveRequestsUnderMe()
        //{
      
        //    return View();
        //}

        //[HttpPost]
        //[ActionName("LeaveRequestsUnderMe")]
        //public IActionResult GetLeaveRequestsUnderMe()
        //{
        //    int pid = Convert.ToInt32(GetSession().PersonId);
        //    ArrayList arrayData = new ArrayList();
        //    return LoadData<LeaveRequest>(ApiUrl + "/api/LeaveRequest/RequestsUnderMe/" + pid + "", null, null);
        //}

        [DisplayName("Show My Leaves")]
        public IActionResult ShowMyLeaves()
        {
            int pid = Convert.ToInt32(GetSession().PersonId);
            HttpResponseMessage response = _services.LeaveRequest.GetResponse(ApiUrl + "/api/LeaveRequest/GetAvailableCount/" + pid);
            string stringData = response.Content.ReadAsStringAsync().Result;
            int value = JsonConvert.DeserializeObject<int>(stringData);
            ViewBag.AvailableCount = value;
            return View();
        }
        
        [HttpPost]
        [ActionName("ShowMyLeaves")]
        public IActionResult GetMyLeaves()
        {
            int pid = Convert.ToInt32(GetSession().PersonId);
            HttpResponseMessage response = _services.LeaveRequest.GetResponse(ApiUrl + "/api/LeaveRequest/Employee/" + pid);
            string stringData = response.Content.ReadAsStringAsync().Result;
            List<SP_EmployeeLeaveRequest> leaveData = JsonConvert.DeserializeObject<List<SP_EmployeeLeaveRequest>>(stringData);
                return Json(leaveData);

        }

        [DisplayName("Request For Leave")]
        public IActionResult RequestLeave()
        {
            response = _services.LeaveRules.GetResponse(ApiUrl+ "/api/LeaveCredit/GetCreditsByPerson/" + GetSession().PersonId);
            string stringData = response.Content.ReadAsStringAsync().Result;
            data = JsonConvert.DeserializeObject<List<LeaveCredit>>(stringData).Where(x=> x.ValidFrom <= DateTime.Now.Date && DateTime.Now.Date <= x.ValidTo).ToList();
            if (data.Count == 0) { 
                ViewBag.Status = "NoData";
                ViewBag.ListOfPolicy = data;
            }
            else
                ViewBag.ListOfPolicy = data;
            return PartialView("RequestLeave");
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
            else Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return PartialView("RequestLeave", request);
        }
        [DisplayName("Add Leave By Hr")]
        public IActionResult AddLeaveByHr()
        {
            ViewBag.Persons = GetPersons();
            return PartialView("AddLeaveByHr");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddLeaveByHr(LeaveRequest request)
        {
            ViewBag.Persons = GetPersons();
            request.CreatedDate = DateTime.Now;
            request.AppliedDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                request.CreatedBy = Convert.ToInt32(GetSession().PersonId);
                request.IsActive = true;
                request.Id = 0;
                HttpResponseMessage response = _services.LeaveRequest.PostResponse(ApiUrl + "/api/LeaveRequest/Future", request);
                if (response.IsSuccessStatusCode == true)
                {
                    return View();
                }
            }
            else Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return PartialView("RequestLeave", request);
        }
        [DisplayName("Edit Leave Request")]
        public IActionResult EditLeaveRequest(int id)
        {
            response = _services.LeaveRules.GetResponse(ApiUrl+"/api/LeavePolicy" );
            string stringData1 = response.Content.ReadAsStringAsync().Result;
            data = JsonConvert.DeserializeObject<List<LeaveCredit>>(stringData1);
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
        [DisplayName("Add Past Leave")]
        public IActionResult AddPastLeave()
        {
            response = _services.LeaveRules.GetResponse(ApiUrl + "/api/LeaveCredit/GetCreditsByPerson/" + GetSession().PersonId);
            string stringData = response.Content.ReadAsStringAsync().Result;
            data = JsonConvert.DeserializeObject<List<LeaveCredit>>(stringData);
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

        }

        #endregion

        #region Leave Policy
        [DisplayName("Leave Policies")]
        public IActionResult LeavePolicies()
        {
            ViewBag.Locations = GetLocations();
            return View();
        }

        [HttpPost]
        [ActionName("LeavePolicies")]
        public IActionResult GetLeavePolicy(int LocationId)
        {
         HttpResponseMessage responseMessage=_service.GetResponse(ApiUrl + "/api/LeavePolicy/GetLeavePolicies/"+LocationId);
            string stringData = responseMessage.Content.ReadAsStringAsync().Result;
            IEnumerable<LeavePolicyViewModel> policy = JsonConvert.DeserializeObject<IEnumerable<LeavePolicyViewModel>>(stringData);
            return Json(policy);
        }

        [DisplayName("Add Leave Policy")]
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
        [DisplayName("Edit Leave Policy")]
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
                        return View();
                }
            }
            else Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return PartialView("EditPolicy", Leave);

        }
        [DisplayName("Delete Leave Policy")]
        public void DeletePolicy(int id)
        {
            HttpClient client = _services.LeaveRules.GetService();
            HttpResponseMessage response = _services.LeaveRules.PostResponse(ApiUrl + "/api/LeavePolicy/PolicyDelete/" + id, null);
            if (response != null)
            {

            }

        }
        #endregion

        #region Credits
        [DisplayName("Leave Credits")]
        public IActionResult LeaveCredits()
        {
            ViewBag.Locations = GetLocations();
            return View();
        }

        [ActionName("LeaveCredits")]
        [HttpPost]
        public IActionResult GetLeaveCredits(int LocationId)
        {
            HttpResponseMessage httpResponse= _service.GetResponse(ApiUrl + "/api/LeaveCredit/GetLeaveCredits/" + LocationId);
            string stringData = httpResponse.Content.ReadAsStringAsync().Result;
            IEnumerable<LeaveCreditViewModel> leaveCredits = JsonConvert.DeserializeObject<IEnumerable<LeaveCreditViewModel>>(stringData);
            return Json(leaveCredits);
        }



        [DisplayName("Add Leave Credit")]
        public IActionResult AddCredit()
        {
            ViewBag.Locations = GetLocations();
            response = _services.LeaveRules.GetResponse(ApiUrl+"/api/LeavePolicy" );
            string stringData1 = response.Content.ReadAsStringAsync().Result;
            List<LeaveRules> data = JsonConvert.DeserializeObject<List<LeaveRules>>(stringData1);
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
            if (Credit.PersonId == 0) ModelState.AddModelError("PersonId", "Please select Employee");
            if (Credit.LeaveId == 0) ModelState.AddModelError("LeaveType", "Please select leave type");
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
            else Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return PartialView("AddCredit", Credit);

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
                    return View();
                }
            }
            else Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return PartialView("EditCredit", credit);

        }
        [DisplayName("Delete Leave Credit")]
        public void DeleteCredit(int id)
        {
            HttpClient client = _services.LeaveCredit.GetService();
            HttpResponseMessage response = _services.LeaveCredit.PostResponse(ApiUrl + "/api/LeaveCredit/CreditDelete/" + id, null);
            if (response != null)
            {

            }

        }
        #endregion
    }
}
