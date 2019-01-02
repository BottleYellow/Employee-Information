using EIS.Entities.Address;
using EIS.Repositories.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace EIS.WebAPI.Controllers
{

    [Route("api/EmergencyAddress")]
    [ApiController]
    public class EmergencyAddressController : Controller
    {
        public readonly IRepositoryWrapper _repository;
        public EmergencyAddressController(IRepositoryWrapper repository) 
        {
            _repository = repository;
        }

        // GET: api/Emergencys
        [HttpGet]
        public IEnumerable<Emergency> GetEmergencyAddresses()
        {
            return _repository.EmergencyAddress.FindAll();
        }

        // GET: api/Emergencys/5
        [HttpGet("Get/{id}")]
        public Emergency GetEmergencyById([FromRoute] int id)
        {
            var EmergencyAddress = _repository.EmergencyAddress.FindByCondition(e=>e.Id==id);
            return EmergencyAddress;
        }

        [HttpGet("{id}")]
        public IEnumerable<Emergency> GetEmergencyByPersonId([FromRoute] int id)
        {
            var EmergencyAddresses = _repository.EmergencyAddress.FindAllByCondition(addr=>addr.PersonId==id);
            return EmergencyAddresses;
        }

        // PUT: api/Emergencys/5
        [HttpPut]
        public IActionResult PutEmergency([FromBody] Emergency emergency)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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

        // POST: api/Emergency
        [HttpPost]
        public IActionResult PostEmergency([FromBody] Emergency emergency)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _repository.EmergencyAddress.Create(emergency);
            _repository.EmergencyAddress.Save();

            return CreatedAtAction("GetEmergencyById", new { id = emergency.Id }, emergency);
        }

        // DELETE: api/Emergencys/5
        [HttpDelete("{id}")]
        public IActionResult DeleteEmergency([FromRoute] int id)
        {
            var Emergency = _repository.EmergencyAddress.FindByCondition(addr => addr.Id == id);
            if (Emergency == null)
            {
                return NotFound();
            }
            Other other = new Other
            {
                AddressType = "Emergency Address",
                PersonId = Emergency.PersonId,
                Address = Emergency.Address,
                City = Emergency.City,
                State = Emergency.State,
                Country = Emergency.Country,
                Person = Emergency.Person,
                PhoneNumber = Emergency.PhoneNumber,
                PinCode = Emergency.PinCode,
                IsActive = false,
                CreatedDate = Emergency.CreatedDate,
                UpdatedDate = DateTime.Now
            };
            _repository.OtherAddress.Create(other);
            _repository.OtherAddress.Save();
            _repository.EmergencyAddress.Delete(Emergency);
            _repository.EmergencyAddress.Save();
            return Ok(Emergency);
        }
    }
}