using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EIS.Entities.Leave;
using EIS.Repositories.IRepository;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EIS.WebAPI.Controllers.Leave
{
    [Route("api/LeaveCredit")]
    [ApiController]
    public class LeaveCreditController : Controller
    {
        public readonly IRepositoryWrapper _repository;
        public LeaveCreditController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }
        [HttpGet]
        public IEnumerable<LeaveCredit> Get()
        {
            return _repository.Leave.GetCredits();
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }
        [Route("AddCredits")]
        [HttpPost]
        public IActionResult PostLeaveCredits([FromBody] LeaveMaster Leave)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _repository.Leave.AddCredits(Leave);
            _repository.Leave.Save();

            return Ok();
        }
        [HttpPost]
        public IActionResult PostLeaveCredit([FromBody] LeaveCredit Credit)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _repository.Leave.AddCredit(Credit);
            _repository.Leave.Save();

            return Ok();
        }
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
