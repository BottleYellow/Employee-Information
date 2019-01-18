using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using EIS.Entities.Generic;
using EIS.Entities.Leave;
using EIS.Repositories.IRepository;
using EIS.WebAPI.RedisCache;
using Microsoft.AspNetCore.Mvc;

namespace EIS.WebAPI.Controllers.Leave
{

    [Route("api/LeaveCredit")]
    [ApiController]
    public class LeaveCreditController : BaseController
    {
        public LeaveCreditController(IRepositoryWrapper repository): base(repository)
        {

        }

        [DisplayName("leave Credits")]
        [HttpPost]            
            public ActionResult Get([FromBody]SortGrid sortGrid)
            {
                ArrayList data = new ArrayList();
            var credits = _repository.LeaveCredit.GetCredits();

                if (string.IsNullOrEmpty(sortGrid.Search))
                {

                    data = _repository.LeaveCredit.GetDataByGridCondition(null, sortGrid, credits);
                }
                else
                {
                    data = _repository.LeaveCredit.GetDataByGridCondition(x => x.LeaveType == sortGrid.Search, sortGrid, credits);
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
        [HttpPost("PostLeaveCredit")]
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
