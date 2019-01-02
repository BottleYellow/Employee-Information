using EIS.Entities.Employee;
using EIS.Entities.Leave;
using EIS.Repositories.IRepository;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace EIS.WebAPI.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("api/LeaveRequest")]
    [ApiController]
    public class LeaveRequestController : BaseController
    {
        public LeaveRequestController(IRepositoryWrapper repository):base(repository)
        {
        }

        [HttpGet]
        public IEnumerable<LeaveRequest> GetLeaveRequests()
        {
            return _repository.Leave.FindAll();
        }

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
    
        [HttpPut("{id}")]
        public IActionResult PutLeaveRequest([FromRoute] int id, [FromBody] LeaveRequest leave)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
<<<<<<< HEAD
            _repository.Leave.UpdateAndSave(leave);
            return Ok(leave);
=======
            _repository.Leave.Update(leave);

            try
            {
                _repository.Leave.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;               
            }

            return NoContent();
>>>>>>> eab0133b5e8f6e86eb09bb18611280e9b8dcee1c
        }
        
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