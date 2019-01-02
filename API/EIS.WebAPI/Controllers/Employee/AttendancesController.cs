using EIS.Entities.Employee;
using EIS.Repositories.IRepository;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;

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

        // GET: api/Attendances
        [HttpGet]
        public IEnumerable<Attendance> GetAttendances()
        {
            return _repository.Attendances.FindAll();
        }

        // PUT: api/Attendances/5
        [HttpPut("{id}")]
        public IActionResult PutAttendance([FromRoute] int id, [FromBody] Attendance attendance)
        {
            attendance = _repository.Attendances.FindByCondition2(x => x.PersonId == id && x.DateIn.Date==DateTime.Now.Date);
            //attendance.CreatedDate = DateTime.Now;
            attendance.UpdatedDate = DateTime.Now;
            attendance.DateOut = DateTime.Now;
            attendance.TimeOut = DateTime.Now.TimeOfDay;
            attendance.TotalHours = attendance.TimeOut - attendance.TimeIn;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (attendance.DateIn.Date != DateTime.Now.Date)
            {

            }
            else
            {
                _repository.Attendances.UpdateAndSave(attendance);
            }
            return NoContent();
        }

        // POST: api/Attendances
        [HttpPost("{id}")]
        public IActionResult PostAttendance(int id, [FromBody] Attendance attendance)
        {
            
            attendance.PersonId = id;
            attendance.DateIn = DateTime.Now;
            string dt = attendance.DateIn.DayOfWeek.ToString();
            string Timein= DateTime.Now.ToLongTimeString();
            attendance.TimeIn = DateTime.Now.TimeOfDay;
            attendance.CreatedDate = DateTime.Now;
            attendance.UpdatedDate = DateTime.Now;
            attendance.IsActive = true;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _repository.Attendances.CreateAndSave(attendance);
            return CreatedAtAction("GetAttendance", new { id = attendance.Id }, attendance);
        }

        // DELETE: api/Attendances/5
        [HttpDelete("{id}")]
        public IActionResult DeleteAttendance([FromRoute] int id)
        {
            var Attendance = _repository.Attendances.FindByCondition(x => x.Id == id);
            if (Attendance == null)
            {
                return NotFound();
            }
            _repository.Attendances.DeleteAndSave(Attendance);
            
            return Ok(Attendance);
        }

        [Route("GetAllAttendanceMonthly/{month}/{year}")]
        [HttpGet]
        public IEnumerable<Person> GetAllAttendanceMonthly([FromRoute] int month, [FromRoute] int year)
        {
            var data = _repository.Attendances.GetAttendanceMonthly(month,year);
            return data;
        }
        [Route("GetAllAttendanceYearly/{year}") ]
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