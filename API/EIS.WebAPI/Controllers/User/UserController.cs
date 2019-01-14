﻿using EIS.Entities.User;
using EIS.Repositories.IRepository;
using EIS.WebAPI.RedisCache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace EIS.WebAPI.Controllers.User
{
    [DisplayName("User Management")]
    [ApiController]
    [Route("api/user")]
    public class UserController : BaseController
    {
        RedisAgent Cache = new RedisAgent();
        int TenantId = 0;
        public UserController(IRepositoryWrapper repository) : base(repository)
        {
            TenantId = Convert.ToInt32(Cache.GetStringValue("TenantId"));
        }
        [DisplayName("List Of Users")]
        [HttpGet]
        public IEnumerable<Users> Get()
        {
            return _repository.Users.FindAll();
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute]int id)
        {
            var user = _repository.Users.FindByCondition(e => e.PersonId == id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("create")]
        public void Post([FromBody]Users user)
        {
            user.TenantId = TenantId;
            _repository.Users.CreateUserAndSave(user);
        }
        [AllowAnonymous]
        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody]Users user)
        {
           
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _repository.Users.UpdateAndSave(user);
            return Ok(user);
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
