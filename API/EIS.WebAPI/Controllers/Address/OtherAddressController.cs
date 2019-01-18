using EIS.Entities.Address;
using EIS.Repositories.IRepository;
using EIS.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace EIS.WebAPI.Controllers
{
    [Route("api/OtherAddress")]
    [ApiController]
    public class OtherAddressController : Controller
    {
        public readonly IRepositoryWrapper _repository;
        public OtherAddressController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }
        [DisplayName("Profile view")]
        [HttpGet("{id}")]
        public IEnumerable<Other> GetOtherAddresseses(int id)
        {
            return _repository.OtherAddress.FindAllByCondition(o => o.PersonId == id);
        }
    }
}