using EIS.Entities.Employee;
using EIS.Entities.Generic;
using EIS.Entities.Models;
using EIS.Repositories.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace EIS.WebAPI.Controllers
{
    [DisplayName("Attendance Management")]
    [EnableCors("MyPolicy")]
    [Route("api/Attendances")]
    [ApiController]
    public class AttendancesController : BaseController
    {
        public AttendancesController(IRepositoryWrapper repository) : base(repository)
        {

        }

        #region[Attendance]
        [HttpGet]
        public IEnumerable<Attendance> GetAttendances()
        {
            return _repository.Attendances.FindAll().Where(x=>x.TenantId==TenantId);
        }
        [HttpGet("{Id}")]
        public Attendance GetAttendancesById([FromRoute]int id)
        {
            return _repository.Attendances.FindByCondition(x => x.PersonId == id && x.DateIn.Date == DateTime.Now.Date && x.TenantId == TenantId);
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
            return Ok(attendance);
        }

        [DisplayName("Create Attendance")]
        [HttpPost("{id}")]
        public IActionResult PostAttendance(int id, [FromBody] Attendance attendance)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            attendance.TenantId = TenantId;
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
        #endregion

        #region[Attendance Reports]
        [DisplayName("Attendance Reports")]
        [HttpPost("GetAllAttendanceMonthly/{month}/{year}")]
        public IActionResult GetAllAttendanceMonthly([FromBody] SortGrid sortGrid, [FromRoute] int month, [FromRoute] int year)
        {
            ArrayList data = new ArrayList();
            var attendanceData = _repository.Attendances.GetAttendanceMonthly(month, year);
            data = string.IsNullOrEmpty(sortGrid.Search) ? _repository.Employee.GetDataByGridCondition(null, sortGrid, attendanceData) : _repository.Employee.GetDataByGridCondition(x => x.FirstName.ToLower().Contains(sortGrid.Search.ToLower()), sortGrid, attendanceData);
            return Ok(data);
        }

        [DisplayName("Attendance Reports")]
        [HttpPost("GetAllAttendanceYearly/{year}")]
        public IActionResult GetAllAttendanceYearly([FromBody]SortGrid sortGrid, [FromRoute] int year)
        {
            ArrayList data = new ArrayList();
            var attendanceData = _repository.Attendances.GetAttendanceYearly(year);
            data = string.IsNullOrEmpty(sortGrid.Search) ? _repository.Employee.GetDataByGridCondition(null, sortGrid, attendanceData) : _repository.Employee.GetDataByGridCondition(x => x.FirstName.ToLower().Contains(sortGrid.Search.ToLower()), sortGrid, attendanceData);
            return Ok(data);
        }


        [DisplayName("Attendance Reports")]
        [HttpPost("GetAllAttendanceWeekly/{startOfWeek}/{endOfWeek}")]
        public IActionResult GetAllAttendanceWeekly([FromBody]SortGrid sortGrid, [FromRoute] string startOfWeek, [FromRoute] string endOfWeek)
        {
            ArrayList data = new ArrayList();
            var attendanceData = _repository.Attendances.GetAttendanceWeekly(Convert.ToDateTime(startOfWeek), Convert.ToDateTime(endOfWeek));
            data = string.IsNullOrEmpty(sortGrid.Search) ? _repository.Employee.GetDataByGridCondition(null, sortGrid, attendanceData) : _repository.Employee.GetDataByGridCondition(x => x.FirstName.ToLower().Contains(sortGrid.Search.ToLower()), sortGrid, attendanceData);
            return Ok(data);
        }
        #endregion

        #region[My Attendance History]
        [DisplayName("My Attendance History")]
        [HttpPost("GetYearlyAttendanceById/{id}/{year}")]
        public IActionResult GetYearlyAttendanceById([FromBody]SortGrid sortGrid, [FromRoute] int year, [FromRoute]int id)
        {
            ArrayList data = new ArrayList();
            IQueryable<Attendance> attendanceData = _repository.Attendances.FindAllByCondition(x => x.DateIn.Year == year && x.PersonId == id);
            DateTime startDate = new DateTime(year, 1, 1);
            DateTime endDate = startDate.AddYears(1);
            IEnumerable<Attendance> attendancelist = _repository.Attendances.GetAttendanceReportByDate(startDate, endDate, attendanceData);
            data = string.IsNullOrEmpty(sortGrid.Search) ? _repository.Attendances.GetDataByGridCondition(null, sortGrid, attendancelist.AsQueryable()) : _repository.Attendances.GetDataByGridCondition(null, sortGrid, attendancelist.AsQueryable());
            data.Add(attendanceData.Count());
            return Ok(data);
        }

        [DisplayName("My Attendance History")]
        [HttpPost("GetMonthlyAttendanceById/{id}/{year}/{month}")]
        public IActionResult GetMonthlyAttendanceById([FromBody]SortGrid sortGrid, [FromRoute] int year, [FromRoute]int id, [FromRoute]int month)
        {
            ArrayList data = new ArrayList();
            IQueryable<Attendance> attendanceData = _repository.Attendances.FindAllByCondition(x => x.DateIn.Year == year && x.DateIn.Month == month && x.PersonId == id);
            DateTime startDate = new DateTime(year, month, 1);
            DateTime endDate = startDate.AddMonths(1);
           IEnumerable<Attendance> attendancelist = _repository.Attendances.GetAttendanceReportByDate(startDate, endDate, attendanceData);
            data = string.IsNullOrEmpty(sortGrid.Search) ? _repository.Attendances.GetDataByGridCondition(null, sortGrid, attendancelist.AsQueryable()) : _repository.Attendances.GetDataByGridCondition(null, sortGrid, attendancelist.AsQueryable());
            data.Add(attendanceData.Count());
            return Ok(data);
        }

        [DisplayName("My Attendance History")]
        [HttpPost("GetWeeklyAttendanceById/{id}/{startDate}/{endDate}")]
        public IActionResult GetWeeklyAttendanceById([FromBody]SortGrid sortGrid, [FromRoute]int id, [FromRoute]string startDate, [FromRoute]string endDate)
        {
            ArrayList data = new ArrayList();
            var attendanceData = _repository.Attendances.FindAllByCondition(x => x.DateIn.Date >= Convert.ToDateTime(startDate) && x.DateIn.Date <= Convert.ToDateTime(endDate) && x.PersonId == id);
            IEnumerable<Attendance> attendancelist = _repository.Attendances.GetAttendanceReportByDate(Convert.ToDateTime(startDate), Convert.ToDateTime(endDate).AddDays(1), attendanceData);
            data = string.IsNullOrEmpty(sortGrid.Search) ? _repository.Attendances.GetDataByGridCondition(null, sortGrid, attendancelist.AsQueryable()) : _repository.Attendances.GetDataByGridCondition(null, sortGrid, attendancelist.AsQueryable());
            data.Add(attendanceData.Count());
            return Ok(data);
        }
        #endregion

        #region[My Attendance Summary]
        [DisplayName("My Attendance Summary")]
        [HttpGet("GetYearlyAttendanceSummaryById/{id}/{year}")]
        public IActionResult GetYearlyAttendanceSummaryById([FromRoute] int year, [FromRoute]int id)
        {
            AttendanceReport attendanceReport = new AttendanceReport();
            IQueryable<Attendance> attendanceData = _repository.Attendances.FindAllByCondition(x => x.DateIn.Year == year && x.PersonId == id);
            int TotalDays = (DateTime.IsLeapYear(year)) ? 366 : 365;
            attendanceReport = _repository.Attendances.GetAttendanceReportSummary(TotalDays, attendanceData);
            return Ok(attendanceReport);
        }

        [DisplayName("My Attendance Summary")]
        [HttpGet("GetMonthlyAttendanceSummaryById/{id}/{year}/{month}")]
        public IActionResult GetMonthlyAttendanceSummaryById([FromRoute] int year, [FromRoute]int id, [FromRoute]int month)
        {
            AttendanceReport attendanceReport = new AttendanceReport();
            IQueryable<Attendance> attendanceData = _repository.Attendances.FindAllByCondition(x => x.DateIn.Year == year && x.DateIn.Month == month && x.PersonId == id);
            int TotalDays = DateTime.DaysInMonth(year, month);
            attendanceReport = _repository.Attendances.GetAttendanceReportSummary(TotalDays, attendanceData);
            return Ok(attendanceReport);
        }

        [DisplayName("My Attendance Summary")]
        [HttpGet("GetWeeklyAttendanceSummaryById/{id}/{startDate}/{endDate}")]
        public IActionResult GetWeeklySummaryAttendanceById([FromRoute]int id, [FromRoute]string startDate, [FromRoute]string endDate)
        {
            AttendanceReport attendanceReport = new AttendanceReport();
            var attendanceData = _repository.Attendances.FindAllByCondition(x => x.DateIn.Date >= Convert.ToDateTime(startDate) && x.DateIn.Date <= Convert.ToDateTime(endDate) && x.PersonId == id);
            attendanceReport = _repository.Attendances.GetAttendanceReportSummary(7, attendanceData);
            return Ok(attendanceReport);
        }
        #endregion

        
        [HttpPost("GetDateWiseAttendance/{id}/{startDate}/{endDate}")]
        public IActionResult GetDateWiseAttendance([FromBody]SortGrid sortGrid, [FromRoute]int id, [FromRoute]string startDate, [FromRoute]string endDate)
        {
            ArrayList data = new ArrayList();
            IEnumerable<Attendance> attendanceData = _repository.Attendances.FindAllByCondition(x => x.DateIn.Date >= Convert.ToDateTime(startDate) && x.DateIn.Date <= Convert.ToDateTime(endDate) && x.PersonId == id);
            IEnumerable<Attendance> attendancelist = _repository.Attendances.GetAttendanceDateWise(Convert.ToDateTime(startDate), Convert.ToDateTime(endDate).AddDays(1), attendanceData);
            data = string.IsNullOrEmpty(sortGrid.Search) ? _repository.Attendances.GetDataByGridCondition(null, sortGrid, attendancelist.AsQueryable()) : _repository.Attendances.GetDataByGridCondition(null, sortGrid, attendancelist.AsQueryable());
            data.Add(attendanceData.Count());
            return Ok(data);
        }
    }
}