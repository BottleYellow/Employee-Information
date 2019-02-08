using EIS.Entities.Generic;
using EIS.Entities.Leave;
using EIS.Repositories.IRepository;
using EIS.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace EIS.WebAPI.Controllers.Leave
{
    [Route("api/LeavePolicy")]
    [ApiController]
    public class LeavePolicyController : BaseController
    {
        public LeavePolicyController(IRepositoryWrapper repository):base(repository)
        {
        }
        [Route("GetPolicies/{loc}")]
        [HttpGet]
        public IActionResult GetPolicyLocationWise(int loc)
        {
            if (loc == 0)
            {
                IQueryable<LeaveRules> policies = _repository.LeaveRules.FindAll();
                return Ok(policies);
            }
            else
            {
                IQueryable<LeaveRules> policies = _repository.LeaveRules.FindAllByCondition(e => e.LocationId == loc);
                return Ok(policies);
            }

        }
        [Route("GetPolicyByLocation/{PersonId}")]
        [HttpGet]
        public IEnumerable<LeaveRules> GetLeavePolicies([FromRoute]int PersonId)
        {
            string locationId = _repository.Employee.FindByCondition(x => x.Id == PersonId).LocationId.ToString();
            return _repository.LeaveRules.GetAllLeaveRules().Where(x => x.TenantId == TenantId && x.LocationId==Convert.ToInt32(locationId));
        }
        [DisplayName("leave Policies")]
        [HttpGet]
        public IEnumerable<LeaveRules> GetLeavePolicies()
        {
            return _repository.LeaveRules.GetAllLeaveRules().Where(x => x.TenantId == TenantId);
        }

        [DisplayName("leave Policies")]
        [Route("GetLeavePolicies")]
        [HttpPost]
        public ActionResult GetLeavePolicies([FromBody]SortGrid sortGrid)
        {
            ArrayList data = new ArrayList();
            IQueryable<LeaveRules> employeeData = null;
            if(sortGrid.LocationId==0)
            {
                employeeData = _repository.LeaveRules.GetAllLeaveRules().Where(x => x.TenantId == TenantId).Include(x=>x.Location);
            }
            else
            {
                employeeData = _repository.LeaveRules.GetAllLeaveRules().Where(x => x.TenantId == TenantId && x.LocationId == sortGrid.LocationId).Include(x => x.Location);
            }
            
            if (string.IsNullOrEmpty(sortGrid.Search))
            {

                data = _repository.LeaveRules.GetDataByGridCondition(null, sortGrid, employeeData);
            }
            else
            {
                data = _repository.LeaveRules.GetDataByGridCondition(x => x.LeaveType.ToLower().Contains(sortGrid.Search.ToLower()), sortGrid, employeeData);
            }
            return Ok(data);
        }

        [DisplayName("Add Leave Rule")]
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
