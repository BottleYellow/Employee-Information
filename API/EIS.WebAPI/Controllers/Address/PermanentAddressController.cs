using EIS.Entities.Address;
using EIS.Repositories.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace EIS.WebAPI.Controllers
{
    [Route("api/PermanentAddress")]
    [ApiController]
    public class PermanentAddressController : Controller
    {
        public readonly IRepositoryWrapper _repository;
        public PermanentAddressController(IRepositoryWrapper repository)
        {
            _repository = repository;
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

            var Permanent = _repository.PermanentAddress.FindByCondition(addr=>addr.PersonId==id);

            if (Permanent == null)
            {
                return NotFound();
            }

            return Ok(Permanent);
        }

        // PUT: api/Permanents/5
        [HttpPut]
        public IActionResult PutPermanent([FromBody] Permanent permanent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _repository.PermanentAddress.Update(permanent);
           
            try
            {          
                _repository.PermanentAddress.Save();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return StatusCode(500, ex.Message);
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

            var permanent = _repository.PermanentAddress.FindByCondition(addr => addr.Id == id);
            if (permanent == null)
            {
                return NotFound();
            }
            Other other = new Other();
            other.AddressType = "Permanent Address";
            other.PersonId = permanent.PersonId;
            other.Address = permanent.Address;
            other.City = permanent.City;
            other.State = permanent.State;
            other.Country = permanent.Country;
            other.Person = permanent.Person;
            other.PhoneNumber = permanent.PhoneNumber;
            other.PinCode = permanent.PinCode;
            other.IsActive = false;
            other.CreatedDate = permanent.CreatedDate;
            other.UpdatedDate = DateTime.Now;
            _repository.OtherAddress.Create(other);
            _repository.OtherAddress.Save();
            _repository.PermanentAddress.Delete(permanent);
            _repository.PermanentAddress.Save();
            return Ok(permanent);
        }
    }
}