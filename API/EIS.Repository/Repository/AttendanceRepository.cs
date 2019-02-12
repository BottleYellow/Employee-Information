using EIS.Data.Context;
using EIS.Entities.Employee;
using EIS.Entities.Models;
using EIS.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EIS.Repositories.Repository
{
    public class AttendanceRepository : RepositoryBase<Attendance>, IAttendanceRepository
    {
        public AttendanceRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public IQueryable<Person> GetAttendanceYearly(int year, int loc)
        {
            if (loc == 0)
            {
                var results = _dbContext.Person.Include(x => x.Location).Include(x => x.Role).Where(x => x.Role.Name != "Admin" && x.Location.IsActive == true)
                .Select(p => new
                {
                    p,
                    Attendances = p.Attendance.Where(a => a.DateIn.Year == year)
                });

                foreach (var x in results)
                {
                    x.p.Attendance = x.Attendances.ToList();
                }
                var result = results.Select(x => x.p);
                return result;
            }
            else
            {
                var results = _dbContext.Person.Where(x => x.LocationId == loc).Include(x => x.Location).Include(x => x.Role).Where(x => x.Role.Name != "Admin" && x.Location.IsActive == true)
                .Select(p => new
                {
                    p,
                    Attendances = p.Attendance.Where(a => a.DateIn.Year == year)
                });

                foreach (var x in results)
                {
                    x.p.Attendance = x.Attendances.ToList();
                }
                var result = results.Select(x => x.p);
                return result;
            }

        }

        public IQueryable<Person> GetAttendanceMonthly(int month, int year, int loc)
        {
            if (loc == 0)
            {
                var results = _dbContext.Person.Include(x => x.Location).Include(x => x.Role).Where(x => x.Role.Name != "Admin" && x.Location.IsActive == true)
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

        public IQueryable<Person> GetAttendanceWeekly(DateTime startOfWeek, DateTime endOfWeek, int loc)
        {
            if (loc == 0)
            {
                var results = _dbContext.Person.Include(x => x.Location).Include(x => x.Role).Where(x => x.Role.Name != "Admin" && x.Location.IsActive == true)
                .Select(p => new
                {
                    p,
                    Attendances = p.Attendance.Where(a => a.DateIn >= startOfWeek && a.DateIn <= endOfWeek)
                });
                foreach (var x in results)
                {
                    x.p.Attendance = x.Attendances.ToList();
                }
                var result = results.Select(x => x.p);
                return result;
            }
            else
            {
                var results = _dbContext.Person.Include(x => x.Location).Include(x => x.Role).Where(x => x.Role.Name != "Admin" && x.Location.IsActive == true && x.LocationId == loc)
                    .Select(p => new
                    {
                        p,
                        Attendances = p.Attendance.Where(a => a.DateIn >= startOfWeek && a.DateIn <= endOfWeek)
                    }).ToList();
                foreach (var x in results)
                {
                    x.p.Attendance = x.Attendances.ToList();
                }
                var result = results.Select(x => x.p);
                return result.AsQueryable();
            }
        }

        public AttendanceReport GetAttendanceReportSummary(int totalDays, int totalWorkingDays, IEnumerable<Attendance> attendanceData)
        {
            AttendanceReport attendanceReport = new AttendanceReport();

            attendanceReport.TotalWorkingDays = totalWorkingDays;
            attendanceReport.TotalDays = totalDays;
            attendanceReport.PresentDays = attendanceData.Count();
            attendanceReport.AbsentDays = attendanceReport.TotalWorkingDays - attendanceReport.PresentDays;
            if (attendanceReport.PresentDays == 0)
            {
                attendanceReport.TimeIn = "-";
                attendanceReport.TimeOut = "-";
                attendanceReport.AverageTime = "-";
                attendanceReport.AdditionalWorkingHours = "-";
            }
            else
            {

                TimeSpan averageTimeIn = new TimeSpan(Convert.ToInt64(attendanceData.Average(x => x.TimeIn.Ticks)));
                DateTime timeIn = DateTime.Today.Add(averageTimeIn);
                attendanceReport.TimeIn = timeIn.ToString("hh:mm tt");

                IEnumerable<Attendance> attendanceTimeOutData = attendanceData.Where(x => x.TimeOut != null);
                if (attendanceTimeOutData != null && attendanceTimeOutData.Count() > 0)
                {
                    TimeSpan averageTimeOut = new TimeSpan(Convert.ToInt64(attendanceTimeOutData.Average(x => x.TimeOut.GetValueOrDefault().Ticks)));
                    DateTime timeOut = DateTime.Today.Add(averageTimeOut);
                    attendanceReport.TimeOut = timeOut.ToString("hh:mm tt");
                }
                else
                {
                    attendanceReport.TimeOut = "-";
                }
                IEnumerable<Attendance> attendanceTotalHoursData = attendanceData.Where(x => x.TotalHours != null);
                if (attendanceTotalHoursData != null && attendanceTotalHoursData.Count() > 0)
                {
                    TimeSpan averageHour = new TimeSpan(Convert.ToInt64(attendanceTotalHoursData.Average(x => x.TotalHours.GetValueOrDefault().Ticks)));
                    DateTime avgHour = DateTime.Today.Add(averageHour);
                    attendanceReport.AverageTime = avgHour.ToString("HH:mm");
                    if (avgHour > new DateTime(2000, 1, 1, 9, 0, 0))
                    {
                        TimeSpan additionalHours = averageHour - new TimeSpan(9, 0, 0);
                        TimeSpan result = TimeSpan.FromTicks(additionalHours.Ticks * attendanceTotalHoursData.Count());
                        attendanceReport.AdditionalWorkingHours = (int)result.TotalHours + ":" + result.Minutes;
                    }
                }
                else
                {
                    attendanceReport.AverageTime = "-";
                    attendanceReport.AdditionalWorkingHours = "-";
                }
            }
            return attendanceReport;
        }

        public IEnumerable<Attendance> GetAttendanceReportByDate(DateTime startDate, DateTime endDate, IQueryable<Attendance> attendanceData)
        {
            IList<Attendance> attendances = new List<Attendance>();
            for (DateTime date = startDate; date < endDate; date = date.AddDays(1))
            {
                Attendance newlist = new Attendance();
                newlist.DateIn = date;
                var attendance = attendanceData.Where(x => x.DateIn == date).Select(x => new { x.TimeIn, x.TimeOut, x.TotalHours }).FirstOrDefault();
                if (attendance == null || attendance.TimeIn == attendance.TimeOut)
                {
                    newlist.TimeIn = new TimeSpan();
                    newlist.TimeOut = new TimeSpan();
                    newlist.IsActive = false;
                    newlist.TotalHours = new TimeSpan();
                }
                else
                {
                    newlist.TimeIn = attendance.TimeIn;
                    newlist.TimeOut = attendance.TimeOut;
                    newlist.IsActive = true;
                    newlist.TotalHours = (attendance.TimeOut == null) ? new TimeSpan() : newlist.TotalHours;
                }
                attendances.Add(newlist);
            }
            IEnumerable<Attendance> attendancelist = attendances;
            return attendancelist;
        }
    }
}