using EIS.Entities.Employee;
using EIS.Entities.Generic;
using EIS.Entities.Leave;
using EIS.Repositories.IRepository;
using EIS.WebAPI.RedisCache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace EIS.WebAPI.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("api/LeaveRequest")]
    [ApiController]
    public class LeaveRequestController : BaseController
    {
        public LeaveRequestController(IRepositoryWrapper repository): base(repository)
        {
        }

        [DisplayName("View all requests")]
        [HttpPost]
        public IActionResult GetLeaveRequests([FromBody]SortGrid sortGrid)
        {
            ArrayList data = new ArrayList();
            var leaveData = _repository.LeaveRequest.FindAll();

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


        [DisplayName("Show my leaves")]
        [HttpPost]
        [Route("Employee/{id}")]
        public IActionResult GetLeaveRequestsByEmployee([FromBody]SortGrid sortGrid,[FromRoute] int id)
        {           
            ArrayList data = new ArrayList();
            var leaveData = _repository.LeaveRequest.FindAllByCondition(x => x.PersonId == id);
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
                data = _repository.LeaveRequest.GetDataByGridCondition(x => x.EmployeeName == sortGrid.Search, sortGrid, leaveData);
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
            return NoContent();
        }

        [DisplayName("Request for leave")]
        [HttpPost]
        [Route("PostLeave")]
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
            return Ok();
        }

        // DELETE: api/Leaves/5
        [HttpDelete("{id}")]
        public IActionResult DeleteLeave([FromRoute] int id)
        {
            var leave = _repository.LeaveRequest.FindByCondition(x => x.Id == id);
            if (leave == null)
            {
                return NotFound();
            }
            _repository.LeaveRequest.DeleteAndSave(leave);
            return Ok(leave);
        }
    }
}