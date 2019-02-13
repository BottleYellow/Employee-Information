using EIS.Entities.Address;
using EIS.Repositories.IRepository;
using EIS.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace EIS.WebAPI.Controllers
{

    [Route("api/EmergencyAddress")]
    [ApiController]
    public class EmergencyAddressController : BaseController
    {
        public EmergencyAddressController(IRepositoryWrapper repository) :base(repository)
        {
        }

        [HttpGet]
        public IEnumerable<Emergency> GetEmergencyAddresses()
        {
            return _repository.EmergencyAddress.FindAll();
        }

        [HttpGet("Get/{id}")]
        public Emergency GetEmergencyById([FromRoute] int id)
        {
            Emergency EmergencyAddress = _repository.EmergencyAddress.FindByCondition(e=>e.Id==id);
            return EmergencyAddress;
        }
        [DisplayName("Profile view")]
        [HttpGet("{id}")]
        public IEnumerable<Emergency> GetEmergencyByPersonId([FromRoute] int id)
        {
            IEnumerable<Emergency> EmergencyAddresses = _repository.EmergencyAddress.FindAllByCondition(addr=>addr.PersonId==id).ToList();
            return EmergencyAddresses;
        }

        [DisplayName("Update Emergency Address")]
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

        [Route("AddEmergency/{id}")]
        [HttpPost]
        public IActionResult PostEmergency([FromRoute]int id,[FromBody] Emergency emergency)
        {
            if (id == 0)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                emergency.TenantId = TenantId;
                _repository.EmergencyAddress.CreateAndSave(emergency);
                return CreatedAtAction("GetEmergencyById", new { id = emergency.Id }, emergency);
            }
            else
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                _repository.EmergencyAddress.UpdateAndSave(emergency);
                return Ok(emergency);
            }
        }

        [Route("DeleteEmergency/{id}")]
        [HttpPost("{id}")]
        public IActionResult DeleteEmergency([FromRoute] int id)
        {
            Emergency Emergency = _repository.EmergencyAddress.FindByCondition(addr => addr.Id == id);
            if (Emergency == null)
            {
                return NotFound();
            }
            Other other = new Other
            {
                TenantId = TenantId,
                AddressType = "Emergency Address",
                PersonId = Emergency.PersonId,
                Address = Emergency.FirstName+" "+Emergency.LastName+" ADDRESS:-"+Emergency.Address,
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