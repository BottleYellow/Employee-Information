using EIS.Entities.Address;
using EIS.Repositories.IRepository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;

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

        [HttpGet]
        public IEnumerable<Current> GetCurrentAddresses()
        {
            return _repository.CurrentAddress.FindAll();
        }
        [DisplayName("Profile view")]
        [HttpGet("{id}")]
        public IActionResult GetCurrent([FromRoute] int id)
        {
            var current = _repository.CurrentAddress.FindByCondition(addr=>addr.PersonId==id);
            if (current == null)
                return NotFound();
            return Ok(current);
        }
        [DisplayName("Update Current Address")]
        [HttpPut]
        public IActionResult PutCurrent([FromBody] Current current)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _repository.CurrentAddress.UpdateAndSave(current);          
            return Ok(current);
        }
        [DisplayName("Add Current Address")]
        [HttpPost]
        public IActionResult PostCurrent([FromBody] Current current)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _repository.CurrentAddress.CreateAndSave(current);
            return CreatedAtAction("GetCurrent", new { id = current.Id }, current);
        }
        [DisplayName("Delete Current Address")]
        [HttpDelete("{id}")]
        public IActionResult DeleteCurrent([FromRoute] int id)
        {
            var current = _repository.CurrentAddress.FindByCondition(addr => addr.Id == id);
            if (current == null)
            {
                return NotFound();
            }
            Other other = new Other
            {
                AddressType = "Current Address",
                PersonId = current.PersonId,
                Address = current.Address,
                City = current.City,
                State = current.State,
                Country = current.Country,
                Person = current.Person,
                PhoneNumber = current.PhoneNumber,
                PinCode = current.PinCode,
                IsActive = false,
                CreatedDate = current.CreatedDate,
                UpdatedDate = DateTime.Now
            };
            _repository.OtherAddress.CreateAndSave(other);
            _repository.CurrentAddress.DeleteAndSave(current);
            return Ok(current);
        }
    }
}