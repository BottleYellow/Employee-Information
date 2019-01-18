using EIS.Entities.Generic;
using EIS.Entities.Leave;
using EIS.Repositories.IRepository;
using EIS.WebAPI.RedisCache;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace EIS.WebAPI.Controllers.Leave
{
    [Route("api/LeavePolicy")]
    [ApiController]
    public class LeavePolicyController : BaseController
    {
        public LeavePolicyController(IRepositoryWrapper repository): base(repository)
        {
        }

        [DisplayName("leave Policies")]
        [HttpPost]
        public ActionResult GetLeavePolicies([FromBody]SortGrid sortGrid)
        {
                ArrayList data = new ArrayList();
            var employeeData = _repository.LeaveRules.GetAllLeaveRules();
            if (string.IsNullOrEmpty(sortGrid.Search))
                {

                    data = _repository.LeaveRules.GetDataByGridCondition(null, sortGrid, employeeData);
                }
                else
                {
                    data = _repository.LeaveRules.GetDataByGridCondition(x => x.LeaveType== sortGrid.Search, sortGrid, employeeData);
                }
                return Ok(data);
        }

        [DisplayName("Add Leave Rule")]
        [Route("Leave")]
        [HttpPost]
        public IActionResult PostLeavePolicy([FromBody] LeaveRules policy)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            policy.TenantId = TenantId;
            _repository.LeaveRules.CreateLeaveRuleAndSave(policy);

            return CreatedAtAction("GetLeavePolicies", new { id = policy.Id}, policy);
        }
    }
}
