using EIS.Entities.User;
using EIS.Repositories.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            return _repository.Users.FindAll();
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute]int id)
        {
            var user = _repository.Users.FindByCondition(e => e.Id == id);
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
            _repository.Users.CreateUserAndSave(user);
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
