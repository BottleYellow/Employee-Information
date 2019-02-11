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

        public AdminDashboard GetAdminDashboard(string attendanceStatus, int location,int TenantId)
        {
            List<LeaveRequest> leaveRequests = new List<LeaveRequest>();
            List<Person> employees = new List<Person>();
            int leaves = location == 0 ? _dbContext.LeaveRequests.Include(x=>x.Person.Location).Where(x => x.Status == "Pending" && x.TenantId == TenantId && x.Person.Location.IsActive==true).Count() : _dbContext.LeaveRequests.Include(x=>x.Person).Include(x=>x.Person.Location).Where(x => x.Status == "Pending" && x.TenantId == TenantId && x.Person.Location.IsActive==true && x.Person.LocationId==location).Count();
            int pcount = 0;
            
            var results = _dbContext.Person.Include(x => x.Location).Include(x => x.Role).Where(x => x.Role.Name != "Admin" && x.Location.IsActive==true)
           .Select(p => new
           {
               p,
               Attendances = p.Attendance.Where(a => a.DateIn.Date == DateTime.Now.Date)
           });
           
            foreach (var x in results)
            {
                x.p.Attendance = x.Attendances.ToList();
            }
            var result = results.Select(x => x.p).ToList();
            int totalCount=0;

            if (attendanceStatus== "Present")
            {
                result = location == 0 ? result: result.Where(x =>x.LocationId == location).ToList();
                totalCount = result.Count();
                result = result.Where(x =>x.Attendance!=null && x.Attendance.Count() > 0).ToList();
                pcount = result.Count();
            }else
            if(attendanceStatus == "Absent")
            {
                result = location == 0 ? result : result.Where(x => x.LocationId == location).ToList();
                totalCount = result.Count();
                result = result.Where(x => x.Attendance.Count() == 0).ToList();
                pcount = totalCount- result.Count();
            }
            else
            {
                result = location == 0 ? result : result.Where(x => x.LocationId == location).ToList();
                totalCount = result.Count();
                pcount = result.Where(x => x.Attendance != null && x.Attendance.Count() > 0).Count();
                leaveRequests =location == 0 ? _dbContext.LeaveRequests.Include(x=>x.Person.Location).Where(x => x.Status == "Pending" && x.TenantId == TenantId && x.Person.Location.IsActive==true).ToList() : _dbContext.LeaveRequests.Include(x => x.Person).Include(x=>x.Person.Location).Where(x => x.Status == "Pending" && x.TenantId == TenantId && x.Person.Location.IsActive==true && x.Person.LocationId == location).ToList();
                leaves = leaveRequests.Count();
            }        
            
            AdminDashboard dashboard = new AdminDashboard
            {
                AllEmployeesCount = totalCount,
                PresentEmployees = pcount,
                AbsentEmployees = totalCount - pcount,
                PendingLeavesCount = leaves,
                Employees = result,
                LeaveRequests= leaveRequests
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
            IEnumerable<Attendance> data = new List<Attendance>();
            if (location==0)
            {
                holidays = _dbContext.Holidays.Include(x => x.Location).Where(x => x.Location.IsActive == true).ToList();
                leaveList = _dbContext.LeaveRequests.Include(x=>x.Person.Location).Where(x=>x.Person.Location.IsActive==true).ToList();
                data = _dbContext.Attendances.Include(x => x.Person).Include(x => x.Person.Location).Where(x => x.Person.Location.IsActive == true).ToList();
            }
            else
            {
                holidays = _dbContext.Holidays.Include(x => x.Location).Where(x => x.LocationId == location && x.Location.IsActive == true).ToList();
                leaveList = _dbContext.LeaveRequests.Include(x => x.Person).Include(x => x.Person.Location).Where(x => x.Person.Location.Id == location && x.Person.Location.IsActive == true).ToList();
                data = _dbContext.Attendances.Include(x => x.Person).Include(x => x.Person.Location).Where(x => x.Person.Location.Id == location && x.Person.Location.IsActive == true).ToList();
            }

            int count = 0;
            for (DateTime date = beginDate; date < stopDate; date = date.AddDays(1))
            {
                IEnumerable<Attendance> attendances = data.Where(x => x.DateIn == date);
                if (attendances.Count() != 0)
                {
                    CalendarData calendarData1 = new CalendarData();
                    calendarData1.Title = "Present Count:" + attendances.Count();
                    calendarData1.StartDate = date;
                    calendarData1.EndDate = date;
                    calendarData1.Color = "Green";
                    calendarData1.IsFullDay = true;
                    StringBuilder sb = new StringBuilder();
                    foreach (var d in attendances)
                    {
                        if (d != null)
                        {
                            sb.AppendLine("<br/>");
                            sb.AppendLine(d.Person.FullName + " (" + d.TimeIn.ToString(@"hh\:mm") + "-" + d.TimeOut.GetValueOrDefault(new TimeSpan()).ToString(@"hh\:mm") + ") Working Hours-" + d.TotalHours.GetValueOrDefault(new TimeSpan()).ToString(@"hh\:mm"));
                        }
                    }
                    calendarData1.Description = sb.ToString();
                    calendarDataList.Add(calendarData1);
                }

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

            }
            return calendarDataList;
        }


    }
}
