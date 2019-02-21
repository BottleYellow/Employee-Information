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

namespace EIS.WebAPI.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("api/LeaveRequest")]
    [ApiController]
    public class LeaveRequestController : BaseController
    {

        private readonly IConfiguration _configuration;
        public LeaveRequestController(IRepositoryWrapper repository, IConfiguration configuration):base(repository)
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

            if (sortGrid.LocationId==0)
            {
                leaveData = _repository.LeaveRequest.FindAll().Where(x=>x.Status=="Pending").Include(x => x.Person).Include(x => x.Person.Location).Where(x => x.TenantId == TenantId && x.Person.Location.IsActive == true).ToList();
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
                data = _repository.LeaveRequest.GetDataByGridCondition(x => x.EmployeeName.ToLower().Contains(search) || x.LeaveType.ToLower().Contains(search)||x.Reason.ToLower().Contains(search) || x.Status.ToLower().Contains(search), sortGrid, leaveData.AsQueryable());
            }
            return Ok(data);

        }

        [Route("GetLeaveHistory/{employeeCode}/{month}/{year}")]
        [HttpPost]
        public IActionResult GetLeaveHistory([FromBody]SortGrid sortGrid, [FromRoute]string employeeCode, [FromRoute]string month,[FromRoute]string year)
        {
            ArrayList data = new ArrayList();
            IEnumerable<LeaveRequest> leaveData = null;
            int monthData = DateTime.ParseExact(month, "MMMM", CultureInfo.InvariantCulture).Month;
            int yearData = Convert.ToInt32(year);
            int IdData = Convert.ToInt32(employeeCode);
            if (sortGrid.LocationId == 0)
            {
                leaveData= employeeCode == "0"? _repository.LeaveRequest.FindAllByCondition(x => (x.FromDate.Month == monthData || x.ToDate.Month == monthData) && (x.FromDate.Year == yearData || x.ToDate.Year == yearData)).Where(x => x.Status != "Pending").Include(x => x.Person).Include(x => x.Person.Location).Where(x => x.TenantId == TenantId && x.Person.Location.IsActive == true).ToList():
                                                _repository.LeaveRequest.FindAllByCondition(x => (x.FromDate.Month == monthData || x.ToDate.Month == monthData) && (x.FromDate.Year == yearData || x.ToDate.Year == yearData)).Where(x => x.Status != "Pending").Include(x => x.Person).Include(x => x.Person.Location).Where(x => x.Person.Id == IdData && x.TenantId == TenantId && x.Person.Location.IsActive == true).ToList();                
            }
            else
            {
                leaveData = employeeCode == "0" ? _repository.LeaveRequest.FindAllByCondition(x => (x.FromDate.Month == monthData || x.ToDate.Month == monthData) && (x.FromDate.Year == yearData || x.ToDate.Year == yearData)).Where(x => x.Status != "Pending").Include(x => x.Person).Include(x => x.Person.Location).Where(x => x.TenantId == TenantId && x.Person.Location.IsActive == true && x.Person.LocationId == sortGrid.LocationId).ToList():
                                                 _repository.LeaveRequest.FindAllByCondition(x => (x.FromDate.Month == monthData || x.ToDate.Month == monthData)&&( x.FromDate.Year == yearData || x.ToDate.Year == yearData)).Where(x => x.Status != "Pending").Include(x => x.Person).Include(x => x.Person.Location).Where(x => x.Person.Id == IdData && x.TenantId == TenantId && x.Person.Location.IsActive == true && x.Person.LocationId == sortGrid.LocationId).ToList();              
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

        [Route("RequestsUnderMe/{PersonId}")]
        [DisplayName("Leave Requests of employees under me")]
        [HttpPost]
        public ActionResult GetLeaveRequestsUnderMe([FromRoute]int PersonId , [FromBody]SortGrid sortGrid)
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
        [HttpPost]
        [Route("Employee/{id}")]
        public IActionResult GetLeaveRequestsByEmployee([FromBody]SortGrid sortGrid, [FromRoute] int id)
        {
            ArrayList data = new ArrayList();
            IQueryable<LeaveRequest> leaveData = _repository.LeaveRequest.FindAllByCondition(x => x.PersonId == id);
        
            if (leaveData == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(sortGrid.Search))
            {
               
                data = _repository.LeaveRequest.GetDataByGridCondition(null, sortGrid, leaveData);
            }
            else
            {
                data = _repository.LeaveRequest.GetDataByGridCondition(x=>x.LeaveType.ToLower().Contains(sortGrid.Search.ToLower())||x.Status.Contains(sortGrid.Search.ToLower()), sortGrid, leaveData);
            }
            return Ok(data);

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
                string messsege =_repository.LeaveRequest.UpdateRequestStatus(RequestId, Status, PersonId);
                SendMail(RequestId, Status);
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
            string msg =_repository.LeaveRequest.UpdateRequestStatus(leave.Id, null, leave.PersonId);
            return Ok(msg);
        }


        [DisplayName("Request for leave")]
        [HttpPost("{type}")]
        public IActionResult PostLeaveRequest([FromRoute]string type,[FromBody] LeaveRequest leave)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Person p = _repository.Employee.FindByCondition(x => x.Id == leave.PersonId);
            leave.EmployeeName = p.FullName;
            leave.TenantId = TenantId;
            _repository.LeaveRequest.CreateAndSave(leave);

            //string to = person.Select(x => x.EmailAddress).ToString();
            string msg = null;
            if (type == "Future")
            {
                msg = _repository.LeaveRequest.UpdateRequestStatus(leave.Id, "Pending", leave.PersonId);
            }
            else if (type == "Past")
            {
                msg = _repository.LeaveRequest.UpdateRequestStatus(leave.Id, "Approve", leave.PersonId);
            }
           
            //List<Person> person = _repository.Employee.FindAll().Include(x => x.Role).Where(x => x.Role.Name == "Admin" || x.Role.Name == "Manager" || x.Role.Name == "HR").ToList();
            //foreach (var x in person)
            //{
            //    var name = p.FirstName + " " + p.LastName;
            //    SendMail(x.EmailAddress, leave.LeaveType, leave.FromDate, leave.ToDate, name);
            //}
            SendMail(leave.Id, "Pending");
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

        public void SendMail(int RequestId,string status)
        {
            LeaveRequest leave = _repository.LeaveRequest.FindByCondition(x => x.Id == RequestId);
            Person person = _repository.Employee.FindByCondition(x => x.Id == leave.PersonId);
            string To = person.EmailAddress;
            string subject = "EMS Leave Request";
            string body = "Hello " + person.FirstName + "\n";
            if (status == "Pending")
            {
                body += "Your leave request for " + leave.RequestedDays.ToString() + " days is submitted successfully.\n";
                body += "Date From:" + leave.FromDate + "\n";
                body += "Date To:" + leave.ToDate + "\n";
                body += "Requested Days:" + leave.RequestedDays;
            }
            else if (status == "Reject")
            {
                body += "Your leave request for " + leave.RequestedDays.ToString() + " days has been rejected";
            }
            else if (status == "Approve")
            {
                body += "Your leave request for " + leave.RequestedDays.ToString() + " days has been approved.\n Remaining available leaves are " + leave.Available.ToString() + " days";
            }
            else if (status == "Cancel")
            {
                if (leave.Status == "Pending")
                {
                    body += "Your leave request for " + leave.RequestedDays.ToString() + " days has been cancelled";
                }
                else
                {
                    body += "Your cancelling request for " + leave.RequestedDays.ToString() + " days is submitted successfully.";
                }
            }
            else if (status == "Approve Cancel")
            {
                body += "Your cancelling request for " + leave.RequestedDays.ToString() + " days has been approved.";
            }
            else if (status == "Reject Cancel")
            {
                body += "Your cancelling request for " + leave.RequestedDays.ToString() + " days has been rejected.";
            }
            _repository.LeaveCredit.Dispose();
            new EmailManager(_configuration).SendEmail(subject, body, To,null);
        }

        [DisplayName("PastLeaves")]
        [Route("PastLeaves/{id}")]
        [HttpPost]
        public IActionResult GetPastLeaves([FromRoute]int id,[FromBody]SortGrid sortGrid)
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
                data = _repository.PastLeaves.GetDataByGridCondition(x => x.Person.Location.LocationName.Contains(search)|| x.EmployeeName.ToLower().Contains(search) || x.Reason.ToLower().Contains(search), sortGrid, leaveData);
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
        [Route("CheckDates/{type}/{PersonId}/{FromDate}/{ToDate}")]
        [HttpGet]
        public IActionResult CheckForScheduledLeave([FromRoute]string type,[FromRoute]int PersonId, [FromRoute]DateTime FromDate, [FromRoute]DateTime ToDate)
        {
            string result = null;
            if (type == "Future")
            {
                result = _repository.LeaveRequest.CheckForScheduledLeave(PersonId, FromDate, ToDate);
            }
            else if (type == "Past")
            {
               result = _repository.LeaveRequest.CheckForScheduledPastLeave(PersonId, FromDate, ToDate);
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
            for (var d = FromDate; d <= ToDate; d=d.AddDays(1))
            {
                Holiday holiday = _repository.Holidays.FindByCondition(x => x.Date == d && x.LocationId == LocationId);
                if (holiday != null)
                {
                    if (holiday.Date.DayOfWeek == DayOfWeek.Sunday && d.DayOfWeek == DayOfWeek.Sunday)
                    {
                        count++;
                    }
                    else if (holiday.Date.DayOfWeek == DayOfWeek.Saturday && d.DayOfWeek == DayOfWeek.Saturday && LocationId == 2)
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
                    else if (d.DayOfWeek == DayOfWeek.Saturday && LocationId == 2)
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
                    else if (d.DayOfWeek == DayOfWeek.Saturday && LocationId == 2)
                    {
                        string check = _repository.Attendances.CalculateDate(d);
                        count = (check == "2nd Saturday Weekly Off" || check == "4th Saturday Weekly Off") ? count + 1 : count;
                    }
                }

            }
            requestedDays = requestedDays - count;
          
            return Ok(requestedDays);
        }
        public void SendMail(string To,string leavetype,DateTime fromdate, DateTime todate,string name)
        {
            string to = To;
            string subject = "EMS Leave Request";
            string body = name + " " + "has send a request for " + leavetype + " leave from " + fromdate.ToString("dd/MM/yyyy") + " to " + todate.ToString("dd/MM/yyyy") + "."; 
            new EmailManager(_configuration).SendEmail(subject, body, To, null);
        }
    }
}