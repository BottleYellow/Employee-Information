using EIS.Entities.User;
using EIS.Repositories.IRepository;
using EIS.WebAPI.Services;
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
        public UserController(IRepositoryWrapper repository) : base(repository)
        {

        }

        [DisplayName("List Of Users")]
        [HttpGet]
        public IEnumerable<Users> Get()
        {
            return _repository.Users.FindAllByCondition(x=>x.TenantId==TenantId);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute]int id)
        {
            Users user = _repository.Users.FindByCondition(e => e.PersonId == id);
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
