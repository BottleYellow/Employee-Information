using EIS.Entities.Address;
using EIS.Repositories.IRepository;
using Microsoft.AspNetCore.Mvc;
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
                return NotFound();
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
            _repository.CurrentAddress.UpdateAndSave(current);          
            return Ok(current);
        }

        // POST: api/Currents
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

        // DELETE: api/Currents/5
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