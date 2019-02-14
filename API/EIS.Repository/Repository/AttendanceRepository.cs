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

        public IList<AttendanceData> GetAttendanceYearly(int year, int loc)
        {
            IList<AttendanceData> attendanceData = new List<AttendanceData>();
            if (loc == 0)
            {
                var results = _dbContext.Person.Include(x => x.Location).Include(x => x.Role).Where(x => x.Role.Name != "Admin" && x.Location.IsActive == true)
                .Select(p => new
                {
                    Location = p.Location.LocationName,
                    Name = p.FullName,
                    AttendanceCount = p.Attendance.Where(a => a.DateIn.Year == year ).Count(),
                    OnLeave = p.LeaveRequests.Where(a => a.AppliedDate.Year == year).Sum(x => x.ApprovedDays),
                    TotalWorkingDays = 365
                });

                foreach (var x in results)
                {
                    AttendanceData attendance = new AttendanceData
                    {
                        Location = x.Location,
                        Name = x.Name,
                        PresentDays = x.AttendanceCount,
                        OnLeave = (int)x.OnLeave,
                        TotalWorkingDays = x.TotalWorkingDays
                    };
                    attendanceData.Add(attendance);
                }
                return attendanceData;
            }
            else
            {
                var results = _dbContext.Person.Where(x=>x.LocationId==loc).Include(x => x.Location).Include(x => x.Role).Where(x => x.Role.Name != "Admin" && x.Location.IsActive == true)
                .Select(p => new
                {
                    Location = p.Location.LocationName,
                    Name = p.FullName,
                    AttendanceCount = p.Attendance.Where(a => a.DateIn.Year == year).Count(),
                    OnLeave = p.LeaveRequests.Where(a => a.AppliedDate.Year == year).Sum(x => x.ApprovedDays),
                    TotalWorkingDays = 365
                });

                foreach (var x in results)
                {
                    AttendanceData attendance = new AttendanceData
                    {
                        Location = x.Location,
                        Name = x.Name,
                        PresentDays = x.AttendanceCount,
                        OnLeave = (int)x.OnLeave,
                        TotalWorkingDays = x.TotalWorkingDays
                    };
                    attendanceData.Add(attendance);
                }
                return attendanceData;
            }
        }

        public IList<AttendanceData> GetAttendanceMonthly(int month, int year, int loc)
        {
            IList<AttendanceData> attendanceData=new List<AttendanceData>();
            if (loc == 0)
            {
                var results = _dbContext.Person.Include(x => x.Location).Include(x => x.Role).Where(x => x.Role.Name != "Admin" && x.Location.IsActive == true).Include(x => x.LeaveRequests)
                .Select(p => new
                {
                    Location = p.Location.LocationName,
                    Name = p.FullName,
                    AttendanceCount = p.Attendance.Where(a => a.DateIn.Year == year && a.DateIn.Month == month).Count(),
                    OnLeave = p.LeaveRequests.Where(a => a.AppliedDate.Year == year && a.AppliedDate.Month == month).Sum(x => x.ApprovedDays),
                    TotalWorkingDays = DateTime.DaysInMonth(year, month)
                }).ToList();
                foreach (var x in results)
                {
                    AttendanceData attendance = new AttendanceData
                    {
                        Location = x.Location,
                        Name = x.Name,
                        PresentDays = x.AttendanceCount,
                        OnLeave = (int)x.OnLeave,
                        TotalWorkingDays = x.TotalWorkingDays
                    };
                    attendanceData.Add(attendance);
            }

                return attendanceData;
            }
            else
            {
                var results = _dbContext.Person.Where(x=>x.LocationId==loc).Include(x => x.Location).Include(x => x.Role).Where(x => x.Role.Name != "Admin" && x.Location.IsActive == true)
                    .Select(p => new
                    {
                        Location = p.Location.LocationName,
                        Name = p.FullName,
                        AttendanceCount = p.Attendance.Where(a => a.DateIn.Year == year && a.DateIn.Month == month).Count(),
                        OnLeave = p.LeaveRequests.Where(a => a.AppliedDate.Year == year && a.AppliedDate.Month == month).Sum(x => x.ApprovedDays),
                        TotalWorkingDays = DateTime.DaysInMonth(year, month)
                    }).ToList();

                foreach (var x in results)
                {
                    AttendanceData attendance = new AttendanceData
                    {
                        Location = x.Location,
                        Name = x.Name,
                        PresentDays = x.AttendanceCount,
                        OnLeave = (int)x.OnLeave,
                        TotalWorkingDays = x.TotalWorkingDays
                    };
                    attendanceData.Add(attendance);
                }
                return attendanceData;
            }
        }
   
        public IList<AttendanceData> GetAttendanceWeekly(DateTime startOfWeek, DateTime endOfWeek, int loc)
        {
            IList<AttendanceData> attendanceData = new List<AttendanceData>();
            if (loc == 0)
            {
                var results = _dbContext.Person.Include(x => x.Location).Include(x => x.Role).Where(x => x.Role.Name != "Admin" && x.Location.IsActive == true)
                .Select(p => new
                {
                    Location = p.Location.LocationName,
                    Name = p.FullName,
                    AttendanceCount = p.Attendance.Where(a => a.DateIn >= startOfWeek && a.DateIn <= endOfWeek).Count(),
                    OnLeave = p.LeaveRequests.Where(a => a.AppliedDate.Date >= startOfWeek && a.AppliedDate.Date <= endOfWeek).Sum(x => x.ApprovedDays),
                    TotalWorkingDays = 7
                });
                foreach (var x in results)
                {
                    AttendanceData attendance = new AttendanceData
                    {
                        Location = x.Location,
                        Name = x.Name,
                        PresentDays = x.AttendanceCount,
                        OnLeave = (int)x.OnLeave,
                        TotalWorkingDays = x.TotalWorkingDays
                    };
                    attendanceData.Add(attendance);
                }
                return attendanceData;
            }
            else
            {
                var results = _dbContext.Person.Include(x => x.Location).Include(x => x.Role).Where(x => x.Role.Name != "Admin" && x.Location.IsActive == true && x.LocationId==loc)
                    .Select(p => new
                    {
                        Location = p.Location.LocationName,
                        Name = p.FullName,
                        AttendanceCount = p.Attendance.Where(a => a.DateIn >= startOfWeek && a.DateIn <= endOfWeek).Count(),
                        OnLeave = p.LeaveRequests.Where(a => a.AppliedDate.Date >= startOfWeek && a.AppliedDate.Date <= endOfWeek).Sum(x => x.ApprovedDays),
                        TotalWorkingDays = 7
                    }).ToList();
                foreach (var x in results)
                {
                    AttendanceData attendance = new AttendanceData
                    {
                        Location = x.Location,
                        Name = x.Name,
                        PresentDays = x.AttendanceCount,
                        OnLeave = (int)x.OnLeave,
                        TotalWorkingDays = x.TotalWorkingDays
                    };
                    attendanceData.Add(attendance);
                }
                return attendanceData;
            }
        }

        public AttendanceReport GetAttendanceReportSummary(int totalDays,int totalWorkingDays, IEnumerable<Attendance> attendanceData)
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

                IEnumerable<Attendance> attendanceTimeOutData = attendanceData.Where(x => x.TimeOut != null && x.TotalHours != null);
                if (attendanceTimeOutData != null && attendanceTimeOutData.Count() > 0)
                {
                    TimeSpan averageTimeOut = new TimeSpan(Convert.ToInt64(attendanceTimeOutData.Average(x => x.TimeOut.GetValueOrDefault().Ticks)));
                    DateTime timeOut = DateTime.Today.Add(averageTimeOut);
                    attendanceReport.TimeOut = timeOut.ToString("hh:mm tt");
                    TimeSpan averageHour = new TimeSpan(Convert.ToInt64(attendanceTimeOutData.Average(x => x.TotalHours.GetValueOrDefault().Ticks)));
                    DateTime avgHour = DateTime.Today.Add(averageHour);
                    attendanceReport.AverageTime = avgHour.ToString("HH:mm");
                    if (averageHour > new TimeSpan(9, 0, 0))
                    {
                        TimeSpan additionalHours = averageHour - new TimeSpan(9, 0, 0);
                        TimeSpan result = TimeSpan.FromTicks(additionalHours.Ticks * attendanceTimeOutData.Count());
                        attendanceReport.AdditionalWorkingHours = (int)result.TotalHours + ":" + result.Minutes;
                    }else
                    {
                        attendanceReport.AdditionalWorkingHours = "-";
                    }
                }
                else
                {
                    attendanceReport.TimeOut = "-";
                    attendanceReport.AverageTime = "-";
                    attendanceReport.AdditionalWorkingHours = "-";
                }              
            }
            return attendanceReport;
        }

        public List<AttendanceReportByDate> GetAttendanceReportByDate(DateTime startDate, DateTime endDate, IEnumerable<Attendance> attendanceData)
        {
            
            List<AttendanceReportByDate> attendances = new List<AttendanceReportByDate>();
            for (DateTime date = startDate; date < endDate; date = date.AddDays(1))
            {
                AttendanceReportByDate attendance = new AttendanceReportByDate();
                attendance.Date = date.ToShortDateString();
                var attendancedata = attendanceData.Where(x => x.DateIn == date).Select(x => new { x.TimeIn, x.TimeOut, x.TotalHours }).FirstOrDefault();
                if (attendancedata == null || attendancedata.TimeIn == attendancedata.TimeOut)
                {
                    if (date.DayOfWeek == DayOfWeek.Sunday)
                    {
                        attendance.Status = "Weekly Off";
                    }
                    else
                    {
                        attendance.Status = "-";
                    }
                        attendance.TimeIn = "-";
                    attendance.TimeOut = "-";
                    
                    attendance.TotalHours = "-";
                }
                else
                {
                    attendance.TimeIn = attendancedata.TimeIn.ToString();
                    attendance.TimeOut = attendancedata.TimeOut==null? new TimeSpan().ToString(): attendancedata.TimeOut.ToString();
                    attendance.Status = "Present";
                    attendance.TotalHours = attendancedata.TotalHours == null ? new TimeSpan().ToString() : attendancedata.TotalHours.ToString();
                }
                attendances.Add(attendance);
            }
            return attendances;
        }
    }
}
