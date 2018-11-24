using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EIS.Entities.User;
using EIS.Repositories.IRepository;
using Microsoft.AspNetCore.Mvc;

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
