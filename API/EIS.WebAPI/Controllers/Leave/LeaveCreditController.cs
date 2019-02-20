﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using EIS.Entities.Generic;
using EIS.Entities.Leave;
using EIS.Repositories.IRepository;
using EIS.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EIS.WebAPI.Controllers.Leave
{

    [Route("api/LeaveCredit")]
    [ApiController]
    public class LeaveCreditController : BaseController
    {
        public LeaveCreditController(IRepositoryWrapper repository):base(repository)
        {

        }

        [DisplayName("leave Credits")]
        [HttpPost]
        [Route("GetLeaveCredits")]
        public ActionResult Get([FromBody]SortGrid sortGrid)
        {
        
            IQueryable<LeaveCredit> credits = sortGrid.LocationId==0? _repository.LeaveCredit.GetCredits().Include(x => x.Person.Location).Where(x => x.TenantId == TenantId && x.IsActive == true && x.Person.Location.IsActive == true):
                _repository.LeaveCredit.GetCredits().Include(x => x.Person.Location).Where(x => x.TenantId == TenantId && x.IsActive == true && x.Person.LocationId == sortGrid.LocationId && x.Person.Location.IsActive == true);

            ArrayList data = string.IsNullOrEmpty(sortGrid.Search)? _repository.LeaveCredit.GetDataByGridCondition(null, sortGrid, credits):
                _repository.LeaveCredit.GetDataByGridCondition(x => x.Person.Location.LocationName.ToLower().Contains(sortGrid.Search.ToLower()) || x.Person.FullName.ToLower().Contains(sortGrid.Search.ToLower()) || x.LeaveType.ToLower().Contains(sortGrid.Search.ToLower()), sortGrid, credits);           
            return Ok(data);
        }

        // GET api/<controller>/5
        [Route("GetCreditById/{Id}")]
        [HttpGet]
        public LeaveCredit GetCreditById([FromRoute]int Id)
        {
            return _repository.LeaveCredit.FindByCondition(x => x.Id == Id);
        }

        [DisplayName("Add Leave Credit")]
        [Route("AddCredits")]
        [HttpPost]
        public IActionResult PostLeaveCredits([FromBody] LeaveRules Leave)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Leave.TenantId = TenantId;
            _repository.LeaveCredit.AddCreditsAndSave(Leave);
            _repository.LeaveCredit.Dispose();
            return Ok();
        }

        [DisplayName("Add Leave Credit")]
        [HttpPost("{Id}")]
        public IActionResult PostLeaveCredit([FromRoute]int Id,[FromBody] LeaveCredit Credit)
        {
            if (Id == 0)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                Credit.TenantId = TenantId;
                _repository.LeaveCredit.AddCreditAndSave(Credit);
                _repository.LeaveCredit.Dispose();
                return Ok();
            }
            else
            {
                LeaveCredit leaveCredit = _repository.LeaveCredit.FindByCondition(x => x.Id == Credit.Id);
                float diff = leaveCredit.AllotedDays - leaveCredit.Available;
                Credit.TenantId = TenantId;
                Credit.Available = Credit.AllotedDays - diff;
                _repository.LeaveCredit.UpdateAndSave(Credit);
                return Ok(Credit);
            }
        }

        [Route("CreditDelete/{id}")]
        [HttpPost]
        public IActionResult Delete([FromRoute]int id)
        {
            LeaveCredit credit = _repository.LeaveCredit.FindByCondition(x => x.Id == id);
            if (credit == null)
            {
                return NotFound();
            }
            credit.IsActive = false;
            _repository.LeaveCredit.UpdateAndSave(credit);
            _repository.LeaveCredit.Dispose();
            return Ok(credit);
        }
    }
}
