﻿using EIS.Entities.Employee;
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
        [HttpGet("GetAllAttendanceMonthly/{month}/{year}/{location}")]
        public IActionResult GetAllAttendanceMonthly([FromRoute] int month, [FromRoute] int year, [FromRoute]int location)
        {
            IList<AttendanceData> attendanceData = _repository.Attendances.GetAttendanceMonthly(month, year, location);
            return Ok(attendanceData);
        }

        [DisplayName("Attendance Reports")]
        [HttpGet("GetAllAttendanceYearly/{year}/{location}")]
        public IActionResult GetAllAttendanceYearly([FromRoute] int year, [FromRoute]int location)
        {
            IList<AttendanceData> attendanceData = _repository.Attendances.GetAttendanceYearly(year, location);
            return Ok(attendanceData);
        }


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
            IEnumerable<Attendance> attendanceData = _repository.Attendances.FindAllByCondition(x => x.DateIn.Year == year && x.PersonId == id);
            DateTime startDate = new DateTime(year, 1, 1);
            DateTime endDate = startDate.AddYears(1);
            List<AttendanceReportByDate> attendancelist = _repository.Attendances.GetAttendanceReportByDate(startDate, endDate, attendanceData);
            return Ok(attendancelist);
        }

        [DisplayName("My Attendance History")]
        [HttpGet("GetMonthlyAttendanceById/{id}/{year}/{month}")]
        public IActionResult GetMonthlyAttendanceById([FromRoute] int year, [FromRoute]int id, [FromRoute]int month)
        {

            IEnumerable<Attendance> attendanceData = _repository.Attendances.FindAllByCondition(x => x.DateIn.Year == year && x.DateIn.Month == month && x.PersonId == id);
            DateTime startDate = new DateTime(year, month, 1);
            DateTime endDate = startDate.AddMonths(1);
           List<AttendanceReportByDate> attendancelist = _repository.Attendances.GetAttendanceReportByDate(startDate, endDate, attendanceData);
            return Ok(attendancelist);
        }

        [DisplayName("My Attendance History")]
        [HttpGet("GetWeeklyAttendanceById/{id}/{startDate}/{endDate}")]
        public IActionResult GetWeeklyAttendanceById([FromRoute]int id, [FromRoute]string startDate, [FromRoute]string endDate)
        {
            IEnumerable<Attendance> attendanceData = _repository.Attendances.FindAllByCondition(x => x.DateIn.Date >= Convert.ToDateTime(startDate) && x.DateIn.Date <= Convert.ToDateTime(endDate) && x.PersonId == id);
            List<AttendanceReportByDate> attendancelist = _repository.Attendances.GetAttendanceReportByDate(Convert.ToDateTime(startDate), Convert.ToDateTime(endDate).AddDays(1), attendanceData);
            return Ok(attendancelist);
        }
        #endregion

        #region[My Attendance Summary]
        [DisplayName("My Attendance Summary")]
        [HttpGet("GetYearlyAttendanceSummaryById/{id}/{year}")]
        public IActionResult GetYearlyAttendanceSummaryById([FromRoute] int year, [FromRoute]int id)
        {
            AttendanceReport attendanceReport = new AttendanceReport();
            IQueryable<Attendance> attendanceData = _repository.Attendances.FindAllByCondition(x => x.DateIn.Year == year && x.PersonId == id).Where(x => x.TimeOut != null);
            int TotalWorkingDays = 0;
            int TotalDays = DateTime.IsLeapYear(year) ? 366 : 365;
            TotalWorkingDays = TotalDays - 24;
            attendanceReport = _repository.Attendances.GetAttendanceReportSummary(TotalDays, TotalWorkingDays, attendanceData.AsEnumerable());
            return Ok(attendanceReport);
        }

        [DisplayName("My Attendance Summary")]
        [HttpGet("GetMonthlyAttendanceSummaryById/{id}/{year}/{month}")]
        public IActionResult GetMonthlyAttendanceSummaryById([FromRoute] int year, [FromRoute]int id, [FromRoute]int month)
        {
            AttendanceReport attendanceReport = new AttendanceReport();
            IQueryable<Attendance> attendanceData = _repository.Attendances.FindAllByCondition(x => x.DateIn.Year == year && x.DateIn.Month == month && x.PersonId == id);

            int TotalWorkingDays = 0;
            int TotalDays = DateTime.DaysInMonth(year, month);
            for (int i = 1; i <= TotalDays; i++)
            {
                DateTime thisDay = new DateTime(year, month, i);
                if (thisDay.DayOfWeek != DayOfWeek.Sunday)
                {
                    TotalWorkingDays += 1;
                }
            }
            TotalWorkingDays = TotalWorkingDays - 2;
            attendanceReport = _repository.Attendances.GetAttendanceReportSummary(TotalDays, TotalWorkingDays, attendanceData.AsEnumerable());
            return Ok(attendanceReport);
        }

        [DisplayName("My Attendance Summary")]
        [HttpGet("GetWeeklyAttendanceSummaryById/{id}/{startDate}/{endDate}")]
        public IActionResult GetWeeklySummaryAttendanceById([FromRoute]int id, [FromRoute]string startDate, [FromRoute]string endDate)
        {
            AttendanceReport attendanceReport = new AttendanceReport();
            IQueryable<Attendance> attendanceData = _repository.Attendances.FindAllByCondition(x => x.DateIn.Date >= Convert.ToDateTime(startDate) && x.DateIn.Date <= Convert.ToDateTime(endDate) && x.PersonId == id);
            attendanceReport = _repository.Attendances.GetAttendanceReportSummary(7, 6, attendanceData.AsEnumerable());
            return Ok(attendanceReport);
        }
        #endregion

        
        [HttpGet("GetDateWiseAttendance/{id}/{startDate}/{endDate}")]
        public IActionResult GetDateWiseAttendance([FromRoute]string id, [FromRoute]string startDate, [FromRoute]string endDate)
        {
            int PersonId = _repository.Employee.FindByCondition(x => x.EmployeeCode == id).Id;
            IEnumerable<Attendance> attendanceData = _repository.Attendances.FindAllByCondition(x => x.DateIn.Date >= Convert.ToDateTime(startDate) && x.DateIn.Date <= Convert.ToDateTime(endDate) && x.PersonId == PersonId);
            List<AttendanceReportByDate> attendancelist = _repository.Attendances.GetAttendanceReportByDate(Convert.ToDateTime(startDate), Convert.ToDateTime(endDate).AddDays(1), attendanceData);
            return Ok(attendancelist);
        }
    }
}