using Microsoft.AspNetCore.Mvc;
using EIS.Entities.Hoildays;
using System.Collections.Generic;
using EIS.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;
using EIS.Entities.Generic;
using System.Collections;
using System.Linq;
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
        [Route("GetHolidays")]
        [HttpPost]
        public IActionResult GetPastLeaves([FromBody]SortGrid sortGrid)
        {
            ArrayList data = new ArrayList();
            IQueryable<Holiday> holidays = null;
            if (sortGrid.LocationId == 0)
            {
                holidays = _repository.Holidays.FindAll().Include(x => x.Location);
            }
            else
            {
                holidays = _repository.Holidays.FindAll().Include(x => x.Location).Where(x => x.LocationId == sortGrid.LocationId);
            }

            if (string.IsNullOrEmpty(sortGrid.Search))
            {

                data = _repository.Holidays.GetDataByGridCondition(null, sortGrid, holidays);
            }
            else
            {
                string search = sortGrid.Search.ToLower();
                data = _repository.Holidays.GetDataByGridCondition(x => x.Location.LocationName.ToLower().Contains(search) || x.Vacation.ToLower().Contains(search), sortGrid, holidays);
            }
            return Ok(data);

        }
        [HttpGet]
        public IEnumerable<Holiday> GetHolidays()
        {
            return _repository.Holidays.FindAll().Include(x=>x.Location);
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
