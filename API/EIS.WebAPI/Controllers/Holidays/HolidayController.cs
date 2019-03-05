using Microsoft.AspNetCore.Mvc;
using EIS.Entities.Hoildays;
using System.Collections.Generic;
using EIS.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;
using EIS.Entities.Generic;
using System.Collections;
using System.Linq;
using System;
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
        public IActionResult GetHolidays([FromBody]SortGrid sortGrid)
        {
            ArrayList data = new ArrayList();
            IQueryable<Holiday> holidays = null;
            if (sortGrid.LocationId == 0)
            {
                holidays = _repository.Holidays.FindAll().Include(x => x.Location).Where(x => x.Location.IsActive == true && x.IsActive==true);
            }
            else
            {
                holidays = _repository.Holidays.FindAll().Include(x => x.Location).Where(x => x.Location.IsActive == true && x.LocationId == sortGrid.LocationId && x.IsActive == true);
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
            return _repository.Holidays.FindAll().Include(x=>x.Location).Where(x=>x.IsActive==true);
        }
        [HttpGet("{PersonId}")]
        public IEnumerable<Holiday> GetHolidaysForEmployee([FromRoute]int PersonId)
        {
            int? LocationId = _repository.Employee.FindByCondition(x => x.Id == PersonId).LocationId;
            return _repository.Holidays.FindAll().Where(x => x.LocationId == LocationId && x.IsActive==true);
        }
        [Route("GetHolidayById/{HolidayId}")]
        [HttpGet]
        public IActionResult GetHolidaysById([FromRoute]int HolidayId)
        {
            Holiday holiday = _repository.Holidays.FindByCondition(x => x.Id == HolidayId);
            if (holiday == null)
            {
                return BadRequest();
            }
            return Ok(holiday);
        }
        [HttpPost("{HolidayId}")]
        public IActionResult Create([FromRoute]int HolidayId, [FromBody]Holiday holiday)
        {
            if (HolidayId == 0)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                holiday.TenantId = TenantId;
                _repository.Holidays.CreateAndSave(holiday);
                _repository.Holidays.Dispose();
                return Ok(holiday);
            }
            else
            {
                holiday.TenantId = TenantId;
                _repository.Holidays.UpdateAndSave(holiday);
                _repository.Holidays.Dispose();
                return Ok(holiday);
            }
        }
        [Route("DeleteHoliday/{id}")]
        [HttpPost]
        public IActionResult Delete([FromRoute]int id)
        {
            Holiday holiday = _repository.Holidays.FindByCondition(x => x.Id == id);
            if (holiday == null)
            {
                return NotFound();
            }
            holiday.IsActive = false;
            _repository.Holidays.UpdateAndSave(holiday);
            _repository.Holidays.Dispose();
            return Ok(holiday);
        }
        [HttpGet("{date}/{loc}")]
        public IEnumerable<Holiday> GetHoliday([FromRoute] string date, [FromRoute] int loc)
        {
            var holiday = _repository.Holidays.GetHolidayByYear(date, loc);
            return holiday;
        }
        [HttpGet("{month}/{year}/{loc}")]
        public IEnumerable<Holiday> GetHoliday([FromRoute] string month,[FromRoute] string year, [FromRoute] int loc)
        {
            var holiday = _repository.Holidays.GetHolidayByMonth(month,year, loc);
            return holiday;
        }
        [HttpGet("{firstDate}/{lastDate}/{v}/{location}")]
        public IEnumerable<Holiday> GetHoliday([FromRoute] string firstDate, [FromRoute] string lastDate, [FromRoute] string v, [FromRoute] int location)
        {
            var holiday = _repository.Holidays.GetHolidayByWeek(firstDate, lastDate, location);
            return holiday;
        }
    }
}
