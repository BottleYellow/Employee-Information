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
        [Route("GetPolicyById/{Id}")]
        [HttpGet]
        public LeaveRules GetPolicyById([FromRoute]int Id)
        {
            return _repository.LeaveRules.FindByCondition(x => x.Id == Id);
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
                employeeData = _repository.LeaveRules.GetAllLeaveRules().Include(x => x.Location).Where(x => x.TenantId == TenantId && x.Location.IsActive == true);
            }
            else
            {
                employeeData = _repository.LeaveRules.GetAllLeaveRules().Include(x => x.Location).Where(x => x.TenantId == TenantId && x.Location.IsActive == true && x.LocationId == sortGrid.LocationId);
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
        [HttpPost("{Id}")]
        public IActionResult PostLeavePolicy([FromRoute] int Id,[FromBody] LeaveRules policy)
        {
            if (Id == 0)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                policy.TenantId = TenantId;
                _repository.LeaveRules.CreateLeaveRuleAndSave(policy);

                return CreatedAtAction("GetLeavePolicies", new { id = policy.Id }, policy);
            }
            else
            {
                policy.TenantId = TenantId;
                _repository.LeaveRules.UpdateAndSave(policy);
                List<LeaveCredit> Credits = _repository.LeaveCredit.FindAllByCondition(x => x.LeaveId == policy.Id).ToList();
                foreach (var Credit in Credits)
                {
                    float diff = Credit.AllotedDays - Credit.Available;
                    Credit.LeaveType = policy.LeaveType;
                    Credit.ValidFrom = policy.ValidFrom;
                    Credit.ValidTo = policy.ValidTo;
                    Credit.AllotedDays = policy.Validity;
                    Credit.Available = Credit.AllotedDays - diff;
                    Credit.UpdatedDate = policy.UpdatedDate;
                    Credit.UpdatedBy = policy.UpdatedBy;
                    _repository.LeaveCredit.UpdateAndSave(Credit);
                }
                return Ok(policy);
            }
        }
    }
}
