using EIS.Entities.Employee;
using EIS.Entities.Generic;
using EIS.Entities.Hoildays;
using EIS.Entities.Leave;
using EIS.Repositories.IRepository;
using EIS.WebAPI.Services;
using System.Globalization;
using EIS.WebAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using EIS.Entities.SP;
using EIS.Entities.Models;

namespace EIS.WebAPI.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("api/LeaveRequest")]
    [ApiController]
    public class LeaveRequestController : BaseController
    {

        private readonly IConfiguration _configuration;
        public LeaveRequestController(IRepositoryWrapper repository, IConfiguration configuration) : base(repository)
        {
            _configuration = configuration;
        }

        [DisplayName("View all requests")]
        [Route("GetLeaveRequests")]
        [HttpPost]
        public IActionResult GetLeaveRequests([FromBody]SortGrid sortGrid)
        {
            ArrayList data = new ArrayList();
            IEnumerable<LeaveRequest> leaveData = null;

            if (sortGrid.LocationId == 0)
            {
                leaveData = _repository.LeaveRequest.FindAll().Where(x => x.Status == "Pending").Include(x => x.Person).Include(x => x.Person.Location).Where(x => x.TenantId == TenantId && x.Person.Location.IsActive == true).ToList();
            }
            else
            {
                leaveData = _repository.LeaveRequest.FindAll().Where(x => x.Status == "Pending").Include(x => x.Person).Include(x => x.Person.Location).Where(x => x.TenantId == TenantId && x.Person.Location.IsActive == true && x.Person.LocationId == sortGrid.LocationId).ToList();
            }


            if (string.IsNullOrEmpty(sortGrid.Search))
            {

                data = _repository.LeaveRequest.GetDataByGridCondition(null, sortGrid, leaveData.AsQueryable());
            }
            else
            {
                string search = sortGrid.Search.ToLower();
                data = _repository.LeaveRequest.GetDataByGridCondition(x => x.EmployeeName.ToLower().Contains(search) || x.LeaveType.ToLower().Contains(search) || x.Reason.ToLower().Contains(search) || x.Status.ToLower().Contains(search), sortGrid, leaveData.AsQueryable());
            }
            return Ok(data);

        }

        [Route("GetLeaveHistory/{locationId}/{employeeId}/{month}/{year}/{leaveType}/{status}")]
        [HttpGet]
        public IActionResult GetLeaveHistory([FromRoute]int locationId, [FromRoute]string employeeId, [FromRoute]int month, [FromRoute]int year, [FromRoute]string leaveType,[FromRoute]bool status)
        {
            List<LeaveRequestViewModel> leaveData = _repository.LeaveRequest.GetLeaveData(locationId, employeeId, month, year, TenantId, leaveType,status);
            return Ok(leaveData);

        }

        [Route("RequestsUnderMe/{PersonId}")]
        [DisplayName("Leave Requests of employees under me")]
        [HttpPost]
        public ActionResult GetLeaveRequestsUnderMe([FromRoute]int PersonId, [FromBody]SortGrid sortGrid)
        {
            ArrayList data = new ArrayList();
            IQueryable<LeaveRequest> leaveData = _repository.LeaveRequest.GetLeaveRequestUnderMe(Convert.ToInt32(PersonId), TenantId);

            if (string.IsNullOrEmpty(sortGrid.Search))
            {

                data = _repository.LeaveRequest.GetDataByGridCondition(null, sortGrid, leaveData);
            }
            else
            {
                data = _repository.LeaveRequest.GetDataByGridCondition(x => x.EmployeeName == sortGrid.Search, sortGrid, leaveData);
            }
            return Ok(data);
        }

        [DisplayName("View request")]
        [HttpGet("{id}")]
        public LeaveRequest GetLeaveRequestById([FromRoute] int id)
        {
            return _repository.LeaveRequest.FindByCondition(x => x.Id == id);
        }

        [DisplayName("Show my leaves")]
        [HttpGet]
        [Route("Employee/{id}")]
        public IActionResult GetLeaveRequestsByEmployee([FromRoute] int id)
        {
            List<SP_EmployeeLeaveRequest> leaveData = _repository.LeaveRequest.GetEmployeeLeaveData(id);
            return Ok(leaveData);
        }


        [AllowAnonymous]
        [HttpGet("{PersonId}/{LeaveId}")]
        public IActionResult GetAvailableLeaves([FromRoute] int PersonId, [FromRoute] int LeaveId)
        {
            var leave = _repository.LeaveCredit.GetAvailableLeaves(PersonId, LeaveId);
            if (leave == 0)
            {
                leave = -1;
                return Ok(leave);
            }
            return Ok(leave);
        }

        [AllowAnonymous]
        [Route("UpdateStatus/{RequestId}/{Status}/{PersonId}")]
        [HttpPost]
        public IActionResult UpdateRequestStatus([FromRoute]int RequestId, [FromRoute]string Status, [FromRoute]int PersonId)
        {
            if (!string.IsNullOrEmpty(Status))
            {
                string OldStatus = _repository.LeaveRequest.FindByCondition(x => x.Id == RequestId).Status;
                string messsege = _repository.LeaveRequest.UpdateRequestStatus(RequestId, Status, PersonId);
                SendMail(RequestId, Status, OldStatus);
                return Ok(messsege);
            }
            return NotFound();
        }

        // PUT: api/Leaves/5
        [HttpPut("{id}")]
        public IActionResult PutLeaveRequest([FromRoute] int id, [FromBody] LeaveRequest leave)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _repository.LeaveRequest.UpdateAndSave(leave);
            string msg = _repository.LeaveRequest.UpdateRequestStatus(leave.Id, null, leave.PersonId);
            return Ok(msg);
        }


        [DisplayName("Request for leave")]
        [HttpPost("{type}")]
        public IActionResult PostLeaveRequest([FromRoute]string type, [FromBody] LeaveRequest leave)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Person p = _repository.Employee.FindByCondition(x => x.Id == leave.PersonId);
            leave.EmployeeName = p.FullName;
            leave.TenantId = TenantId;
            leave.TypeId = _repository.LeaveCredit.FindByCondition(x => x.Id == leave.TypeId).LeaveId;
            _repository.LeaveRequest.CreateAndSave(leave);

            string msg = null;
            if (type == "Future")
            {
                msg = _repository.LeaveRequest.UpdateRequestStatus(leave.Id, "Pending", leave.PersonId);
            }
            else if (type == "Past")
            {
                msg = _repository.LeaveRequest.UpdateRequestStatus(leave.Id, "Approve", leave.PersonId);
            }

            SendMail(leave.Id, "Pending", null);
            return Ok(msg);
        }

        // DELETE: api/Leaves/5
        [HttpDelete("{id}")]
        public IActionResult DeleteLeave([FromRoute] int id)
        {
            LeaveRequest leave = _repository.LeaveRequest.FindByCondition(x => x.Id == id);
            if (leave == null)
            {
                return NotFound();
            }
            _repository.LeaveRequest.DeleteAndSave(leave);
            return Ok(leave);
        }

        public void SendMail(int RequestId, string status, string OldStatus)
        {
            LeaveRequest leave = _repository.LeaveRequest.FindByCondition(x => x.Id == RequestId);
            bool isPaid = _repository.LeaveRequest.FindAll().Include(x => x.TypeOfLeave).Where(x => x.Id == RequestId).FirstOrDefault().TypeOfLeave.IsPaid;
            Person person = _repository.Employee.FindByCondition(x => x.Id == leave.PersonId);
            string Role = null;
            if (person != null)
                Role = _repository.Employee.GetDesignationById(person.RoleId).Name;
            List<GetAdminHrManager> p = _repository.Employee.getAdminHrManager();
            string To = person.EmailAddress;
            string subject = "EMS Leave Request";
            string body = "Hello " + person.FirstName + "\n";
            string bodyforadmin = null;
            if (status == "Pending")
            {
                if (OldStatus == "Approved")
                {
                    body += "Your Approved leave request is currently on hold and current status is Pending.\n";
                    body += "Date From :" + leave.FromDate.ToString("dd/MM/yyyy") + "\n";
                    body += "Date To :" + leave.ToDate.ToString("dd/MM/yyyy") + "\n";
                    body += "Requested Days :" + leave.RequestedDays;
                    bodyforadmin = "Approved leave request of " + person.FullName + " is currently on hold and curent status is Pending.\n";
                    bodyforadmin += "Leave Details : \n";
                    bodyforadmin += "Date From :" + leave.FromDate.ToString("dd/MM/yyyy") + "\n";
                    bodyforadmin += "Date To :" + leave.ToDate.ToString("dd/MM/yyyy") + "\n";
                    bodyforadmin += "Requested Days :" + leave.RequestedDays;
                }
                else
                {
                    body += "Your leave request for " + leave.RequestedDays.ToString() + " days is submitted successfully.\n";
                    body += "Date From:" + leave.FromDate.ToString("dd/MM/yyyy") + "\n";
                    body += "Date To:" + leave.ToDate.ToString("dd/MM/yyyy") + "\n";
                    body += "Requested Days:" + leave.RequestedDays;
                    bodyforadmin = person.FullName + " has send a request for " + leave.LeaveType + " leave from " + leave.FromDate.ToString("dd/MM/yyyy") + " to " + leave.ToDate.ToString("dd/MM/yyyy") + ".";
                }
            }
            else if (status == "Reject")
            {
                body += "Your leave request for " + leave.RequestedDays.ToString() + " days from " + leave.FromDate.ToString("dd/MM/yyyy") + " to " + leave.ToDate.ToString("dd/MM/yyyy") + " has been rejected";
                bodyforadmin = "The leave request of " + person.FullName + " for " + leave.RequestedDays.ToString() + " days from " + leave.FromDate.ToString("dd/MM/yyyy") + " to " + leave.ToDate.ToString("dd/MM/yyyy") + " has been rejected successfully.";
            }
            else if (status == "Approve")
            {
                LeaveCredit credit = _repository.LeaveCredit.FindByCondition(x => x.LeaveId == leave.TypeId && x.PersonId == leave.PersonId);
                if (credit != null && isPaid==true)
                {
                    body += "Your leave request for " + leave.RequestedDays.ToString() + " days from " + leave.FromDate.ToString("dd/MM/yyyy") + " to " + leave.ToDate.ToString("dd/MM/yyyy") + " has been approved.\n Remaining available leaves are " + credit.Available.ToString() + " days";
                }
                else
                {
                    body += "Your leave request for " + leave.RequestedDays.ToString() + " days from " + leave.FromDate.ToString("dd/MM/yyyy") + " to " + leave.ToDate.ToString("dd/MM/yyyy") + " has been approved.";
                }
                bodyforadmin = "The leave request of " + person.FullName + " for " + leave.RequestedDays.ToString() + " days from " + leave.FromDate.ToString("dd/MM/yyyy") + " to " + leave.ToDate.ToString("dd/MM/yyyy") + " has been approved successfully.";
            }
            else if (status == "Cancel")
            {
                if (OldStatus == "Pending")
                {
                    body += "Your leave request for " + leave.RequestedDays.ToString() + " days from " + leave.FromDate.ToString("dd/MM/yyyy") + " to " + leave.ToDate.ToString("dd/MM/yyyy") + " has been cancelled";
                }
                else
                {
                    body += "Your cancelling request for " + leave.RequestedDays.ToString() + " days from " + leave.FromDate.ToString("dd/MM/yyyy") + " to " + leave.ToDate.ToString("dd/MM/yyyy") + " is submitted successfully.";
                    bodyforadmin = person.FullName + " has send a request to cancel the leave from " + leave.FromDate.ToString("dd/MM/yyyy") + " to " + leave.ToDate.ToString("dd/MM/yyyy");
                }
            }
            else if (status == "Accept Cancel")
            {
                body += "Your cancelling leave request for " + leave.RequestedDays.ToString() + " days from " + leave.FromDate.ToString("dd/MM/yyyy") + " to " + leave.ToDate.ToString("dd/MM/yyyy") + " has been approved.";
                bodyforadmin = "The leave approved for " + person.FullName + " from " + leave.FromDate.ToString("dd/MM/yyyy") + " to " + leave.ToDate.ToString("dd/MM/yyyy") + " has been cancelled by his/her request.";
            }
            else if (status == "Reject Cancel")
            {
                body += "Your cancelling leave request for " + leave.RequestedDays.ToString() + " days from " + leave.FromDate.ToString("dd/MM/yyyy") + " to " + leave.ToDate.ToString("dd/MM/yyyy") + " has been rejected.";
                bodyforadmin = "The request for 'cancelling the leave request' send by " + person.FullName + " from " + leave.FromDate.ToString("dd/MM/yyyy") + " to " + leave.ToDate.ToString("dd/MM/yyyy") + " has been rejected successfully.";
            }

            if (bodyforadmin != null)
            {
                foreach (var pers in p)
                {
                    if (pers.Name != Role)
                        new EmailManager(_configuration, _repository).SendEmail(subject, bodyforadmin, pers.EmailAddress, null);
                }
            }
            new EmailManager(_configuration, _repository).SendEmail(subject, body, To, null);
            _repository.LeaveCredit.Dispose();
        }

        [DisplayName("PastLeaves")]
        [Route("PastLeaves/{id}")]
        [HttpPost]
        public IActionResult GetPastLeaves([FromRoute]int id, [FromBody]SortGrid sortGrid)
        {
            ArrayList data = new ArrayList();
            IQueryable<PastLeaves> leaveData = _repository.LeaveRequest.GetPastLeaves(id, TenantId, sortGrid.LocationId);

            if (string.IsNullOrEmpty(sortGrid.Search))
            {

                data = _repository.PastLeaves.GetDataByGridCondition(null, sortGrid, leaveData);
            }
            else
            {
                string search = sortGrid.Search.ToLower();
                data = _repository.PastLeaves.GetDataByGridCondition(x => x.Person.Location.LocationName.Contains(search) || x.EmployeeName.ToLower().Contains(search) || x.Reason.ToLower().Contains(search), sortGrid, leaveData);
            }
            return Ok(data);

        }
        [Route("AddPastLeave")]
        [DisplayName("Add Past Leave")]
        [HttpPost]
        public IActionResult AddPastLeave([FromBody] PastLeaves leave)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Person p = _repository.Employee.FindByCondition(x => x.Id == leave.PersonId);
            leave.EmployeeName = p.FirstName + " " + p.LastName;
            leave.TenantId = TenantId;
            _repository.LeaveRequest.AddPastLeave(leave);
            _repository.LeaveRequest.Dispose();
            //_repository.LeaveRequest.UpdateRequestStatus(leave.Id, "Pending");
            //SendMail(leave.Id, "Pending");
            return Ok();
        }
        [AllowAnonymous]
        [Route("CheckDates/{type}/{PersonId}/{LeaveId}/{FromDate}/{ToDate}")]
        [HttpGet]
        public IActionResult CheckForScheduledLeave([FromRoute]string type, [FromRoute]int PersonId, [FromRoute]int LeaveId, [FromRoute]DateTime FromDate, [FromRoute]DateTime ToDate)
        {
            string result = null;
            var credit = _repository.LeaveCredit.FindByCondition(x => x.PersonId == PersonId && x.Id == LeaveId);
            if (credit.ValidFrom <= FromDate.Date && FromDate.Date <= credit.ValidTo)
            {
                if (type == "Future")
                {
                    result = _repository.LeaveRequest.CheckForScheduledLeave(PersonId, FromDate, ToDate);
                }
                else if (type == "Past")
                {
                    result = _repository.LeaveRequest.CheckForScheduledPastLeave(PersonId, FromDate, ToDate);
                }
            }
            else
            {
                result = "You can only request for " + credit.LeaveType + " leave between " + credit.ValidFrom.ToString("dd/MM/yyyy").ToString() + " to " + credit.ValidTo.ToString("dd/MM/yyyy").ToString();
            }
            return Ok(result);
        }
        [AllowAnonymous]
        [Route("CalculateDates/{PersonId}/{days}/{FromDate}/{ToDate}")]
        [HttpGet]
        public IActionResult CalculateDates([FromRoute]int PersonId, [FromRoute]int days, [FromRoute]DateTime FromDate, [FromRoute]DateTime ToDate)
        {
            int requestedDays = days;
            int? LocationId = _repository.Employee.FindByCondition(x => x.Id == PersonId).LocationId;
            int count = 0;
            string WeeklyOffType = _repository.Employee.GetWeeklyOffByPerson(PersonId);
            for (var d = FromDate; d <= ToDate; d = d.AddDays(1))
            {
                Holiday holiday = _repository.Holidays.FindByCondition(x => x.Date == d && x.LocationId == LocationId && x.IsActive == true);
                if (holiday != null)
                {
                    if (holiday.Date.DayOfWeek == DayOfWeek.Sunday && d.DayOfWeek == DayOfWeek.Sunday)
                    {
                        count++;
                    }
                    else if (holiday.Date.DayOfWeek == DayOfWeek.Saturday && d.DayOfWeek == DayOfWeek.Saturday && WeeklyOffType == "AlternateSaturday")
                    {
                        string check = _repository.Attendances.CalculateDate(d);
                        count = (check == "2nd Saturday Weekly Off" || check == "4th Saturday Weekly Off") ? count + 1 : count;
                    }
                    else if (holiday.Date == d)
                    {
                        count++;
                    }
                    else if (d.DayOfWeek == DayOfWeek.Sunday)
                    {
                        count++;
                    }
                    else if (d.DayOfWeek == DayOfWeek.Saturday && WeeklyOffType == "AlternateSaturday")
                    {
                        string check = _repository.Attendances.CalculateDate(d);
                        count = (check == "2nd Saturday Weekly Off" || check == "4th Saturday Weekly Off") ? count + 1 : count;
                    }
                }
                else
                {
                    if (d.DayOfWeek == DayOfWeek.Sunday)
                    {
                        count++;
                    }
                    else if (d.DayOfWeek == DayOfWeek.Saturday && WeeklyOffType == "AlternateSaturday")
                    {
                        string check = _repository.Attendances.CalculateDate(d);
                        count = (check == "2nd Saturday Weekly Off" || check == "4th Saturday Weekly Off") ? count + 1 : count;
                    }
                }

            }
            requestedDays = requestedDays - count;

            return Ok(requestedDays);
        }

        [AllowAnonymous]
        [Route("GetAvailableCount/{personId}")]
        [HttpGet]
        public IActionResult GetAvailableCount([FromRoute]int personId)
        {
            float leave = _repository.LeaveCredit.FindAllByCondition(x => x.PersonId == personId).Include(x => x.LeaveRule).Where(x => x.IsActive == true).Sum(x => x.Available);
            int value = Convert.ToInt32(leave);
            return Ok(value);
        }

    }
}