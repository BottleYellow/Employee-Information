using EIS.Entities.Address;
using EIS.Repositories.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace EIS.WebAPI.Controllers
{
    [Route("api/CurrentAddress")]
    [ApiController]
    public class CurrentAddressController : Controller
    {
        public readonly IRepositoryWrapper _repository;
        public CurrentAddressController(IRepositoryWrapper repository)
        {
            _repository = repository;
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
            var current = _repository.CurrentAddress.FindByCondition(addr=>addr.PersonId==id);

            if (current == null)
            {
                return NotFound();
            }

            return Ok(current);
        }

        // PUT: api/Currents/5
        [HttpPut]
        public IActionResult PutCurrent([FromBody] Current current)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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
            var current = _repository.CurrentAddress.FindByCondition(addr => addr.Id == id);
            if (current == null)
            {
                return NotFound();
            }
            Other other = new Other();
            other.AddressType = "Current Address";
            other.PersonId = current.PersonId;
            other.Address = current.Address;
            other.City = current.City;
            other.State = current.State;
            other.Country = current.Country;
            other.Person = current.Person;
            other.PhoneNumber = current.PhoneNumber;
            other.PinCode = current.PinCode;
            other.IsActive = false;
            other.CreatedDate = current.CreatedDate;
            other.UpdatedDate = DateTime.Now;
            _repository.OtherAddress.Create(other);
            _repository.OtherAddress.Save();
            _repository.CurrentAddress.Delete(current);
            _repository.CurrentAddress.Save();
            return Ok(current);
        }
    }
}