using System;
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
            ArrayList data = new ArrayList();
            IQueryable<LeaveCredit> credits = null;
            if(sortGrid.LocationId==0)
            {
                credits = _repository.LeaveCredit.GetCredits().Where(x => x.TenantId == TenantId).Include(x=>x.Person.Location);
            }
            else
            {
                credits = _repository.LeaveCredit.GetCredits().Where(x => x.TenantId == TenantId && x.Person.LocationId==sortGrid.LocationId).Include(x => x.Person.Location);
            }
            

            if (string.IsNullOrEmpty(sortGrid.Search))
            {

                data = _repository.LeaveCredit.GetDataByGridCondition(null, sortGrid, credits);
            }
            else
            {
                data = _repository.LeaveCredit.GetDataByGridCondition(x =>x.Person.FullName.ToLower().Contains(sortGrid.Search.ToLower())||x.LeaveType.ToLower().Contains(sortGrid.Search.ToLower()), sortGrid, credits);
            }
            return Ok(data);
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
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

            return Ok();
        }

        [DisplayName("Add Leave Credit")]
        [HttpPost]
        public IActionResult PostLeaveCredit([FromBody] LeaveCredit Credit)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Credit.TenantId = TenantId;
            _repository.LeaveCredit.AddCreditAndSave(Credit);

            return Ok();
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
