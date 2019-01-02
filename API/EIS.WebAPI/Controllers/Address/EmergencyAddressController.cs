using EIS.Entities.Address;
using EIS.Repositories.IRepository;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        public IEnumerable<Emergency> GetEmergencyAddresses()
        {
            return _repository.EmergencyAddress.FindAll();
        }

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

        [HttpPut]
        public IActionResult PutEmergency([FromBody] Emergency emergency)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _repository.EmergencyAddress.UpdateAndSave(emergency);
            return Ok(emergency);
        }

        [HttpPost]
        public IActionResult PostEmergency([FromBody] Emergency emergency)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _repository.EmergencyAddress.CreateAndSave(emergency);
            return CreatedAtAction("GetEmergencyById", new { id = emergency.Id }, emergency);
        }

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
            _repository.OtherAddress.CreateAndSave(other);
            _repository.EmergencyAddress.DeleteAndSave(Emergency);
            return Ok(Emergency);
        }
    }
}