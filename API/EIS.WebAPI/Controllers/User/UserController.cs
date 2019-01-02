using EIS.Entities.User;
using EIS.Repositories.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EIS.WebAPI.Controllers.User
{
    [ApiController]
    [Route("api/user")]
    public class UserController : BaseController
    {

        public UserController(IRepositoryWrapper repository) : base(repository)
        {

        }
        
        [HttpGet]
        public IEnumerable<Users> Get()
        {
            return _repository.Users.FindAll();
        }

        // GET api/<controller>/5
        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute]int id)
        {

            var user = _repository.Users.FindByCondition(e => e.Id == id);

            if (user == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(user);
            }

        }

        // POST api/<controller>
        [AllowAnonymous]
        [HttpPost]
        [Route("create")]
        public void Post([FromBody]Users user)
       {
            _repository.Users.CreateUser(user);
            _repository.Users.Save();
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
