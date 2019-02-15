using Microsoft.AspNetCore.Mvc;
using EIS.Entities.Hoildays;
using System.Collections.Generic;
using EIS.Repositories.IRepository;
using EIS.Entities.Employee;
using Microsoft.AspNetCore.Authorization;
using EIS.Entities.Generic;
using System.Collections;
using System.Linq;
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
            return _repository.Locations.FindAll().Where(x => x.IsActive == true);
        }
        [HttpPost]
        [Route("Data")]
        [AllowAnonymous]
        public IActionResult GetData([FromBody]SortGrid sortGrid)
        {
            ArrayList locationsList;
            IQueryable<Locations> list = null;
            list = _repository.Locations.FindAllByCondition(x => x.TenantId == TenantId && x.IsActive == sortGrid.IsActive);
            if (sortGrid.Search == null)
            {
                locationsList = _repository.Locations.GetDataByGridCondition(null, sortGrid, list);
            }
            else
            {
                locationsList = _repository.Locations.GetDataByGridCondition(x => x.LocationName.Contains(sortGrid.Search) , sortGrid, list);
            }
            return Ok(locationsList);
        }
        [HttpPost("{LocationId}")]
        public IActionResult Create([FromRoute]int LocationId,[FromBody]Locations location)
        {
            if (LocationId == 0)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                location.TenantId = TenantId;
                _repository.Locations.CreateAndSave(location);
                return Ok(location);
            }
            else
            {
                location.TenantId = TenantId;
                _repository.Locations.UpdateAndSave(location);
                return Ok(location);
            }
        }
        [Route("DeleteLocation/{id}")]
        [HttpPost]
        public IActionResult Delete([FromRoute]int id)
        {
            Locations location = _repository.Locations.FindByCondition(x => x.Id == id);
            if (location == null)
            {
                return NotFound();
            }
            location.IsActive = false;
            _repository.Locations.UpdateAndSave(location);
            return Ok(location);
        }
        [Route("ActivateLocation/{id}")]
        [HttpGet]
        public IActionResult ActivateLocation([FromRoute]int id)
        {
            Locations location = _repository.Locations.ActivateLocation(id);
            if(location==null)
            {
                return BadRequest();
            }
            return Ok();
        }
    }
}
