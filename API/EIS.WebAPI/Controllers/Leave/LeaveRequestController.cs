using EIS.Entities.Employee;
using EIS.Entities.Leave;
using EIS.Repositories.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel;

namespace EIS.WebAPI.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("api/LeaveRequest")]
    [ApiController]
    public class LeaveRequestController : Controller
    {
        public readonly IRepositoryWrapper _repository;
        public LeaveRequestController(IRepositoryWrapper repository) 
        {
            _repository = repository;
        }

        [DisplayName("View all requests")]
        [HttpGet]
        public IEnumerable<LeaveRequest> GetLeaveRequests()
        {
            return _repository.Leave.FindAll();
        }

        [DisplayName("Show my leaves")]
        [HttpGet("Employee/{id}")]
        public IActionResult GetLeaveRequestsByEmployee([FromRoute] int id)
        {
            var leave = _repository.Leave.FindAllByCondition(x => x.PersonId == id);
            if (leave == null)
            {
                return NotFound();
            }

            return Ok(leave);
        }

        [AllowAnonymous]
        [HttpGet("{PersonId}/{LeaveId}")]
        public IActionResult GetAvailableLeaves([FromRoute] int PersonId, [FromRoute] int LeaveId)
        {
            var leave = _repository.Leave.GetAvailableLeaves(PersonId, LeaveId);
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
                _repository.Leave.UpdateRequestStatus(RequestId, Status);
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
            _repository.Leave.UpdateAndSave(leave);
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
            _repository.Leave.CreateAndSave(leave);
            _repository.Leave.UpdateRequestStatus(leave.Id, "Pending");
            return Ok();
        }

        // DELETE: api/Leaves/5
        [HttpDelete("{id}")]
        public IActionResult DeleteLeave([FromRoute] int id)
        {
            var leave = _repository.Leave.FindByCondition(x => x.Id == id);
            if (leave == null)
            {
                return NotFound();
            }
            _repository.Leave.DeleteAndSave(leave);
            return Ok(leave);
        }
    }
}