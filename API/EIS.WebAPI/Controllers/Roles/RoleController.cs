using System;
using System.Collections.Generic;
using System.Linq;
using EIS.Entities.User;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EIS.Repositories.IRepository;
using EIS.WebAPI.Filters;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EIS.WebAPI.Controllers.Roles
{
    //[TypeFilter(typeof(Authorization))]
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
        public IActionResult GetAllRoles()
        {
            var roles = _repository.RoleManager.FindAll();
            return Ok(roles);
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute]int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var role = _repository.RoleManager.FindByCondition(e => e.Id == id);

            if (role == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(role);
            }

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

        [HttpPut("{id}")]
        public IActionResult UpdateData([FromRoute]int id, [FromBody]Role role)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != role.Id)
            {
                return BadRequest();
            }
            _repository.RoleManager.Update(role);
            try
            {
                _repository.RoleManager.Save();
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                throw;
            }

            return NoContent();
        }


        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
