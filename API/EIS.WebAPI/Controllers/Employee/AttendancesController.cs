using EIS.Entities.Employee;
using EIS.Repositories.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace EIS.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendancesController : BaseController
    {
        public AttendancesController(IRepositoryWrapper repository) : base(repository)
        {
        }

        // GET: api/Attendances
        [HttpGet]
        public IEnumerable<Attendance> GetAttendances()
        {
            return _repository.Attendance.FindAll();
        }

        // GET: api/Attendances/5
        [HttpGet("{id}")]
        public IActionResult GetAttendance([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var Attendance = _repository.Attendance.FindByCondition(x => x.Id==id);

            if (Attendance == null)
            {
                return NotFound();
            }

            return Ok(Attendance);
        }

        // PUT: api/Attendances/5
        [HttpPut("{id}")]
        public IActionResult PutAttendance([FromRoute] int id, [FromBody] Attendance attendance)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != attendance.Id)
            {
                return BadRequest();
            }

            _repository.Attendance.Update(attendance);

            try
            {
                _repository.Attendance.Save();
            }
            catch (DbUpdateConcurrencyException)
            {             
                    throw;
            }

            return NoContent();
        }

        // POST: api/Attendances
        [HttpPost]
        public IActionResult PostAttendance([FromBody] Attendance attendance)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _repository.Attendance.Create(attendance);
            _repository.Attendance.Save(); 
            return CreatedAtAction("GetAttendance", new { id = attendance.Id }, attendance);
        }

        // DELETE: api/Attendances/5
        [HttpDelete("{id}")]
        public IActionResult DeleteAttendance([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var Attendance = _repository.Attendance.FindByCondition(x => x.Id == id);
            if (Attendance == null)
            {
                return NotFound();
            }
            _repository.Attendance.Delete(Attendance);
            _repository.Attendance.Save();
            
            return Ok(Attendance);
        }
    }
}