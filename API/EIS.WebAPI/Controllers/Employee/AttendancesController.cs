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

        [HttpGet("{id}")]
        public IActionResult GetAttendanceById([FromRoute]int id)
        {
<<<<<<< HEAD
            var data = _repository.Attendances.FindAllByCondition(x => x.PersonId == id);
            return Ok(data);
        }
=======
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
>>>>>>> eab0133b5e8f6e86eb09bb18611280e9b8dcee1c

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

        [HttpGet("GetWeeklyAttendanceById/{id}/{startDate}/{endDate}")]
        public IActionResult GetWeeklyAttendanceById([FromRoute]int id, [FromRoute]DateTime startDate, [FromRoute]DateTime endDate)
        {
            var data = _repository.Attendances.FindAllByCondition(x => x.DateIn.Date >= startDate && x.DateIn.Date <= endDate && x.PersonId == id);
            return Ok(data);
        }
        
        [HttpPut("{id}")]
        public IActionResult PutAttendance([FromRoute] int id, [FromBody] Attendance attendance)
        {
            var updateAttendance = _repository.Attendances.FindByCondition(x => x.PersonId == id && x.DateIn.Date == DateTime.Now.Date);
            if (updateAttendance == null)
            {
                return BadRequest();
            }
            updateAttendance.UpdatedDate = DateTime.Now;
            updateAttendance.DateOut = DateTime.Now.Date;
            updateAttendance.TimeOut = DateTime.Now.TimeOfDay;
            updateAttendance.TotalHours = updateAttendance.TimeOut - updateAttendance.TimeIn;
            _repository.Attendances.UpdateAndSave(updateAttendance);
            return Ok(updateAttendance);
        }
        
        [HttpPost("{id}")]
        public IActionResult PostAttendance(int id, [FromBody] Attendance attendance)
        {
            var createAttendance = _repository.Attendances.FindByCondition(x => x.PersonId == id && x.DateIn.Date == DateTime.Now.Date);
            if (createAttendance != null)
            {
                return BadRequest(createAttendance.TimeIn);
            }     
            attendance.Id = 0;
            attendance.PersonId = id;
            attendance.DateIn = DateTime.Now;
            attendance.TimeIn = DateTime.Now.TimeOfDay;
            attendance.CreatedDate = DateTime.Now;
            attendance.UpdatedDate = DateTime.Now;
            attendance.IsActive = true;
            _repository.Attendances.CreateAndSave(attendance);
            return Ok(attendance);
        }
        
        [HttpDelete("{id}")]
        public IActionResult DeleteAttendance([FromRoute] int id)
        {
<<<<<<< HEAD
            var attendance = _repository.Attendances.FindByCondition(x => x.Id == id);
            if (attendance == null)
=======
            var Attendance = _repository.Attendances.FindByCondition(x => x.Id == id);
            if (Attendance == null)
>>>>>>> eab0133b5e8f6e86eb09bb18611280e9b8dcee1c
            {
                return NotFound();
            }
            _repository.Attendances.DeleteAndSave(attendance);
            return Ok(attendance);
        }

        [Route("GetAllAttendanceMonthly/{month}/{year}")]
        [HttpGet]
        public IEnumerable<Person> GetAllAttendanceMonthly([FromRoute] int month, [FromRoute] int year)
        {
<<<<<<< HEAD
            var data = _repository.Attendances.GetAttendanceMonthly(month, year);
=======
            var data = _repository.Attendances.GetAttendanceMonthly(month,year);
>>>>>>> eab0133b5e8f6e86eb09bb18611280e9b8dcee1c
            return data;
        }
        [Route("GetAllAttendanceYearly/{year}")]
        [HttpGet]
        public IEnumerable<Person> GetAllAttendanceYearly([FromRoute] int year)
        {
<<<<<<< HEAD
            var data = _repository.Attendances.GetAttendanceYearly(year);
=======
            var data = _repository.Attendances.GetAttendanceYearly(year);          
>>>>>>> eab0133b5e8f6e86eb09bb18611280e9b8dcee1c
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