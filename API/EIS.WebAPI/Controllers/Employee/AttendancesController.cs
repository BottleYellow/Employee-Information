using EIS.Entities.Employee;
using EIS.Repositories.IRepository;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace EIS.WebAPI.Controllers
{
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

        [HttpPut("{id}")]
        public IActionResult PutAttendance([FromRoute] int id, [FromBody] Attendance attendance)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            attendance = _repository.Attendances.FindByCondition2(x => x.PersonId == id && x.DateIn.Date == DateTime.Now.Date);
            attendance.UpdatedDate = DateTime.Now;
            attendance.DateOut = DateTime.Now;
            attendance.TimeOut = DateTime.Now.TimeOfDay;
            attendance.TotalHours = attendance.TimeOut - attendance.TimeIn;
            _repository.Attendances.UpdateAndSave(attendance);
            return NoContent();
        }

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

        [Route("GetAllAttendanceMonthly/{month}/{year}")]
        [HttpGet]
        public IEnumerable<Person> GetAllAttendanceMonthly([FromRoute] int month, [FromRoute] int year)
        {
            var data = _repository.Attendances.GetAttendanceMonthly(month, year);
            return data;
        }

        [Route("GetAllAttendanceYearly/{year}")]
        [HttpGet]
        public IEnumerable<Person> GetAllAttendanceYearly([FromRoute] int year)
        {
            var data = _repository.Attendances.GetAttendanceYearly(year);
            return data;
        }

        [Route("GetAllAttendanceWeekly/{startOfWeek}/{endOfWeek}")]
        [HttpGet]
        public IEnumerable<Person> GetAllAttendanceWeekly([FromRoute] DateTime startOfWeek, [FromRoute] DateTime endOfWeek)
        {
            var data = _repository.Attendances.GetAttendanceWeekly(startOfWeek, endOfWeek);
            return data;
        }

    }
}