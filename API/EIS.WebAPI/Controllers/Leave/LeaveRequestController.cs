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
    public class LeaveRequestController : Controller
    {
        public readonly IRepositoryWrapper _repository;
        public LeaveRequestController(IRepositoryWrapper repository) 
        {
            _repository = repository;
        }

        // GET: api/Leaves
        [HttpGet]
        public IEnumerable<LeaveRequest> GetLeaveRequests()
        {
            return _repository.Leave.FindAll();
        }

        // GET: api/Leaves/5
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

        // PUT: api/Leaves/5
        [HttpPut("{id}")]
        public IActionResult PutLeaveRequest([FromRoute] int id, [FromBody] LeaveRequest leave)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
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
        }

        // POST: api/Leaves
        [HttpPost]
        public IActionResult PostLeaveRequest([FromBody] LeaveRequest leave)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Person p = _repository.Employee.FindByCondition(x => x.Id == leave.PersonId);
            leave.EmployeeName = p.FirstName + " " + p.LastName;
            _repository.Leave.Create(leave);
            _repository.Leave.Save();

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
            _repository.Leave.Delete(leave);
            _repository.Leave.Save();
            return Ok(leave);
        }
    }
}