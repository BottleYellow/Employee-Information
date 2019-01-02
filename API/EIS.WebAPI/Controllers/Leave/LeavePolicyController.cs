using EIS.Entities.Leave;
using EIS.Repositories.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace EIS.WebAPI.Controllers.Leave
{
    [Route("api/LeavePolicy")]
    [ApiController]
    public class LeavePolicyController : BaseController
    {
        public LeavePolicyController(IRepositoryWrapper repository): base(repository)
        {

        }

        [HttpGet]
        public IEnumerable<LeaveRules> GetLeavePolicies()
        {
            return _repository.Leave.GetAllLeaveRules();
        }
        [HttpPost]
        public IActionResult PostLeavePolicy([FromBody] LeaveRules policy)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
<<<<<<< HEAD
            _repository.Leave.CreateLeaveTypeAndSave(policy);
=======
            _repository.Leave.CreateLeaveRule(policy);
            _repository.Leave.Save();

>>>>>>> eab0133b5e8f6e86eb09bb18611280e9b8dcee1c
            return CreatedAtAction("GetLeavePolicies", new { id = policy.Id}, policy);
        }
    }
}
