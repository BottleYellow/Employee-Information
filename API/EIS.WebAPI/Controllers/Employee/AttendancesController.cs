using EIS.Entities.Employee;
using EIS.Entities.Generic;
using EIS.Entities.Models;
using EIS.Entities.SP;
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
            return _repository.Attendances.FindAll().Include(x => x.Person).Where(x => x.TenantId == TenantId);
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
        //[Route("ClockIn/{id}")]
        public IActionResult PostAttendance([FromRoute] int id, [FromBody] Attendance attendance)
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
            Attendance Attendance = _repository.Attendances.FindByCondition(x => x.Id == id);
            _repository.Attendances.DeleteAndSave(Attendance);
            return Ok(Attendance);
        }
        #endregion

        #region[Attendance Reports]

        [DisplayName("Attendance Reports")]
        [HttpGet("GetAllAttendanceEmpCount/{SearchFor}/{InputOne}/{InputTwo}/{locationId}")]
        public IActionResult GetAllAttendance([FromRoute] string SearchFor, [FromRoute] string InputOne, [FromRoute] string InputTwo, [FromRoute]int locationId)
        {
            Attendance_Report attendanceData = _repository.Attendances.GetAttendanceCountReport(SearchFor, InputOne, InputTwo, locationId);
            return Ok(attendanceData);
        }
        #endregion

        #region[My Attendance History]
        [DisplayName("My Attendance History")]
        [HttpGet("GetYearlyAttendanceById/{id}/{year}")]
        public IActionResult GetYearlyAttendanceById([FromRoute] int year, [FromRoute]int id)
        {
            string Code = _repository.Employee.GetEmployeeCode(id);
            int? locationId = _repository.Employee.FindByCondition(x => x.EmployeeCode == Code).LocationId;
            IEnumerable<Attendance> attendanceData = _repository.Attendances.FindAllByCondition(x => x.DateIn.Year == year && x.PersonId == id);
            DateTime startDate = new DateTime(year, 1, 1);
            DateTime endDate;
            if (startDate.Year == DateTime.Now.Year)
            {
                endDate = DateTime.Now.Date;
            }
            else
            {
                endDate = startDate.AddYears(1);
            }
            List<AttendanceReportByDate> attendancelist = _repository.Attendances.GetAttendanceReportByDate(startDate, endDate, attendanceData, Code, locationId);

            return Ok(attendancelist);
        }

        [DisplayName("My Attendance History")]
        [HttpGet("GetMonthlyAttendanceById/{id}/{year}/{month}")]
        public IActionResult GetMonthlyAttendanceById([FromRoute] int year, [FromRoute]int id, [FromRoute]int month)
        {
            string Code = _repository.Employee.GetEmployeeCode(id);
            int? locationId = _repository.Employee.FindByCondition(x => x.EmployeeCode == Code).LocationId;

            IEnumerable<Attendance> attendanceData = _repository.Attendances.FindAllByCondition(x => x.DateIn.Year == year && x.DateIn.Month == month && x.PersonId == id);
            DateTime startDate = new DateTime(year, month, 1);
            DateTime endDate;

            if (startDate.Month == DateTime.Now.Month)
            {
                endDate = DateTime.Now.Date;
            }
            else
            {
                endDate = startDate.AddMonths(1).AddDays(-1);
            }
            List<AttendanceReportByDate> attendancelist = _repository.Attendances.GetAttendanceReportByDate(startDate, endDate, attendanceData, Code, locationId);
     
            return Ok(attendancelist);
        }

        [DisplayName("My Attendance History")]
        [HttpGet("GetWeeklyAttendanceById/{id}/{startDate}/{endDate}")]
        public IActionResult GetWeeklyAttendanceById([FromRoute]int id, [FromRoute]string startDate, [FromRoute]string endDate)
        {
            string Code = _repository.Employee.GetEmployeeCode(id);
            int? locationId = _repository.Employee.FindByCondition(x => x.EmployeeCode == Code).LocationId;
            //IEnumerable<Attendance> attendanceData = _repository.Attendances.FindAllByCondition(x => x.DateIn.Date >= Convert.ToDateTime(startDate) && x.DateIn.Date <= Convert.ToDateTime(endDate) && x.PersonId == id);
            //List<AttendanceReportByDate> attendancelist = _repository.Attendances.GetAttendanceReportByDate(Convert.ToDateTime(startDate), Convert.ToDateTime(endDate).AddDays(1), attendanceData,Code,null);
            DateTime sDate = Convert.ToDateTime(startDate);
            DateTime eDate = Convert.ToDateTime(endDate);
            DateTime startOfWeek = DateTime.Today.AddDays(-1 * (int)(DateTime.Today.DayOfWeek));
            if (startOfWeek<=sDate)
            {
                eDate = DateTime.Now.Date;
            }
            IEnumerable<Attendance> attendanceData = _repository.Attendances.FindAllByCondition(x => x.DateIn.Date >= sDate && x.DateIn.Date <= eDate && x.PersonId == id);
            List<AttendanceReportByDate> attendancelist = _repository.Attendances.GetAttendanceReportByDate(sDate, eDate, attendanceData,Code, locationId);
            return Ok(attendancelist);
        }
        #endregion

        #region[My Attendance Summary]
        [DisplayName("My Attendance Summary")]
        [HttpGet("GetYearlyAttendanceSummaryById/{id}/{year}")]
        public IActionResult GetYearlyAttendanceSummaryById([FromRoute] int year, [FromRoute]int id)
        {
            EmployeeAttendanceReport attendanceReport = new EmployeeAttendanceReport();
            attendanceReport = _repository.Attendances.GetAttendanceReportSummary("Year",id,year,1);
            return Ok(attendanceReport);
        }

        [DisplayName("My Attendance Summary")]
        [HttpGet("GetMonthlyAttendanceSummaryById/{id}/{year}/{month}")]
        public IActionResult GetMonthlyAttendanceSummaryById([FromRoute] int year, [FromRoute]int id, [FromRoute]int month)
        {
            EmployeeAttendanceReport attendanceReport = new EmployeeAttendanceReport();
            attendanceReport = _repository.Attendances.GetAttendanceReportSummary("Month", id, year, month);
            return Ok(attendanceReport);
        }

        [DisplayName("My Attendance Summary")]
        [HttpGet("GetWeeklyAttendanceSummaryById/{id}/{startDate}/{endDate}")]
        public IActionResult GetWeeklySummaryAttendanceById([FromRoute]int id, [FromRoute]string startDate, [FromRoute]string endDate)
        {
            AttendanceReport attendanceReport = new AttendanceReport();
            IQueryable<Attendance> attendanceData = _repository.Attendances.FindAllByCondition(x => x.DateIn.Date >= Convert.ToDateTime(startDate) && x.DateIn.Date <= Convert.ToDateTime(endDate) && x.PersonId == id);
            return Ok(attendanceReport);
        }
        #endregion

        #region Datewise Attendance
        [HttpGet("GetDateWiseAttendance/{id}/{LocationId}/{startDate}/{endDate}")]
        public IActionResult GetDateWiseAttendance([FromRoute]string id, [FromRoute]int LocationId, [FromRoute]string startDate, [FromRoute]string endDate)
        {
            if (Convert.ToDateTime(endDate) > DateTime.Now.Date)
            {
                endDate = DateTime.Now.Date.ToString();
            }
            List<AttendanceReportByDate> attendancelist = null;
            if (id == "0")
            {
                IEnumerable<Attendance> attendanceData = _repository.Attendances.FindAllByCondition(x => x.DateIn.Date >= Convert.ToDateTime(startDate) && x.DateIn.Date <= Convert.ToDateTime(endDate)).Include(x=>x.Person);
                attendancelist = _repository.Attendances.GetAttendanceReportByDate(Convert.ToDateTime(startDate), Convert.ToDateTime(endDate), attendanceData,id,LocationId);
            }
            else
            {
                int PersonId = _repository.Employee.FindByCondition(x => x.EmployeeCode == id).Id;
                IEnumerable<Attendance> attendanceData = _repository.Attendances.FindAllByCondition(x => x.DateIn.Date >= Convert.ToDateTime(startDate) && x.DateIn.Date <= Convert.ToDateTime(endDate) && x.PersonId == PersonId).Include(x => x.Person);
                attendancelist = _repository.Attendances.GetAttendanceReportByDate(Convert.ToDateTime(startDate), Convert.ToDateTime(endDate), attendanceData, id, LocationId);
            }
            return Ok(attendancelist);
        }
        #endregion
    }
}