using Microsoft.AspNetCore.Mvc;
using EIS.Entities.Hoildays;
using System.Collections.Generic;
using EIS.Repositories.IRepository;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EIS.WebAPI.Controllers.Holidays
{
    [Route("api/Holiday")]
    [ApiController]
    public class HolidayController : BaseController
    {
        public HolidayController(IRepositoryWrapper repository) : base(repository)
        {
 
        }
        [HttpGet]
        public IEnumerable<Holiday> GetDesignations()
        {
            return _repository.Holidays.FindAll();
        }
        [HttpPost]
        public IActionResult Create([FromBody]Holiday holiday)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            holiday.TenantId = TenantId;
            _repository.Holidays.CreateAndSave(holiday);
            return Ok(holiday);
        }
       
    }
}
