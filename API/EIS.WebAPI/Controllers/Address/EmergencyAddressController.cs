using EIS.Entities.Address;
using EIS.Repositories.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace EIS.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmergencyAddressController : BaseController
    {
        public EmergencyAddressController(IRepositoryWrapper repository) : base(repository)
        {
        }

        // GET: api/Emergencys
        [HttpGet]
        public IEnumerable<Emergency> GetEmergencyAddresses()
        {
            return _repository.EmergencyAddress.FindAll();
        }

        // GET: api/Emergencys/5
        [HttpGet("{id}")]
        public IActionResult GetEmergency([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var Emergency = _repository.EmergencyAddress.FindByCondition(addr=>addr.Id==id);

            if (Emergency == null)
            {
                return NotFound();
            }

            return Ok(Emergency);
        }

        // PUT: api/Emergencys/5
        [HttpPut("{id}")]
        public IActionResult PutEmergency([FromRoute] int id, [FromBody] Emergency emergency)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != emergency.Id)
            {
                return BadRequest();
            }

            _repository.EmergencyAddress.Update(emergency);
           
            try
            {          
                _repository.Employee.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
               
            }

            return NoContent();
        }

        // POST: api/Emergencys
        [HttpPost]
        public IActionResult PostEmergency([FromBody] Emergency emergency)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _repository.EmergencyAddress.Create(emergency);
            _repository.EmergencyAddress.Save();

            return CreatedAtAction("GetEmergency", new { id = emergency.Id }, emergency);
        }

        // DELETE: api/Emergencys/5
        [HttpDelete("{id}")]
        public IActionResult DeleteEmergency([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var Emergency = _repository.EmergencyAddress.FindByCondition(addr => addr.Id == id);
            if (Emergency == null)
            {
                return NotFound();
            }

            _repository.EmergencyAddress.Delete(Emergency);
            _repository.EmergencyAddress.Save();
            return Ok(Emergency);
        }
    }
}