using System.Collections.Generic;
using EIS.Entities.Admin;
using EIS.Repositories.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EIS.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginsController : BaseController
    {
        public LoginsController(IRepositoryWrapper repository) : base(repository)
        {
        }


        // GET: api/Logins
        [HttpGet]
        public IEnumerable<Login> GetLogins()
        {
            return _repository.Login.FindAll();
        }

        // GET: api/Logins/5
        [HttpGet("{id}")]
        public IActionResult GetLogin([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var login = _repository.Login.FindByCondition(x => x.Id == id);

            if (login == null)
            {
                return NotFound();
            }

            return Ok(login);
        }

        // PUT: api/Logins/5
        [HttpPut("{id}")]
        public IActionResult PutLogin([FromRoute] int id, [FromBody] Login login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != login.Id)
            {
                return BadRequest();
            }
            _repository.Login.Update(login);

            try
            {
                _repository.Login.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                
                    throw;
                
            }

            return NoContent();
        }

        // POST: api/Logins
        [HttpPost]
        public IActionResult PostLogin([FromBody] Login login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _repository.Login.Create(login);
            _repository.Login.Save();
            return CreatedAtAction("GetLogin", new { id = login.Id }, login);
        }

        // DELETE: api/Logins/5
        [HttpDelete("{id}")]
        public IActionResult DeleteLogin([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var login = _repository.Login.FindByCondition(x => x.Id == id);
            if (login == null)
            {
                return NotFound();
            }
            _repository.Login.Delete(login);
            _repository.Login.Save();
            return Ok(login);
        }
    }
}