using EIS.Data.Context;
using EIS.Entities.Dashboard;
using EIS.Entities.Employee;
using EIS.Entities.Hoildays;
using EIS.Entities.Leave;
using EIS.Entities.Models;
using EIS.Entities.SP;
using EIS.Entities.User;
using EIS.Repositories.Helpers;
using EIS.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
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

        public Admin_Dashboard GetAdminDashboard(string attendanceStatus, int location, int TenantId)
        {
            // List<LeaveRequest> leaveRequests = new List<LeaveRequest>();
            // List<Person> employees = new List<Person>();
            // int leaves = location == 0 ? _dbContext.LeaveRequests.Where(x => x.Status == "Pending" && x.TenantId == TenantId).Count() : _dbContext.LeaveRequests.Include(x=>x.Person).Where(x => x.Status == "Pending" && x.TenantId == TenantId &&x.Person.LocationId==location).Count();
            // int pcount = 0;

            // var results = _dbContext.Person.Include(x => x.Role).Where(x => x.Role.Name != "Admin").Include(y => y.Location)
            //.Select(p => new
            //{
            //    p,
            //    Attendances = p.Attendance.Where(a => a.DateIn.Date == DateTime.Now.Date)
            //});
            //usp_GetAdminDashboardDetails
            Admin_Dashboard Model = new Admin_Dashboard();
            Model.sP_AdminDashboardCount = new SP_AdminDashboardCount();
            Model.sP_AdminDashboards = new List<SP_AdminDashboard>();

            var param = new SqlParameter("@locationId", location);
            string usp = "LMS.usp_GetAdminDashboardDetails @locationId";
            Model.sP_AdminDashboards = _dbContext._sp_AdminDashboard.FromSql(usp, param).ToList();

            usp = "LMS.usp_GetAdminDashboardCountDetails @locationId";
            Model.sP_AdminDashboardCount = _dbContext._sp_AdminDashboardcount.FromSql(usp, param).FirstOrDefault();


            usp = "LMS.usp_GetAdminDashboardLeaveDetails @locationId";
            Model.sp_AdminDashboardLeaves = _dbContext._sp_AdminDashboardLeave.FromSql(usp, param).ToList();


            //foreach (var x in results)
            //{
            //    x.p.Attendance = x.Attendances.ToList();
            //}
            //var result = results.Select(x => x.p).ToList();
            int totalCount = 0;

            if (attendanceStatus == "Present")
            {
                Model.sP_AdminDashboards = Model.sP_AdminDashboards.Where(x => x.TimeIn != null).ToList();
            }
            else if (attendanceStatus == "Absent")
            {

                Model.sP_AdminDashboards = Model.sP_AdminDashboards.Where(x => x.TimeIn == null).ToList();
            }

            return Model;
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
        public Employee_Dashboard GetEmployeeDashboard(int TenantId, int PersonId)
        {
            //int currentMonth = DateTime.Now.Month;
            //int currentYear = DateTime.Now.Year;
            //int TotalDays = DateTime.DaysInMonth(currentYear, currentMonth);
            //var attendance = _dbContext.Attendances.Where(x => x.PersonId == PersonId && x.DateIn.Month == currentMonth && x.DateIn.Year == currentYear);
            //var leaves = _dbContext.LeaveRequests.Where(x => x.PersonId == PersonId && x.TenantId == TenantId && x.Status == "Approved").Sum(x => x.RequestedDays);
            //var availableLeaves = _dbContext.LeaveCredit.Where(x => x.PersonId == PersonId).Sum(x => x.Available);

            //EmployeeDashboard dashboard = new EmployeeDashboard
            //{
            //    MonthlyPresentDays = attendance.Count(),
            //    MonthlyAbsentDays = TotalDays - attendance.Count(),
            //    TotalLeavesAvailable = Convert.ToInt32(availableLeaves),
            //    TotalLeavesTaken = Convert.ToInt32(leaves)
            //};
            //return dashboard;
            Employee_Dashboard Model = new Employee_Dashboard();
            Model.SP_EmployeeDashboardCount = new SP_EmployeeDashboardCount();
            Model.SP_EmployeeDashboards = new List<SP_EmployeeDashboard>();

            var param = new SqlParameter("@PersonId", PersonId);
            string usp = "LMS.usp_GetEmployeeDashboardDetails @PersonId";
            Model.SP_EmployeeDashboards = _dbContext._sp_EmployeeDashboard.FromSql(usp, param).ToList();

            usp = "LMS.usp_GetEmployeeDashboardCountDetails @PersonId";
            Model.SP_EmployeeDashboardCount = _dbContext._sp_EmployeeDashboardcount.FromSql(usp, param).FirstOrDefault();

            return Model;
        }


        public List<CalendarData> GetCalendarDetails(int location, DateTime beginDate, DateTime stopDate)
        {
            List<CalendarData> calendarDataList = new List<CalendarData>();
            IEnumerable<Holiday> holidays = new List<Holiday>();
            IEnumerable<LeaveRequest> leaveList = new List<LeaveRequest>();

            if (location == 0)
            {
                holidays = _dbContext.Holidays.Include(x => x.Location).Where(x => x.Location.IsActive == true).ToList();
                leaveList = _dbContext.LeaveRequests.Include(x => x.Person.Location).Where(x => x.Person.Location.IsActive == true).ToList();
            }
            else
            {
                holidays = _dbContext.Holidays.Include(x => x.Location).Where(x => x.LocationId == location && x.Location.IsActive == true).ToList();
                leaveList = _dbContext.LeaveRequests.Include(x => x.Person).Include(x => x.Person.Location).Where(x => x.Person.Location.Id == location && x.Person.Location.IsActive == true).ToList();
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


                var results = _dbContext.Person.Include(x => x.Location).Include(x => x.Role).Where(x => x.Role.Name != "Admin" && x.Location.IsActive == true)
                           .Select(p => new
                           {
                               p,
                               Attendances = p.Attendance.Where(a => a.DateIn == date)
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
                if (presentCount > 0)
                {
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
            }
            return calendarDataList;
        }


    }
}
