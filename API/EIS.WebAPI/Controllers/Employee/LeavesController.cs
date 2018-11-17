using EIS.Entities.Employee;
using EIS.Repositories.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace EIS.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeavesController : BaseController
    {
        public LeavesController(IRepositoryWrapper repository) : base(repository)
        {
        }

        // GET: api/Leaves
        [HttpGet]
        public IEnumerable<Leaves> GetLeaves()
        {
            return _repository.Leave.FindAll();
        }

        // GET: api/Leaves/5
        [HttpGet("{id}")]
        public IActionResult GetLeave([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var leave = _repository.Leave.FindByCondition(x => x.Id == id);

            if (leave == null)
            {
                return NotFound();
            }

            return Ok(leave);
        }

        // PUT: api/Leaves/5
        [HttpPut("{id}")]
        public IActionResult PutLeave([FromRoute] int id, [FromBody] Leaves leave)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != leave.Id)
            {
                return BadRequest();
            }
            _repository.Leave.Update(leave);

            try
            {
                _repository.Leave.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;               
            }

            return NoContent();
        }

        // POST: api/Leaves
        [HttpPost]
        public IActionResult PostLeave([FromBody] Leaves leave)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _repository.Leave.Create(leave);
            _repository.Leave.Save();

            return CreatedAtAction("GetLeave", new { id = leave.Id }, leave);
        }

        // DELETE: api/Leaves/5
        [HttpDelete("{id}")]
        public IActionResult DeleteLeave([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var leave = _repository.Leave.FindByCondition(x => x.Id == id);
            if (leave == null)
            {
                return NotFound();
            }
            _repository.Leave.Delete(leave);
            _repository.Leave.Save();
            return Ok(leave);
        }
    }
}