using EIS.Entities.Leave;
using EIS.Repositories.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel;

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

        [DisplayName("leave Policies")]
        [HttpGet]
        public IEnumerable<LeaveRules> GetLeavePolicies()
        {
            return _repository.Leave.GetAllLeaveRules();
        }

        [DisplayName("Add Leave Rule")]
        [HttpPost]
        public IActionResult PostLeavePolicy([FromBody] LeaveRules policy)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _repository.Leave.CreateLeaveRuleAndSave(policy);

            return CreatedAtAction("GetLeavePolicies", new { id = policy.Id}, policy);
        }
    }
}
