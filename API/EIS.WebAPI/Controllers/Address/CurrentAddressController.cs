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
    public class CurrentAddressController : BaseController
    {
        public CurrentAddressController(IRepositoryWrapper repository): base(repository)
        {
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
            Current currentAddress = _repository.CurrentAddress.FindByCondition(addr=>addr.PersonId==id);
            _repository.CurrentAddress.Dispose();
            if (currentAddress == null)
                return NotFound();
            return Ok(currentAddress);
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
            _repository.CurrentAddress.Dispose();
            return Ok(current);
        }
        [Route("AddCurrent/{id}")]
        [HttpPost]
        public IActionResult PostCurrent([FromRoute]int id,[FromBody] Current current)
        {
            if (id == 0)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                current.TenantId = TenantId;
                _repository.CurrentAddress.CreateAndSave(current);
                _repository.CurrentAddress.Dispose();
                return CreatedAtAction("GetCurrent", new { id = current.Id }, current);
            }
            else
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                _repository.CurrentAddress.UpdateAndSave(current);
                _repository.CurrentAddress.Dispose();
                return Ok(current);
            }
        }
        [Route("DeleteCurrent/{id}")]
        [HttpPost]
        public IActionResult DeleteCurrent([FromRoute] int id)
        {
            Current currentAddress = _repository.CurrentAddress.FindByCondition(addr => addr.Id == id);
            if (currentAddress == null)
            {
                return NotFound();
            }
            Other other = new Other
            {
                TenantId=TenantId,
                AddressType = "Current Address",
                PersonId = currentAddress.PersonId,
                Address = currentAddress.Address,
                City = currentAddress.City,
                State = currentAddress.State,
                Country = currentAddress.Country,
                Person = currentAddress.Person,
                PhoneNumber = currentAddress.PhoneNumber,
                PinCode = currentAddress.PinCode,
                IsActive = false,
                CreatedDate = currentAddress.CreatedDate,
                UpdatedDate = DateTime.Now
            };
            _repository.OtherAddress.CreateAndSave(other);
            _repository.CurrentAddress.DeleteAndSave(currentAddress);
            _repository.CurrentAddress.Dispose();
            return Ok(currentAddress);
        }
    }
}