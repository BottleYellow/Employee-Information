using EIS.Entities.Address;
using EIS.Repositories.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace EIS.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermanentAddressController : BaseController
    {
        public PermanentAddressController(IRepositoryWrapper repository) : base(repository)
        {
        }

        // GET: api/Permanents
        [HttpGet]
        public IEnumerable<Permanent> GetPermanentAddresses()
        {
            return _repository.PermanentAddress.FindAll();
        }

        // GET: api/Permanents/5
        [HttpGet("{id}")]
        public IActionResult GetPermanent([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var Permanent = _repository.PermanentAddress.FindByCondition(addr=>addr.Id==id);

            if (Permanent == null)
            {
                return NotFound();
            }

            return Ok(Permanent);
        }

        // PUT: api/Permanents/5
        [HttpPut("{id}")]
        public IActionResult PutPermanent([FromRoute] int id, [FromBody] Permanent permanent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != permanent.Id)
            {
                return BadRequest();
            }

            _repository.PermanentAddress.Update(permanent);
           
            try
            {          
                _repository.Employee.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
               
            }

            return NoContent();
        }

        // POST: api/Permanents
        [HttpPost]
        public IActionResult PostPermanent([FromBody] Permanent permanent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _repository.PermanentAddress.Create(permanent);
            _repository.PermanentAddress.Save();

            return CreatedAtAction("GetPermanent", new { id = permanent.Id }, permanent);
        }

        // DELETE: api/Permanents/5
        [HttpDelete("{id}")]
        public IActionResult DeletePermanent([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var Permanent = _repository.PermanentAddress.FindByCondition(addr => addr.Id == id);
            if (Permanent == null)
            {
                return NotFound();
            }

            _repository.PermanentAddress.Delete(Permanent);
            _repository.PermanentAddress.Save();
            return Ok(Permanent);
        }
    }
}