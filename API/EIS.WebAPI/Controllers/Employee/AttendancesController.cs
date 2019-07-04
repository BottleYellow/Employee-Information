using EIS.Entities.Employee;
using EIS.Entities.Generic;
using EIS.Entities.Models;
using EIS.Entities.SP;
using EIS.Repositories.IRepository;
using EIS.WebAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace EIS.WebAPI.Controllers
{
    [DisplayName("Attendance Management")]
    [EnableCors("MyPolicy")]
    [Route("api/Attendances")]
    [ApiController]
    public class AttendancesController : BaseController
    {
        public readonly IConfiguration _configuration;
        public AttendancesController(IRepositoryWrapper repository, IConfiguration configuration) : base(repository)
        {
            _configuration = configuration;
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
        [HttpGet("GetAllAttendanceEmpCount/{SearchFor}/{InputOne}/{InputTwo}/{locationId}/{status}")]
        public IActionResult GetAllAttendance([FromRoute] string SearchFor, [FromRoute] string InputOne, [FromRoute] string InputTwo, [FromRoute]int locationId, [FromRoute]bool status)
        {
            Attendance_Report attendanceData = _repository.Attendances.GetAttendanceCountReport(SearchFor, InputOne, InputTwo, locationId, status);
            return Ok(attendanceData);
        }
        [HttpGet("GetAllAttendanceNew/{SearchFor}/{InputOne}/{InputTwo}/{locationId}/{status}")]
        public IActionResult GetAllAttendanceNew([FromRoute] string SearchFor, [FromRoute] string InputOne, [FromRoute] string InputTwo, [FromRoute]int locationId, [FromRoute]bool status)
        {
            Attendance_Report_New attendanceData = _repository.Attendances.GetAttendanceCountReportNew(SearchFor, InputOne, InputTwo, locationId, status);
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
            if (startOfWeek <= sDate)
            {
                eDate = DateTime.Now.Date;
            }
            IEnumerable<Attendance> attendanceData = _repository.Attendances.FindAllByCondition(x => x.DateIn.Date >= sDate && x.DateIn.Date <= eDate && x.PersonId == id);
            List<AttendanceReportByDate> attendancelist = _repository.Attendances.GetAttendanceReportByDate(sDate, eDate, attendanceData, Code, locationId);
            return Ok(attendancelist);
        }
        #endregion

        #region[My Attendance Summary]
        [DisplayName("My Attendance Summary")]
        [HttpGet("GetYearlyAttendanceSummaryById/{id}/{year}")]
        public IActionResult GetYearlyAttendanceSummaryById([FromRoute] int year, [FromRoute]string id)
        {
            EmployeeAttendanceReport attendanceReport = new EmployeeAttendanceReport();
            attendanceReport = _repository.Attendances.GetAttendanceReportSummary("Year", id, year, 0);
            return Ok(attendanceReport);
        }

        [DisplayName("My Attendance Summary")]
        [HttpGet("GetMonthlyAttendanceSummaryById/{id}/{year}/{month}")]
        public IActionResult GetMonthlyAttendanceSummaryById([FromRoute] int year, [FromRoute]string id, [FromRoute]int month)
        {
            EmployeeAttendanceReport attendanceReport = new EmployeeAttendanceReport();
            attendanceReport = _repository.Attendances.GetAttendanceReportSummary("Month", id, year, month);
            return Ok(attendanceReport);
        }

        [DisplayName("My Attendance Summary")]
        [HttpGet("GetWeeklyAttendanceSummaryById/{id}/{startDate}/{endDate}")]
        public IActionResult GetWeeklySummaryAttendanceById([FromRoute]string id, [FromRoute]string startDate, [FromRoute]string endDate)
        {
            AttendanceReport attendanceReport = new AttendanceReport();
            IQueryable<Attendance> attendanceData = _repository.Attendances.FindAllByCondition(x => x.DateIn.Date >= Convert.ToDateTime(startDate) && x.DateIn.Date <= Convert.ToDateTime(endDate) && x.EmployeeCode == id);
            return Ok(attendanceReport);
        }
        #endregion

        #region Datewise Attendance
        [HttpGet("GetDateWiseAttendance/{id}/{LocationId}/{startDate}/{endDate}")]
        public IActionResult GetDateWiseAttendance([FromRoute]string id, [FromRoute]int LocationId, [FromRoute]string startDate, [FromRoute]string endDate)
        {
            List<SP_GetDateWiseAttendance> sP_GetDateWiseAttendance = new List<SP_GetDateWiseAttendance>();
            sP_GetDateWiseAttendance = _repository.Attendances.dateWiseAttendances(id, LocationId, startDate, endDate);
            return Ok(sP_GetDateWiseAttendance);
        }

        [HttpGet]
        [Route("GetAttendanceUpdateData/{status}")]
        public IActionResult GetAttendanceUpdateData([FromRoute]string status)
        {
            List<AttendanceUpdateData> attendances = new List<AttendanceUpdateData>();
            attendances = _repository.Attendances.GetattendanceUpdateData(status);
            return Ok(attendances);
        }

        [HttpGet]
        [Route("AttendanceUpdate/{personId}/{message}/{requestedDate}/{timeIn}/{timeOut}")]
        public IActionResult AttendanceUpdate([FromRoute]string personId, [FromRoute]string message, [FromRoute]string requestedDate, [FromRoute]string timeIn, [FromRoute]string timeOut)
        {
            string status = "";
            int id = Convert.ToInt32(personId);
            DateTime date = Convert.ToDateTime(requestedDate);
            Attendance attendance = _repository.Attendances.FindByCondition(x => x.PersonId == id && x.DateIn == date);
            if (attendance != null)
            {
                if (message == "Approve")
                {
                    DateTime inDate = DateTime.ParseExact(timeIn, "hh:mm tt", CultureInfo.InvariantCulture);
                    TimeSpan inTime = inDate.TimeOfDay;
                    DateTime outDate = DateTime.ParseExact(timeOut, "hh:mm tt", CultureInfo.InvariantCulture);
                    TimeSpan outTime = outDate.TimeOfDay;
                    attendance.TimeIn = inTime;
                    attendance.TimeOut = outTime;
                    attendance.TotalHours = outTime - inTime;
                    attendance.IsActive = true;
                    attendance.HrStatus = "Approved";
                    _repository.Attendances.UpdateAndSave(attendance);
                    status = "success";
                }
                else
                {
                    attendance.Message = "HR MESSAGE: " + message;
                    attendance.HrStatus = "Rejected";
                    _repository.Attendances.UpdateAndSave(attendance);
                    status = "success";
                }
            }
            return Ok(status);
        }

        [HttpGet]
        [Route("DeductFromSalary/{EmployeeCode}/{Dates}/{GrantedLeaves}/{TakenLeaves}/{UnpaidAbsent}")]
        public IActionResult DeductFromSalary([FromRoute]string EmployeeCode, [FromRoute]string Dates, [FromRoute]int GrantedLeaves, [FromRoute]int TakenLeaves, [FromRoute]int UnpaidAbsent)
        {
            int count = Dates.Count(x => x == '&') + 1;
            string[] stringDates = new string[count];
            stringDates = Dates.Split("&");
            Person person = _repository.Employee.FindByCondition(x => x.EmployeeCode == EmployeeCode);
            int personId = person != null ? person.Id : 0;
            string EmailId = person != null ? person.EmailAddress : "";
            string EmpName = person != null ? person.FullName : "";
            bool IsOnProbation = person != null ? Convert.ToBoolean(person.IsOnProbation) : false;
            int DeductedCount = 0;
            foreach (var date in stringDates)
            {
                List<Attendance> list = _repository.Attendances.FindAllByCondition(x => x.DateIn == Convert.ToDateTime(date) && x.HrStatus == "Deducted" && x.EmployeeCode == EmployeeCode).ToList();
                int Count = list != null ? list.Count : 0;
                DateTime dateTime = Convert.ToDateTime(date);
                if (Count == 0)
                {
                    Attendance attendance = new Attendance
                    {
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now,
                        DateIn = dateTime,
                        DateOut = dateTime,
                        TimeIn = new TimeSpan(),
                        EmployeeCode = EmployeeCode,
                        HrStatus = "Deducted",
                        IsActive = false,
                        PersonId = personId
                    };
                    _repository.Attendances.CreateAndSave(attendance);
                    DeductedCount++;
                }
            }
            string To = EmailId;
            string subject = "Deduction of unpaid leaves";
            //Send Mail to Employee
            string bodyForEmployee = "";
            List<SP_LeavePoliciesInDetail> LeavePolicies = _repository.LeaveRequest.GetLeavePoliciesInDetails(personId);
            string policies = "";
            if (LeavePolicies.Count > 0)
            {
                policies = "The details of your leaves are as follows (no. of days) : \n";
                foreach (var policy in LeavePolicies)
                {
                    policies += "" + policy.LeaveType + " : " + policy.LeaveAvailed + "\n";
                }
                policies += UnpaidAbsent > 0 ? "Absent days (Unpaid) : " + UnpaidAbsent + "\n" : null;
                bodyForEmployee = "Dear " + EmpName + ",\n\n" + "This is to inform you that your total leave till date are " + TakenLeaves + "." +
                    "They are been exceeded by " + DeductedCount + " days and hence the  same has been deducted from your salary, which please note.\n\n" + policies + "\n" +
                    "Thanks\n\nRegards,\n\nAadyam Consultants.";
            }
            else
            {
                if (IsOnProbation == true && UnpaidAbsent > 0)
                {
                    bodyForEmployee = "Dear " + EmpName + ",\n\n" + "This is to inform you that you were absent for " + UnpaidAbsent + " days." +
                        "This falls under your probation period and hence it has been deducted from your salary.\n\n" +
                        "Thanks\n\nRegards,\n\nAadyam Consultants.";
                }
            }

            new EmailManager(_configuration, _repository).SendEmail(subject, bodyForEmployee, To, null);
            //Send Mail to Admin and HR
            var results = _repository.Employee.FindAllWithNoTracking().Include(x => x.Role).Where(x => x.Role.Name.ToUpper() == "HR" || x.Role.Name.ToUpper() == "ADMIN")
                 .Select(p => new
                 {
                     p.FullName,
                     p.EmailAddress
                 }).ToList();
            foreach (var p in results)
            {
                To = p.EmailAddress;
                string bodyForHRAndAdmin = "Dear " + p.FullName + ",\n\n" + "This is to inform you that total leaves of " + EmpName +
                    " have been extended against granted (" + GrantedLeaves + " days) leaves, As per records, his/her total leaves are " + TakenLeaves +
                    " and hence the excess leaves, " + DeductedCount + " has been deducted from the salary payment, which please note.\n\n" +
                    "Thanks\n\nRegards,\n\nAadyam Consultants.";
                //new EmailManager(_configuration, _repository).SendEmail(subject, bodyForHRAndAdmin, To, null);
            }
            string status = "fine";
            return Ok(status);
        }

        [HttpGet]
        [Route("DeductOneDayFromSalary/{EmployeeCode}/{Date}")]
        public IActionResult DeductOneDayFromSalary([FromRoute]string EmployeeCode, [FromRoute]string Date)
        {
            List<Attendance> list = _repository.Attendances.FindAllByCondition(x => x.DateIn == Convert.ToDateTime(Date) && x.HrStatus == "Deducted" && x.EmployeeCode == EmployeeCode).ToList();
            int Count = list != null ? list.Count : 0;
            Person person = _repository.Employee.FindByCondition(x => x.EmployeeCode == EmployeeCode);
            int personId = person != null ? person.Id : 0;
            string EmailId = person != null ? person.EmailAddress : "";
            string EmpName = person != null ? person.FullName : "";
            DateTime dateTime = Convert.ToDateTime(Date);
            if (Count == 0)
            {
                Attendance attendance = new Attendance
                {
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    DateIn = dateTime,
                    DateOut = dateTime,
                    TimeIn = new TimeSpan(),
                    EmployeeCode = EmployeeCode,
                    HrStatus = "Deducted",
                    IsActive = false,
                    PersonId = personId
                };

                _repository.Attendances.CreateAndSave(attendance);
            }
            string status = "fine";
            //Send Mail to Employee
            string To = EmailId;
            string subject = "Deduction of unpaid leaves";
            string bodyForEmployee = "Dear " + EmpName + ",\n\n" + "This is to inform you that your unpaid leave of " + dateTime.ToString("dd-MMM-yyyy") +
                " has been deducted from the salary payment, which please note.\n\n" +
                "Thanks\n\nRegards,\n\nAadyam Consultants.";
            new EmailManager(_configuration, _repository).SendEmail(subject, bodyForEmployee, To, null);
            //Send Mail to Admin and HR
            var results = _repository.Employee.FindAllWithNoTracking().Include(x => x.Role).Where(x => x.Role.Name.ToUpper() == "HR" || x.Role.Name.ToUpper() == "ADMIN")
                 .Select(p => new
                 {
                     p.FullName,
                     p.EmailAddress
                 }).ToList();
            foreach (var p in results)
            {
                To = p.EmailAddress;
                string bodyForHRAndAdmin = "Dear " + p.FullName + ",\n\n" + "This is to inform you that unpaid leave of " + dateTime.ToString("dd-MMM-yyyy") +
                " has been deducted from the salary payment of " + EmpName + ", which please note.\n\n" +
                "Thanks\n\nRegards,\n\nAadyam Consultants.";
                //new EmailManager(_configuration, _repository).SendEmail(subject, bodyForHRAndAdmin, To, null);
            }
            return Ok(status);
        }
        #endregion

        #region Leaves in Detail

        [HttpGet("GetLeavesInDetail/{Type}/{InputOne}/{InputTwo}/{EmployeeCode}")]
        public IActionResult GetLeavesInDetail([FromRoute] string Type, [FromRoute] string InputOne, [FromRoute] string InputTwo, [FromRoute]string EmployeeCode)
        {
            LeavesInDetail result = _repository.Attendances.GetLeavesInDetail(Type, InputOne, InputTwo, EmployeeCode);
            return Ok(result);
        }
        #endregion
    }
}


//string bodyForEmployee = "Dear " + EmpName + ",\n\n" + "This is to inform you that your total leaves " +
//    "have been extended against granted (" + GrantedLeaves + " days) leaves, As per records, your total leaves are " + TakenLeaves +
//    " and hence the excess leaves, " + DeductedCount + " has been deducted from the salary payment, which please note.\n\n" +
//    "Thanks\n\nRegards,\n\nAadyam Consultants.";
