﻿using System;
using System.Collections.Generic;
using System.Linq;
using EIS.Entities.User;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EIS.Repositories.IRepository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EIS.WebAPI.Controllers.Roles
{
    [Route("api/role")]
    [ApiController]
    public class RoleController : Controller
    {
        public readonly IRepositoryWrapper _repository;
        public RoleController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }
        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [HttpPost]
        public IActionResult Create([FromBody]Role role)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _repository.RoleManager.CreateRole(role);
            _repository.RoleManager.Save();
            return Ok();
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}