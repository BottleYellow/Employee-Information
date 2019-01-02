﻿using EIS.Entities.Address;
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
        
        [HttpGet]
        public IEnumerable<Permanent> GetPermanentAddresses()
        {
            return _repository.PermanentAddress.FindAll();
        }
        
        [HttpGet("{id}")]
        public IActionResult GetPermanent([FromRoute] int id)
        {
            var Permanent = _repository.PermanentAddress.FindByCondition(addr=>addr.PersonId==id);
            if (Permanent == null)
                return NotFound();
            return Ok(Permanent);
        }
        
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
        
        [HttpPost]
        public IActionResult PostPermanent([FromBody] Permanent permanent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _repository.PermanentAddress.CreateAndSave(permanent);
            return CreatedAtAction("GetPermanent", new { id = permanent.Id }, permanent);
        }
        
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