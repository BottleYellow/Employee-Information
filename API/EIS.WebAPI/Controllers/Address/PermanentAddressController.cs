using EIS.Entities.Address;
using EIS.Repositories.IRepository;
using EIS.WebAPI.RedisCache;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace EIS.WebAPI.Controllers
{ 
    [Route("api/PermanentAddress")]
    [ApiController]
    public class PermanentAddressController : BaseController
    {
        public PermanentAddressController(IRepositoryWrapper repository): base(repository)
        {
        }
        
        [HttpGet]
        public IEnumerable<Permanent> GetPermanentAddresses()
        {
            return _repository.PermanentAddress.FindAll();
        }
        [DisplayName("Profile view")]
        [HttpGet("{id}")]
        public IActionResult GetPermanent([FromRoute] int id)
        {
            var Permanent = _repository.PermanentAddress.FindByCondition(addr=>addr.PersonId==id);
            if (Permanent == null)
                return NotFound();
            return Ok(Permanent);
        }

        [DisplayName("Update Permanent Address")]
        [HttpPut]
        public IActionResult PutPermanent([FromBody] Permanent permanent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _repository.PermanentAddress.UpdateAndSave(permanent);
            return Ok(permanent);
        }

        [DisplayName("Add Permanent Address")]
        [HttpPost]
        public IActionResult PostPermanent([FromBody] Permanent permanent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            permanent.TenantId = TenantId;
            _repository.PermanentAddress.CreateAndSave(permanent);
            return CreatedAtAction("GetPermanent", new { id = permanent.Id }, permanent);
        }

        [DisplayName("Delete Permanent Address")]
        [HttpDelete("{id}")]
        public IActionResult DeletePermanent([FromRoute] int id)
        {
            var permanent = _repository.PermanentAddress.FindByCondition(addr => addr.Id == id);
            if (permanent == null)
            {
                return NotFound();
            }
            Other other = new Other
            {
                TenantId=TenantId,
                AddressType = "Permanent Address",
                PersonId = permanent.PersonId,
                Address = permanent.Address,
                City = permanent.City,
                State = permanent.State,
                Country = permanent.Country,
                Person = permanent.Person,
                PhoneNumber = permanent.PhoneNumber,
                PinCode = permanent.PinCode,
                IsActive = false,
                CreatedDate = permanent.CreatedDate,
                UpdatedDate = DateTime.Now
            };
            _repository.OtherAddress.CreateAndSave(other);
            _repository.PermanentAddress.DeleteAndSave(permanent);
            return Ok(permanent);
        }
    }
}