using EIS.Entities.Address;
using EIS.Repositories.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace EIS.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrentAddressController : BaseController
    {
        public CurrentAddressController(IRepositoryWrapper repository) : base(repository)
        {
        }

        // GET: api/Currents
        [HttpGet]
        public IEnumerable<Current> GetCurrentAddresses()
        {
            return _repository.CurrentAddress.FindAll();
        }

        // GET: api/Currents/5
        [HttpGet("{id}")]
        public IActionResult GetCurrent([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var Current = _repository.CurrentAddress.FindByCondition(addr=>addr.Id==id);

            if (Current == null)
            {
                return NotFound();
            }

            return Ok(Current);
        }

        // PUT: api/Currents/5
        [HttpPut("{id}")]
        public IActionResult PutCurrent([FromRoute] int id, [FromBody] Current current)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != current.Id)
            {
                return BadRequest();
            }

            _repository.CurrentAddress.Update(current);
           
            try
            {          
                _repository.Employee.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
               
            }

            return NoContent();
        }

        // POST: api/Currents
        [HttpPost]
        public IActionResult PostCurrent([FromBody] Current current)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _repository.CurrentAddress.Create(current);
            _repository.CurrentAddress.Save();

            return CreatedAtAction("GetCurrent", new { id = current.Id }, current);
        }

        // DELETE: api/Currents/5
        [HttpDelete("{id}")]
        public IActionResult DeleteCurrent([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var Current = _repository.CurrentAddress.FindByCondition(addr => addr.Id == id);
            if (Current == null)
            {
                return NotFound();
            }

            _repository.CurrentAddress.Delete(Current);
            _repository.CurrentAddress.Save();
            return Ok(Current);
        }
    }
}