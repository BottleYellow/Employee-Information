using EIS.Entities.Leave;
using EIS.Repositories.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace EIS.WebAPI.Controllers.Leave
{
    [Route("api/LeavePolicy")]
    [ApiController]
    public class LeavePolicyController : Controller
    {
        public readonly IRepositoryWrapper _repository;
        public LeavePolicyController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }
        [HttpGet]
        public IEnumerable<LeaveMaster> GetLeavePolicies()
        {
            return _repository.Leave.GetAllPolicies();
        }
        [HttpPost]
        public IActionResult PostLeavePolicy([FromBody] LeaveMaster policy)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _repository.Leave.CreateLeaveType(policy);
            _repository.Leave.Save();

            return CreatedAtAction("GetLeavePolicies", new { id = policy.Id}, policy);
        }
    }
}
