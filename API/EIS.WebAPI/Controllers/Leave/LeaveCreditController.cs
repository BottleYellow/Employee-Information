﻿using System.Collections.Generic;
using System.ComponentModel;
using EIS.Entities.Leave;
using EIS.Repositories.IRepository;
using Microsoft.AspNetCore.Mvc;

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

        [DisplayName("leave Credits")]
        [HttpGet]
        public IEnumerable<LeaveCredit> Get()
        {
            var credits= _repository.Leave.GetCredits();
            return credits;
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
            _repository.Leave.AddCreditsAndSave(Leave);

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
            _repository.Leave.AddCreditAndSave(Credit);

            return Ok();
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}