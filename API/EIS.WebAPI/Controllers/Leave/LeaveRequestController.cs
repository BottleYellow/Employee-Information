using EIS.Entities.Employee;
using EIS.Entities.Generic;
using EIS.Entities.Leave;
using EIS.Repositories.IRepository;
using EIS.WebAPI.Services;
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
            IQueryable<LeaveRequest> leaveData = null;
            if(sortGrid.LocationId==0)
            {
                leaveData = _repository.LeaveRequest.FindAll().Include(x => x.Person).Include(x=>x.Person.Location).Where(x => x.TenantId == TenantId);
            }
            else
            {
                leaveData = _repository.LeaveRequest.FindAll().Include(x => x.Person).Include(x => x.Person.Location).Where(x => x.TenantId == TenantId && x.Person.LocationId == sortGrid.LocationId);
                leaveData.Include(x => x.Person.Location);
            }
            
           
            if (string.IsNullOrEmpty(sortGrid.Search))
            {

                data = _repository.LeaveRequest.GetDataByGridCondition(null, sortGrid, leaveData);
            }
            else
            {
                string search = sortGrid.Search.ToLower();
                data = _repository.LeaveRequest.GetDataByGridCondition(x => x.EmployeeName.ToLower().Contains(search) || x.LeaveType.ToLower().Contains(search)||x.Reason.ToLower().Contains(search), sortGrid, leaveData);
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
                data = _repository.LeaveRequest.GetDataByGridCondition(x=>x.LeaveType.ToLower().Contains(sortGrid.Search.ToLower()), sortGrid, leaveData);
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
        [Route("UpdateStatus/{RequestId}/{Status}")]
        [HttpPost]
        public IActionResult UpdateRequestStatus([FromRoute]int RequestId, [FromRoute]string Status)
        {
            if (!string.IsNullOrEmpty(Status))
            {
                _repository.LeaveRequest.UpdateRequestStatus(RequestId, Status);
                SendMail(RequestId, Status);
                return Ok();
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
            _repository.LeaveRequest.UpdateRequestStatus(leave.Id, null);
            return NoContent();
        }

        [DisplayName("Request for leave")]
        [HttpPost]
        public IActionResult PostLeaveRequest([FromBody] LeaveRequest leave)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Person p = _repository.Employee.FindByCondition(x => x.Id == leave.PersonId);
            leave.EmployeeName = p.FirstName + " " + p.LastName;
            leave.TenantId = TenantId;
            _repository.LeaveRequest.CreateAndSave(leave);
            _repository.LeaveRequest.UpdateRequestStatus(leave.Id, "Pending");
            SendMail(leave.Id, "Pending");
            return Ok();
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
            new EmailManager(_configuration).SendEmail(subject, body, To,null);
        }

        [DisplayName("PastLeaves")]
        [Route("PastLeaves/{id}")]
        [HttpPost]
        public IActionResult GetPastLeaves([FromRoute]int id,[FromBody]SortGrid sortGrid)
        {
            ArrayList data = new ArrayList();
            IQueryable<PastLeaves> leaveData = _repository.LeaveRequest.GetPastLeaves(id, TenantId);

            if (string.IsNullOrEmpty(sortGrid.Search))
            {

                data = _repository.PastLeaves.GetDataByGridCondition(null, sortGrid, leaveData);
            }
            else
            {
                string search = sortGrid.Search.ToLower();
                data = _repository.PastLeaves.GetDataByGridCondition(x => x.EmployeeName.ToLower().Contains(search) || x.Reason.ToLower().Contains(search), sortGrid, leaveData);
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
            //_repository.LeaveRequest.UpdateRequestStatus(leave.Id, "Pending");
            //SendMail(leave.Id, "Pending");
            return Ok();
        }
    }
}