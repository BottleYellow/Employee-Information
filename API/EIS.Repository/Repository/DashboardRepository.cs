using EIS.Data.Context;
using EIS.Entities.Dashboard;
using EIS.Entities.Employee;
using EIS.Entities.Hoildays;
using EIS.Entities.Leave;
using EIS.Entities.Models;
using EIS.Entities.User;
using EIS.Repositories.Helpers;
using EIS.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace EIS.Repositories.Repository
{
    public class DashboardRepository : RepositoryBase<AdminDashboard>, IDashboardRepository
    {
        public DashboardRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public AdminDashboard GetAdminDashboard(string attendanceStatus, int location, int TenantId)
        {
            List<LeaveRequest> leaveRequests = new List<LeaveRequest>();
            List<Person> employees = new List<Person>();
            int pcount = 0;
            int leaves = 0;
            leaveRequests = location == 0 ? _dbContext.LeaveRequests.Where(x => x.Status == "Pending" && x.TenantId == TenantId).ToList() : _dbContext.LeaveRequests.Include(x => x.Person).Where(x => x.Status == "Pending" && x.TenantId == TenantId && x.Person.LocationId == location).ToList();
            leaves = leaveRequests.Count();
            var results = _dbContext.Person.Include(x => x.Role).Include(y => y.Location).Where(x => x.Role.Name != "Admin")
           .Select(p => new
           {
               p,
               Attendances = p.Attendance.Where(a => a.DateIn.Date == DateTime.Now.Date)
           });

            foreach (var x in results)
            {
                x.p.Attendance = x.Attendances.ToList();
            }
            var result =location==0? results.Select(x => x.p).ToList(): results.Select(x => x.p).Where(x=>x.LocationId==location).ToList();

            int totalCount = result.Count(); ;

            if (attendanceStatus == "Present")
            {
                result = result.Where(x => x.Attendance != null && x.Attendance.Count() > 0).ToList();
                pcount = result.Count();
            }
            else
            if (attendanceStatus == "Absent")
            {
                result = result.Where(x => x.Attendance.Count() == 0).ToList();
                pcount = totalCount - result.Count();
            }
            else
            {
                pcount = result.Where(x => x.Attendance != null && x.Attendance.Count() > 0).Count();
            }

            AdminDashboard dashboard = new AdminDashboard
            {
                AllEmployeesCount = totalCount,
                PresentEmployees = pcount,
                AbsentEmployees = totalCount - pcount,
                PendingLeavesCount = leaves,
                Employees = result,
                LeaveRequests = leaveRequests
            };
            return dashboard;
        }

        public ManagerDashboard GetManagerDashboard(int TenantId)
        {
            var employees = _dbContext.Person.Include(x => x.Attendance).Where(x => x.TenantId == TenantId && x.IsActive == true).AsQueryable();
            var leaves = _dbContext.LeaveRequests.Where(x => x.Status == "Pending" && x.TenantId == TenantId).Count();
            int pcount = 0;
            foreach (var pa in employees)
            {
                foreach (var a in pa.Attendance)
                {
                    if (a.DateIn.Date == DateTime.Now.Date)
                    {
                        pcount++;
                    }
                }
            }
            ManagerDashboard dashboard = new ManagerDashboard
            {

            };


            return dashboard;
        }
        public EmployeeDashboard GetEmployeeDashboard(int TenantId, int PersonId)
        {
            int currentMonth = new DateTime().Month;
            int currentYear = new DateTime().Year;
            int TotalDays = DateTime.DaysInMonth(currentYear, currentMonth);
            var attendance = _dbContext.Attendances.Where(x => x.PersonId == PersonId && x.DateIn.Month == currentMonth && x.DateIn.Year == currentYear);
            var leaves = _dbContext.LeaveRequests.Where(x => x.PersonId == PersonId && x.TenantId == TenantId && x.Status == "Approved").Sum(x => x.RequestedDays);
            var availableLeaves = _dbContext.LeaveCredit.Where(x => x.PersonId == PersonId).Sum(x => x.Available);

            EmployeeDashboard dashboard = new EmployeeDashboard
            {
                MonthlyPresentDays = attendance.Count(),
                MonthlyAbsentDays = TotalDays - attendance.Count(),
                TotalLeavesAvailable = Convert.ToInt32(availableLeaves),
                TotalLeavesTaken = Convert.ToInt32(leaves)
            };
            return dashboard;
        }


        public List<CalendarData> GetCalendarDetails(int location,DateTime beginDate, DateTime stopDate)
        {
            List<CalendarData> calendarDataList = new List<CalendarData>();
            IEnumerable<Holiday> holidays = new List<Holiday>();
            IEnumerable<LeaveRequest> leaveList = new List<LeaveRequest>();

            if (location==0)
            {
               holidays = _dbContext.Holidays.ToList();
                leaveList = _dbContext.LeaveRequests.ToList();
            }
            else
            {
                holidays = _dbContext.Holidays.Where(x => x.LocationId == location).ToList();
                leaveList = _dbContext.LeaveRequests.Include(x => x.Person).Where(x => x.Person.Location.Id == location).ToList();             
            }

            int count = 0;
            for (DateTime date = beginDate; date < stopDate; date = date.AddDays(1))
            {
                LeaveRequest leaveRequest = leaveList.Where(x => x.FromDate == date).FirstOrDefault();
                if (leaveRequest != null)
                {
                    CalendarData calendarData = new CalendarData();
                    string leave = "";
                    if (leaveRequest.LeaveType == "Casual Leave")
                    {
                        leave = "CL";
                    }
                    calendarData.Title = leaveRequest.EmployeeName + " (" + leave + "-" + leaveRequest.Status + ")";
                    calendarData.Description = "Leave Status " + leaveRequest.Status;
                    calendarData.StartDate = leaveRequest.FromDate;
                    calendarData.EndDate = leaveRequest.ToDate;
                    if (leaveRequest.Status == "Pending")
                    {
                        calendarData.Color = "Orange";
                    }
                    else
                    {
                        calendarData.Color = "Blue";
                    }
                    calendarData.IsFullDay = true;
                    calendarDataList.Add(calendarData);
                }

                Holiday holiday = holidays.Where(x => x.Date == date).FirstOrDefault();
                if (holiday != null)
                {
                    CalendarData calendarData = new CalendarData();
                    calendarData.Title = holiday.Vacation;
                    calendarData.Description = "Holiday due to " + holiday.Vacation;
                    calendarData.StartDate = holiday.Date;
                    calendarData.EndDate = holiday.Date;
                    calendarData.Color = "Red";
                    calendarData.IsFullDay = true;

                    calendarDataList.Add(calendarData);
                }
                if (date.Day == 1)
                {
                    count = 0;
                }
                if (date.DayOfWeek == DayOfWeek.Sunday)
                {
                    CalendarData holidayCalanderData = new CalendarData();
                    holidayCalanderData.Title = "Weekly Off";
                    holidayCalanderData.Description = "Weekly Off";
                    holidayCalanderData.StartDate = date;
                    holidayCalanderData.EndDate = date;
                    holidayCalanderData.Color = "Orange";
                    holidayCalanderData.IsFullDay = true;
                    calendarDataList.Add(holidayCalanderData);

                }

                if (date.DayOfWeek == DayOfWeek.Saturday)
                {
                    count++;
                    if (count % 2 == 0)
                    {
                        CalendarData holidayCalanderData = new CalendarData();
                        holidayCalanderData.Title = count + "nd Saturday Holiday";
                        holidayCalanderData.Description = "Holiday";
                        holidayCalanderData.StartDate = date;
                        holidayCalanderData.EndDate = date;
                        holidayCalanderData.Color = "Orange";
                        holidayCalanderData.IsFullDay = true;
                        calendarDataList.Add(holidayCalanderData);
                    }
                }


                var results = _dbContext.Person.Include(x => x.Role).Where(x => x.Role.Name != "Admin").Include(y => y.Location)
                           .Select(p => new
                           {
                               p,
                               Attendances = p.Attendance.Where(a => a.DateIn== date)
                           });

                foreach (var x in results)
                {
                    x.p.Attendance = x.Attendances.ToList();
                }
                var result = results.Select(x => x.p).ToList();

                int absentCount = 0;
                int presentCount = 0;
                   var resultPresent = location == 0 ? result.Where(x => x.Attendance != null && x.Attendance.Count() > 0).ToList() : result.Where(x => x.LocationId == location && x.Attendance != null && x.Attendance.Count() > 0).ToList();
                presentCount = resultPresent.Count();
                CalendarData CalanderData10 = new CalendarData();
                CalanderData10.Title = "Present Count:" + presentCount;
                CalanderData10.StartDate = date;
                CalanderData10.EndDate = date;
                CalanderData10.Color = "Green";
                CalanderData10.IsFullDay = true;
                StringBuilder sb1 = new StringBuilder();
                foreach (var d in resultPresent)
                {
                    if (d != null)
                    {
                        sb1.AppendLine("<br/>");
                        sb1.AppendLine(d.FullName + " (" + d.Attendance.FirstOrDefault().TimeIn.ToString(@"hh\:mm") + "-" + d.Attendance.FirstOrDefault().TimeOut.GetValueOrDefault(new TimeSpan()).ToString(@"hh\:mm") + ") Working Hours-" + d.Attendance.FirstOrDefault().TotalHours.GetValueOrDefault(new TimeSpan()).ToString(@"hh\:mm"));
                    }
                }
                CalanderData10.Description = sb1.ToString();
                calendarDataList.Add(CalanderData10);
        
                   var resultAbsent = location == 0 ? result.Where(x => x.Attendance.Count() == 0).ToList() : result.Where(x => x.LocationId == location && x.Attendance.Count() == 0).ToList();
                absentCount = resultAbsent.Count();

                CalendarData CalanderData11 = new CalendarData();
                CalanderData11.Title = "Absent Count:" + absentCount;
                CalanderData11.StartDate = date;
                CalanderData11.EndDate = date;
                CalanderData11.Color = "Red";
                CalanderData11.IsFullDay = true;
                StringBuilder sb2 = new StringBuilder();
                foreach (var d in resultAbsent)
                {
                    if (d != null)
                    {
                        sb2.AppendLine("<br/>");
                        sb2.AppendLine(d.FullName);
                    }
                }
                CalanderData11.Description = sb2.ToString();
                calendarDataList.Add(CalanderData11);
            }
            return calendarDataList;
        }


    }
}
