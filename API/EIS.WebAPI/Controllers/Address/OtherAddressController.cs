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
    public class OtherAddressController : BaseController
    {

        public OtherAddressController(IRepositoryWrapper repository): base(repository)
        {

        }
        [DisplayName("Profile view")]
        [HttpGet("{id}")]
        public IEnumerable<Other> GetOtherAddresseses(int id)
        {
            IEnumerable<Other> data = _repository.OtherAddress.FindAllByCondition(o => o.PersonId == id);
            _repository.OtherAddress.Dispose();
            return data;
        }
    }
}