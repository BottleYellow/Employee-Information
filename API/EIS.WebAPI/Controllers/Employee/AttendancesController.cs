using EIS.Entities.Employee;
using EIS.Entities.Generic;
using EIS.Repositories.IRepository;
using EIS.WebAPI.Models;
using EIS.WebAPI.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
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
    public class AttendancesController : Controller
    {
        RedisAgent Cache = new RedisAgent();
        int TenantId = 0;
        public readonly IRepositoryWrapper _repository;
        public AttendancesController(IRepositoryWrapper repository)
        {
            TenantId = Convert.ToInt32(Cache.GetStringValue("TenantId"));
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

        [DisplayName("Attendance Reports")]
        [Route("GetAllAttendanceMonthly/{month}/{year}")]
        [HttpPost]
        public IActionResult GetAllAttendanceMonthly([FromBody] SortGrid sortGrid, [FromRoute] int month, [FromRoute] int year)
        {
            ArrayList data = new ArrayList();
            var attendanceData = _repository.Attendances.GetAttendanceMonthly(month, year);
            if (string.IsNullOrEmpty(sortGrid.Search))
            {

                data = _repository.Employee.GetDataByGridCondition(null, sortGrid, attendanceData);
            }
            else
            {
                data = _repository.Employee.GetDataByGridCondition(x => x.EmployeeCode == sortGrid.Search, sortGrid, attendanceData);
            }
            return Ok(data);
        }

        [DisplayName("Attendance Reports")]
        [Route("GetAllAttendanceYearly/{year}")]
        [HttpPost]
        public IActionResult GetAllAttendanceYearly([FromBody]SortGrid sortGrid, [FromRoute] int year)
        {
            ArrayList data = new ArrayList();
            var attendanceData = _repository.Attendances.GetAttendanceYearly(year);
            if (string.IsNullOrEmpty(sortGrid.Search))
            {

                data = _repository.Employee.GetDataByGridCondition(null, sortGrid, attendanceData);
            }
            else
            {
                data = _repository.Employee.GetDataByGridCondition(x => x.EmployeeCode == sortGrid.Search, sortGrid, attendanceData);
            }
            return Ok(data);
        }


        [DisplayName("Attendance Reports")]
        [Route("GetAllAttendanceWeekly/{startOfWeek}/{endOfWeek}")]
        [HttpPost]
        public IActionResult GetAllAttendanceWeekly([FromBody]SortGrid sortGrid, [FromRoute] DateTime startOfWeek, [FromRoute] DateTime endOfWeek)
        {
            ArrayList data = new ArrayList();
            var attendanceData = _repository.Attendances.GetAttendanceWeekly(startOfWeek, endOfWeek);
            if (string.IsNullOrEmpty(sortGrid.Search))
            {

                data = _repository.Employee.GetDataByGridCondition(null, sortGrid, attendanceData);
            }
            else
            {
                data = _repository.Employee.GetDataByGridCondition(x => x.EmployeeCode == sortGrid.Search, sortGrid, attendanceData);
            }
            return Ok(data);
        }


        [DisplayName("My Attendance History")]
        [Route("GetYearlyAttendanceById/{id}/{year}")]
        [HttpPost]
        public IActionResult GetYearlyAttendanceById([FromBody]SortGrid sortGrid,[FromRoute] int year, [FromRoute]int id)
        {
            ArrayList data = new ArrayList();
            IQueryable<Attendance> attendanceData = _repository.Attendances.FindAllByCondition(x => x.DateIn.Year == year && x.PersonId == id);
            DateTime targetDate = new DateTime(year, 1, 1);
            DateTime endDate = targetDate.AddYears(1);            

            IList<Attendance> attendancelist = new List<Attendance>();
            for (DateTime date = targetDate; date < endDate; date = date.AddDays(1))
            {
                Attendance newlist = new Attendance();
                newlist.DateIn = date;
                var attendance = attendanceData.Where(x => x.DateIn == date).Select(x => new { x.TimeIn, x.TimeOut }).FirstOrDefault();
                if (attendance == null || attendance.TimeIn == attendance.TimeOut)
                {
                    if (date < DateTime.Now.Date)
                    {

                        newlist.TimeIn = new TimeSpan();
                        newlist.TimeOut = new TimeSpan();
                        newlist.IsActive = false;
                    }
                }
                else
                {
                    newlist.TimeIn = attendance.TimeIn;
                    newlist.TimeOut = attendance.TimeOut;
                    newlist.IsActive = true;
                }
                attendancelist.Add(newlist);
            }
            if (string.IsNullOrEmpty(sortGrid.Search))
            {
                data = _repository.Attendances.GetDataByGridCondition(null, sortGrid, attendancelist.AsQueryable());
            }
            else
            {
                data = _repository.Attendances.GetDataByGridCondition(x => x.DateIn.ToString() == sortGrid.Search, sortGrid, attendancelist.AsQueryable());
            }
            data.Add(attendanceData.Count());
            return Ok(data);
        }

        [DisplayName("My Attendance History")]
        [Route("GetMonthlyAttendanceById/{id}/{year}/{month}")]
        [HttpPost]
        public IActionResult GetMonthlyAttendanceById([FromBody]SortGrid sortGrid,[FromRoute] int year, [FromRoute]int id, [FromRoute]int month)
        {
        ArrayList data = new ArrayList();
        IQueryable<Attendance> attendanceData = _repository.Attendances.FindAllByCondition(x => x.DateIn.Year == year && x.DateIn.Month == month && x.PersonId == id);
        DateTime targetDate = new DateTime(year, month, 1);
        DateTime endDate = targetDate.AddMonths(1);
        IList<Attendance> attendancelist = new List<Attendance>();
        for(DateTime date = targetDate; date < endDate; date = date.AddDays(1))
        {
            Attendance newlist = new Attendance();
            newlist.DateIn = date;
            var attendance = attendanceData.Where(x => x.DateIn == date).Select(x => new { x.TimeIn, x.TimeOut }).FirstOrDefault();
            if (attendance == null || attendance.TimeIn == attendance.TimeOut)
            {
                if (date < DateTime.Now.Date)
                {

                    newlist.TimeIn = new TimeSpan();
                    newlist.TimeOut = new TimeSpan();
                    newlist.IsActive = false;
                }
            }
            else
            {
                newlist.TimeIn = attendance.TimeIn;
                newlist.TimeOut = attendance.TimeOut;
                newlist.IsActive = true;
            }
            attendancelist.Add(newlist);
        }

        if (string.IsNullOrEmpty(sortGrid.Search))
        {
            data = _repository.Attendances.GetDataByGridCondition(null, sortGrid, attendancelist.AsQueryable());
        }
        else
        {
            data = _repository.Attendances.GetDataByGridCondition(x => x.DateIn.ToString() == sortGrid.Search, sortGrid, attendancelist.AsQueryable());
        }
        data.Add(attendanceData.Count());
        return Ok(data);
    }

        [DisplayName("My Attendance History")]
        [Route("GetWeeklyAttendanceById/{id}/{startDate}/{endDate}")]
        [HttpPost]
        public IActionResult GetWeeklyAttendanceById([FromBody]SortGrid sortGrid, [FromRoute]int id, [FromRoute]DateTime startDate, [FromRoute]DateTime endDate)
        {
            ArrayList data = new ArrayList();
            var attendanceData = _repository.Attendances.FindAllByCondition(x => x.DateIn.Date >= startDate && x.DateIn.Date <= endDate && x.PersonId == id);

            IList<Attendance> attendancelist = new List<Attendance>();
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                Attendance newlist = new Attendance();
                newlist.DateIn = date;
                var attendance = attendanceData.Where(x => x.DateIn == date).Select(x => new { x.TimeIn, x.TimeOut }).FirstOrDefault();
                if (attendance == null || attendance.TimeIn == attendance.TimeOut)
                {
                    if (date < DateTime.Now.Date)
                    {

                        newlist.TimeIn = new TimeSpan();
                        newlist.TimeOut = new TimeSpan();
                        newlist.IsActive = false;
                    }
                }
                else
                {
                    newlist.TimeIn = attendance.TimeIn;
                    newlist.TimeOut = attendance.TimeOut;
                    newlist.IsActive = true;
                }
                attendancelist.Add(newlist);
            }
            if (string.IsNullOrEmpty(sortGrid.Search))
            {
                data = _repository.Attendances.GetDataByGridCondition(null, sortGrid, attendancelist.AsQueryable());
            }
            else
            {
                data = _repository.Attendances.GetDataByGridCondition(x => x.DateIn.ToString() == sortGrid.Search, sortGrid, attendancelist.AsQueryable());
            }

            data.Add(attendanceData.Count());
            return Ok(data);

        }

        [DisplayName("My Attendance Summary")]
        [Route("GetYearlyAttendanceSummaryById/{id}/{year}")]
        [HttpGet]
        public IActionResult GetYearlyAttendanceSummaryById([FromRoute] int year, [FromRoute]int id, [FromRoute]int? month)
        {
            AttendanceReport attendanceReport = new AttendanceReport();
            IQueryable<Attendance> attendanceData;
                attendanceData = _repository.Attendances.FindAllByCondition(x => x.DateIn.Year == year && x.PersonId == id);
                if (DateTime.IsLeapYear(year))
                {
                    attendanceReport.TotalDays = 366;
                }
                else
                {
                    attendanceReport.TotalDays = 365;
                }           
            attendanceReport.PresentDays = attendanceData.Count();
            attendanceReport.AbsentDays = attendanceReport.TotalDays - attendanceReport.PresentDays;
            if (attendanceReport.PresentDays == 0)
            {
                attendanceReport.TimeIn = "-";
                attendanceReport.TimeOut = "-";
                attendanceReport.AverageTime = "-";
            }
            else
            {
                TimeSpan averageTimeIn = new TimeSpan(Convert.ToInt64(attendanceData.Average(x => x.TimeIn.Ticks)));
                DateTime timeIn = DateTime.Today.Add(averageTimeIn);
                attendanceReport.TimeIn = timeIn.ToString("hh:mm tt");

                TimeSpan averageTimeOut = new TimeSpan(Convert.ToInt64(attendanceData.Average(x => x.TimeOut.Ticks)));
                DateTime timeOut = DateTime.Today.Add(averageTimeOut);
                attendanceReport.TimeOut = timeOut.ToString("hh:mm tt");

                var hour = Math.Round(timeOut.Subtract(timeIn).TotalHours, 2);
                attendanceReport.AverageTime = (hour * 0.6).ToString("hh:mm");
            }
            return Ok(attendanceReport);
        }

        [DisplayName("My Attendance Summary")]
        [Route("GetMonthlyAttendanceSummaryById/{id}/{year}/{month}")]
        [HttpGet]
        public IActionResult GetMonthlyAttendanceSummaryById([FromRoute] int year, [FromRoute]int id, [FromRoute]int month)
        {
            AttendanceReport attendanceReport = new AttendanceReport();
            IQueryable<Attendance> attendanceData;
                attendanceData = _repository.Attendances.FindAllByCondition(x => x.DateIn.Year == year && x.DateIn.Month == month && x.PersonId == id);
                attendanceReport.TotalDays = DateTime.DaysInMonth(year, month);
            attendanceReport.PresentDays = attendanceData.Count();
            attendanceReport.AbsentDays = attendanceReport.TotalDays - attendanceReport.PresentDays;
            if (attendanceReport.PresentDays == 0)
            {
                attendanceReport.TimeIn = "-";
                attendanceReport.TimeOut = "-";
                attendanceReport.AverageTime = "-";
            }
            else
            {
                TimeSpan averageTimeIn = new TimeSpan(Convert.ToInt64(attendanceData.Average(x => x.TimeIn.Ticks)));
                DateTime timeIn = DateTime.Today.Add(averageTimeIn);
                attendanceReport.TimeIn = timeIn.ToString("hh:mm tt");

                TimeSpan averageTimeOut = new TimeSpan(Convert.ToInt64(attendanceData.Average(x => x.TimeOut.Ticks)));
                DateTime timeOut = DateTime.Today.Add(averageTimeOut);
                attendanceReport.TimeOut = timeOut.ToString("hh:mm tt");

                var hour = Math.Round(timeOut.Subtract(timeIn).TotalHours, 2);
                attendanceReport.AverageTime = (hour * 0.6).ToString();
            }
            return Ok(attendanceReport);
        }

        [DisplayName("My Attendance Summary")]
        [Route("GetWeeklyAttendanceSummaryById/{id}/{startDate}/{endDate}")]
        [HttpGet]
        public IActionResult GetWeeklySummaryAttendanceById([FromRoute]int id, [FromRoute]DateTime startDate, [FromRoute]DateTime endDate)
        {
            AttendanceReport attendanceReport = new AttendanceReport();
            var attendanceData = _repository.Attendances.FindAllByCondition(x => x.DateIn.Date >= startDate && x.DateIn.Date <= endDate && x.PersonId == id);
            attendanceReport.TotalDays = 7;
            attendanceReport.PresentDays = attendanceData.Count();
            attendanceReport.AbsentDays = attendanceReport.TotalDays - attendanceReport.PresentDays;
            if (attendanceReport.PresentDays == 0)
            {
                attendanceReport.TimeIn = "-";
                attendanceReport.TimeOut = "-";
                attendanceReport.AverageTime = "-";
            }
            else
            {
                TimeSpan averageTimeIn = new TimeSpan(Convert.ToInt64(attendanceData.Average(x => x.TimeIn.Ticks)));
                DateTime timeIn = DateTime.Today.Add(averageTimeIn);
                attendanceReport.TimeIn = timeIn.ToString("hh:mm tt");

                TimeSpan averageTimeOut = new TimeSpan(Convert.ToInt64(attendanceData.Average(x => x.TimeOut.Ticks)));
                DateTime timeOut = DateTime.Today.Add(averageTimeOut);
                attendanceReport.TimeOut = timeOut.ToString("hh:mm tt");

                var hour = Math.Round(timeOut.Subtract(timeIn).TotalHours, 2);
                attendanceReport.AverageTime = (hour * 0.6).ToString();
            }
            return Ok(attendanceReport);

        }
    }
}