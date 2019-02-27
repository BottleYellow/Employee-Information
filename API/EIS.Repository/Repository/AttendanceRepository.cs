using EIS.Data.Context;
using EIS.Entities.Employee;
using EIS.Entities.Models;
using EIS.Entities.SP;
using EIS.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace EIS.Repositories.Repository
{
    public class AttendanceRepository : RepositoryBase<Attendance>, IAttendanceRepository
    {
        public AttendanceRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public Attendance_Report GetAttendanceCountReport(string SearchFor, string InputOne, string InputTwo, int locationId)
        {
            SearchFor = SearchFor.ToString();
            InputOne = InputOne.ToString();
            InputTwo = InputTwo.ToString();

            IList<AttendanceData> attendanceData = new List<AttendanceData>();
            Attendance_Report Model = new Attendance_Report();
            Model.sP_GetAttendanceCountReports = new List<SP_GetAttendanceCountReport>();

            var SP_locationId = new SqlParameter("@locationId", locationId);
            var SP_SelectType = new SqlParameter("@SelectType", SearchFor);
            var SP_InputOne = new SqlParameter("@InputOne", InputOne);
            var SP_InputTwo = new SqlParameter("@InputTwo", InputTwo);
            string usp = "LMS.usp_GetAttendanceCountReport @locationId, @SelectType, @InputOne, @InputTwo";
            Model.sP_GetAttendanceCountReports = _dbContext._sp_GetAttendanceCountReport.FromSql(usp, SP_locationId, SP_SelectType, SP_InputOne, SP_InputTwo).ToList();

            return Model;
        }

        public EmployeeAttendanceReport GetAttendanceReportSummary(string type, int PersonId, int year, int? month)
        {
            string InputOne = year.ToString();
            char c = '0';
            string InputTwo = month.ToString().PadLeft(2, c);

            EmployeeAttendanceReport Model = new EmployeeAttendanceReport();
            Model._SP_ReportCount = new AttendanceReport();
            Model._SP_AttendanceData = new List<EmployeeAttendanceData>();
            var SP_SelectType = new SqlParameter("@SelectType", type);
            var SP_PersonId = new SqlParameter("@PersonId", PersonId);
            var SP_InputOne = new SqlParameter("@InputOne", InputOne);
            var SP_InputTwo = new SqlParameter("@InputTwo", InputTwo);
            string usp = "LMS.usp_GetEmployeewiseAttendanceCount @PersonId, @SelectType, @InputOne, @InputTwo";
            Model._SP_ReportCount = _dbContext._sp_GetEmployeeAttendanceCount.FromSql(usp, SP_PersonId, SP_SelectType, SP_InputOne, SP_InputTwo).FirstOrDefault();
            usp = "LMS.usp_GetEmployeewiseAttendanceData @PersonId, @SelectType, @InputOne, @InputTwo";
            Model._SP_AttendanceData = _dbContext._sp_GetEmployeeAttendanceData.FromSql(usp, SP_PersonId, SP_SelectType, SP_InputOne, SP_InputTwo).ToList();
            
            return Model;
        }

        public List<AttendanceReportByDate> GetAttendanceReportByDate(DateTime startDate, DateTime endDate, IEnumerable<Attendance> attendanceData, string id, int? loc)
        {
            List<AttendanceReportByDate> attendances = new List<AttendanceReportByDate>();
            var holidays = _dbContext.Holidays.ToList();
            if (id == "0")
            {
                for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                {

                    List<Person> Emps = loc == 0 ? _dbContext.Person.Include(x => x.Role).Include(x => x.LeaveRequests).Where(x => x.Role.Name != "Admin").ToList() : _dbContext.Person.Include(x => x.Role).Include(x => x.LeaveRequests).Where(x => x.Role.Name != "Admin" && x.LocationId == loc).ToList();
                    foreach (var person in Emps)
                    {

                        AttendanceReportByDate attendance = new AttendanceReportByDate();
                        attendance.Date = date.ToShortDateString();
                        attendance.EmployeeCode = person.EmployeeCode;
                        attendance.EmployeeName = person.FullName;
                        int locationId = person.LocationId.GetValueOrDefault();
                        var attendancedata = attendanceData.Where(x => x.DateIn == date && x.PersonId == person.Id).Select(x => new { x.TimeIn, x.TimeOut, x.TotalHours }).FirstOrDefault();
                        if (attendancedata == null)
                        {
                            attendance.TimeIn = "-";
                            attendance.TimeOut = "-";
                            attendance.TotalHours = "-";
                            var holiday = holidays.Where(x => x.Date == date && x.LocationId == locationId).FirstOrDefault();
                            if (holiday == null)
                            {
                                if (date.DayOfWeek == DayOfWeek.Sunday)
                                {
                                    attendance.Status = "Weekly Off";
                                }
                                else
                                {
                                    if (locationId == 2 && date.DayOfWeek == DayOfWeek.Saturday)
                                    {
                                        string alternateDateStatus = CalculateDate(date);
                                        if (!string.IsNullOrEmpty(alternateDateStatus))
                                        {
                                            attendance.Status = alternateDateStatus;
                                        }
                                        else
                                        {
                                            var leaveData = person.LeaveRequests.Where(x => date >= x.FromDate && date <= x.ToDate && x.Status == "Approved").FirstOrDefault();
                                            if (leaveData == null)
                                            {
                                                attendance.Status = "Absent";
                                            }
                                            else
                                            {
                                                attendance.Status = "On Leave";
                                            }

                                        }

                                    }
                                    else
                                    {
                                        var leaveData = person.LeaveRequests.Where(x => date >= x.FromDate && date <= x.ToDate && x.Status == "Approved").FirstOrDefault();
                                        if (leaveData == null)
                                        {
                                            attendance.Status = "Absent";
                                        }
                                        else
                                        {
                                            attendance.Status = "On Leave";
                                        }
                                    }
                                }
                            }
                            else
                            {
                                attendance.Status = holiday.Vacation;
                            }
                        }
                        else
                        {
                            attendance.TimeIn = attendancedata.TimeIn.ToString();
                            attendance.TimeOut = attendancedata.TimeOut == null ? "-" : attendancedata.TimeOut.ToString();
                            attendance.Status = "Present";
                            attendance.TotalHours = attendancedata.TotalHours == null ? "-" : attendancedata.TotalHours.ToString();
                        }
                        attendances.Add(attendance);
                    }
                }
            }
            else
            {
                Person person = _dbContext.Person.Include(x => x.LeaveRequests).Where(x => x.EmployeeCode == id).FirstOrDefault();
                for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    AttendanceReportByDate attendance = new AttendanceReportByDate();

                    attendance.Date = date.ToShortDateString();
                    attendance.EmployeeCode = person.EmployeeCode;
                    attendance.EmployeeName = person.FullName;
                    var attendancedata = attendanceData.Where(x => x.DateIn == date && x.PersonId == person.Id).Select(x => new { x.TimeIn, x.TimeOut, x.TotalHours }).FirstOrDefault();
                    if (attendancedata == null)
                    {
                        var holiday = holidays.Where(x => x.Date == date).FirstOrDefault();
                        if (holiday == null)
                        {
                            attendance.TimeIn = "-";
                            attendance.TimeOut = "-";
                            attendance.TotalHours = "-";
                            if (date.DayOfWeek == DayOfWeek.Sunday)
                            {
                                attendance.Status = "Weekly Off";
                            }
                            else
                            {
                                if (loc == 2 && date.DayOfWeek == DayOfWeek.Saturday)
                                {
                                    string alternateDateStatus = CalculateDate(date);
                                    if (!string.IsNullOrEmpty(alternateDateStatus))
                                    {
                                        attendance.Status = alternateDateStatus;
                                    }
                                    else
                                    {
                                        var leaveData = person.LeaveRequests.Where(x => date >= x.FromDate && date <= x.ToDate && x.Status == "Approved").FirstOrDefault();
                                        if (leaveData == null)
                                        {
                                            attendance.Status = "Absent";
                                        }
                                        else
                                        {
                                            attendance.Status = "On Leave";
                                        }
                                    }
                                }
                                else
                                {
                                    var leaveData = person.LeaveRequests.Where(x => date >= x.FromDate && date <= x.ToDate && x.Status == "Approved").FirstOrDefault();
                                    if (leaveData == null)
                                    {
                                        attendance.Status = "Absent";
                                    }
                                    else
                                    {
                                        attendance.Status = "On Leave";
                                    }
                                }
                            }
                        }
                        else
                        {
                            attendance.Status = holiday.Vacation;
                        }
                    }
                    else
                    {
                        attendance.TimeIn = attendancedata.TimeIn.ToString();
                        attendance.TimeOut = attendancedata.TimeOut == null ? "-" : attendancedata.TimeOut.ToString();
                        attendance.Status = "Present";
                        attendance.TotalHours = attendancedata.TotalHours == null ? "-" : attendancedata.TotalHours.ToString();
                    }
                    attendances.Add(attendance);

                }
            }
            return attendances;
        }


        public IQueryable<Person> GetAttendanceMonthlyReport(int month, int year, int loc)
        {
            if (loc == 0)
            {
                var results = _dbContext.Person.Include(x => x.Location).Include(x => x.Role).Where(x => x.Role.Name != "Admin" && x.Location.IsActive == true).Include(x => x.LeaveRequests)
                .Select(p => new
                {
                    p,
                    Attendances = p.Attendance.Where(a => a.DateIn.Year == year && a.DateIn.Month == month)
                }).ToList();
                foreach (var x in results)
                {
                    x.p.Attendance = x.Attendances.ToList();
                }
                var result = results.Select(x => x.p).ToList();
                return result.AsQueryable();
            }
            else
            {
                var results = _dbContext.Person.Where(x => x.LocationId == loc).Include(x => x.Location).Include(x => x.Role).Where(x => x.Role.Name != "Admin" && x.Location.IsActive == true)
                    .Select(p => new
                    {
                        p,
                        Attendances = p.Attendance.Where(a => a.DateIn.Year == year && a.DateIn.Month == month)
                    }).ToList();
                foreach (var x in results)
                {
                    x.p.Attendance = x.Attendances.ToList();
                }
                var result = results.Select(x => x.p).ToList();
                return result.AsQueryable();
            }
        }

        public string CalculateDate(DateTime date)
        {
            string data = "";
            int mon = date.Month;
            int yea = date.Year;
            var dat = 1;
            DateTime myDate = new DateTime(yea, mon, dat);
            string first = myDate.DayOfWeek.ToString();
            DateTime secnd = new DateTime();
            DateTime forth = new DateTime();

            switch (first)
            {
                case "Sunday":
                    secnd = new DateTime(yea, mon, 14);
                    forth = new DateTime(yea, mon, 28);
                    break;
                case "Monday":
                    secnd = new DateTime(yea, mon, 13);
                    forth = new DateTime(yea, mon, 27);
                    break;
                case "Tuesday":
                    secnd = new DateTime(yea, mon, 12);
                    forth = new DateTime(yea, mon, 26);
                    break;
                case "Wednesday":
                    secnd = new DateTime(yea, mon, 11);
                    forth = new DateTime(yea, mon, 25);
                    break;
                case "Thursday":
                    secnd = new DateTime(yea, mon, 10);
                    forth = new DateTime(yea, mon, 24);
                    break;
                case "Friday":
                    secnd = new DateTime(yea, mon, 9);
                    forth = new DateTime(yea, mon, 23);
                    break;
                case "Saturday":
                    secnd = new DateTime(yea, mon, 8);
                    forth = new DateTime(yea, mon, 22);
                    break;
                default: break;
            }
            if (date == secnd)
            {
                data = "2nd Saturday Weekly Off";
            }
            else if (date == forth)
            {
                data = "4th Saturday Weekly Off";
            }
            return data;
        }
    }
}
