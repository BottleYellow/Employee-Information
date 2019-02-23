using EIS.Entities.Generic;
using EIS.Entities.Leave;
using EIS.Entities.Models;
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
        public LeavePolicyController(IRepositoryWrapper repository) : base(repository)
        {
        }
        [Route("GetPolicies/{loc}")]
        [HttpGet]
        public IActionResult GetPolicyLocationWise(int loc)
        {
            IQueryable<LeaveRules> policies = loc == 0 ? _repository.LeaveRules.FindAll().Where(x => x.IsActive == true) :
                                                         _repository.LeaveRules.FindAllByCondition(e => e.LocationId == loc && e.IsActive == true);
            return Ok(policies);


        }
        [Route("GetPolicyByLocation/{PersonId}")]
        [HttpGet]
        public IEnumerable<LeaveRules> GetLeavePolicies([FromRoute]int PersonId)
        {
            string locationId = _repository.Employee.FindByCondition(x => x.Id == PersonId).LocationId.ToString();
            return _repository.LeaveRules.FindAll().Where(x => x.TenantId == TenantId && x.IsActive == true && x.LocationId == Convert.ToInt32(locationId));
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
            return _repository.LeaveRules.FindAll().Where(x => x.TenantId == TenantId);
        }

        [DisplayName("leave Policies")]
        [Route("GetLeavePolicies/{LocationId}")]
        public ActionResult LeavePolicies([FromRoute]int LocationId)
        {          
            IEnumerable<LeavePolicyViewModel> employeeData = LocationId == 0?  _repository.LeaveRules.FindAll().Include(x => x.Location).Where(x => x.TenantId == TenantId && x.IsActive == true && x.Location.IsActive == true).Select(x=>new LeavePolicyViewModel
                {Id=x.Id,
                ActiveStatus=x.IsActive,
                LeaveType=x.LeaveType,
                LocationName=x.Location.LocationName,
                ValidFrom=x.ValidFrom,
                ValidTo=x.ValidTo,
                Validity=x.Validity
            }).ToList():
            _repository.LeaveRules.FindAll().Include(x => x.Location).Where(x => x.TenantId == TenantId && x.IsActive == true && x.Location.IsActive == true && x.LocationId == LocationId).Select(x=>new LeavePolicyViewModel
            {Id = x.Id,
            ActiveStatus = x.IsActive,
            LeaveType = x.LeaveType,
            LocationName = x.Location.LocationName,
            ValidFrom = x.ValidFrom,
            ValidTo = x.ValidTo,
            Validity = x.Validity
            }).ToList();
             return Ok(employeeData);
        }

        [DisplayName("Add Leave Rule")]
        [HttpPost("{Id}")]
        public IActionResult PostLeavePolicy([FromRoute] int Id, [FromBody] LeaveRules policy)
        {
            if (Id == 0)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                policy.TenantId = TenantId;
                _repository.LeaveRules.CreateLeaveRuleAndSave(policy);
                _repository.LeaveRules.Dispose();
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
                _repository.LeaveRules.Dispose();
                return Ok(policy);
            }
        }
        [Route("PolicyDelete/{id}")]
        [HttpPost]
        public IActionResult Delete([FromRoute]int id)
        {
            LeaveRules policy = _repository.LeaveRules.FindByCondition(x => x.Id == id);
            List<LeaveCredit> credits = _repository.LeaveCredit.FindAllByCondition(x => x.LeaveId == id).ToList();
            if (policy == null)
            {
                return NotFound();
            }
            policy.IsActive = false;
            _repository.LeaveRules.UpdateAndSave(policy);
            foreach (var Credit in credits)
            {
                Credit.IsActive = false;
                _repository.LeaveCredit.UpdateAndSave(Credit);
            }
            _repository.LeaveRules.Dispose();
            return Ok(policy);
        }

    }
}
