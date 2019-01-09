using EIS.Entities.Employee;
using EIS.Repositories.IRepository;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace EIS.WebAPI.Controllers
{
    [DisplayName("Attendance Management")]
    [EnableCors("MyPolicy")]
    [Route("api/Attendances")]
    [ApiController]
    public class AttendancesController : Controller
    {
        public readonly IRepositoryWrapper _repository;
        public AttendancesController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IEnumerable<Attendance> GetAttendances()
        {
            return _repository.Attendances.FindAll();
        }
        [HttpGet("{Id}")]
        public Attendance GetAttendancesById([FromRoute]int id)
        {
            return _repository.Attendances.FindByCondition(x => x.PersonId == id && x.DateIn.Date==DateTime.Now.Date);
        }


        [DisplayName("Create Attendance")]
        [HttpPut("{id}")]
        public IActionResult PutAttendance([FromRoute] int id, [FromBody] Attendance attendance)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            attendance = _repository.Attendances.FindByCondition(x => x.PersonId == id && x.DateIn.Date == DateTime.Now.Date);
            attendance.UpdatedDate = DateTime.Now;
            attendance.DateOut = DateTime.Now;
            attendance.TimeOut = DateTime.Now.TimeOfDay;
            attendance.TotalHours = attendance.TimeOut - attendance.TimeIn;
            _repository.Attendances.UpdateAndSave(attendance);
            return NoContent();
        }

        [DisplayName("Create Attendance")]
        [HttpPost("{id}")]
        public IActionResult PostAttendance(int id, [FromBody] Attendance attendance)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            attendance.PersonId = id;
            attendance.DateIn = DateTime.Now;
            string dt = attendance.DateIn.DayOfWeek.ToString();
            string Timein = DateTime.Now.ToLongTimeString();
            attendance.TimeIn = DateTime.Now.TimeOfDay;
            attendance.CreatedDate = DateTime.Now;
            attendance.UpdatedDate = DateTime.Now;
            attendance.IsActive = true;
            _repository.Attendances.CreateAndSave(attendance);
            return CreatedAtAction("GetAttendance", new { id = attendance.Id }, attendance);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAttendance([FromRoute] int id)
        {
            var Attendance = _repository.Attendances.FindByCondition(x => x.Id == id);
            _repository.Attendances.DeleteAndSave(Attendance);
            return Ok(Attendance);
        }


        [DisplayName("Attendance Reports")]
        [Route("GetAllAttendanceMonthly/{month}/{year}")]
        [HttpGet]
        public IEnumerable<Person> GetAllAttendanceMonthly([FromRoute] int month, [FromRoute] int year)
        {
            var data = _repository.Attendances.GetAttendanceMonthly(month, year);
            return data;
        }


        [DisplayName("Attendance Reports")]
        [Route("GetAllAttendanceYearly/{year}")]
        [HttpGet]
        public IEnumerable<Person> GetAllAttendanceYearly([FromRoute] int year)
        {
            var data = _repository.Attendances.GetAttendanceYearly(year);
            return data;
        }


        [DisplayName("Attendance Reports")]
        [Route("GetAllAttendanceWeekly/{startOfWeek}/{endOfWeek}")]
        [HttpGet]
        public IEnumerable<Person> GetAllAttendanceWeekly([FromRoute] DateTime startOfWeek, [FromRoute] DateTime endOfWeek)
        {
            var data = _repository.Attendances.GetAttendanceWeekly(startOfWeek, endOfWeek);
            return data;
        }
     

        [DisplayName("My Attendance History")]
        [HttpGet("GetAttendanceById/{id}/{year}/{month?}")]
        public IActionResult GetAttendanceById([FromRoute] int year, [FromRoute]int id, [FromRoute]int? month)
        {
            IEnumerable<Attendance> attendance;
            if (month == null)
                attendance = _repository.Attendances.FindAllByCondition(x => x.DateIn.Year == year && x.PersonId == id);
            else
                attendance = _repository.Attendances.FindAllByCondition(x => x.DateIn.Year == year && x.DateIn.Month == month && x.PersonId == id);
            return Ok(attendance);
        }

        [DisplayName("My Attendance History")]
        [HttpGet("GetWeeklyAttendanceById/{id}/{startDate}/{endDate}")]
        public IActionResult GetWeeklyAttendanceById([FromRoute]int id, [FromRoute]DateTime startDate, [FromRoute]DateTime endDate)
        {
            var data = _repository.Attendances.FindAllByCondition(x => x.DateIn.Date >= startDate && x.DateIn.Date <= endDate && x.PersonId == id);
            return Ok(data);
        }
    }
}