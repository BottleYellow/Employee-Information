using Microsoft.AspNetCore.Mvc;
using EIS.Entities.Hoildays;
using System.Collections.Generic;
using EIS.Repositories.IRepository;
using EIS.Entities.Employee;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EIS.WebAPI.Controllers.Location
{
    [Route("api/Location")]
    [ApiController]
    public class LocationController : BaseController
    {
        public LocationController(IRepositoryWrapper repository) : base(repository)
        {
 
        }
        [HttpGet]
        public IEnumerable<Locations> GetLocations()
        {
            return _repository.Locations.FindAll();
        }
        [HttpPost]
        public IActionResult Create([FromBody]Locations location)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            location.TenantId = TenantId;
            _repository.Locations.CreateAndSave(location);
            return Ok(location);
        }
       
    }
}
