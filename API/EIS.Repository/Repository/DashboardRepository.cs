﻿using EIS.Data.Context;
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
using System.Globalization;
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

        public Admin_Dashboard GetAdminDashboard(string attendanceStatus, int location, int TenantId,string date)
        {
            DateTime date1 = Convert.ToDateTime(date);
            string date2 = date1.ToString("yyyy-MM-dd");
            Admin_Dashboard Model = new Admin_Dashboard();
            Model.sP_AdminDashboardCount = new SP_AdminDashboardCount();
            Model.sP_AdminDashboards = new List<SP_AdminDashboard>();            
            var param = new SqlParameter("@locationId", location);
            var param2 = new SqlParameter("@date", date2);
            string usp = "LMS.usp_GetAdminDashboardDetails @locationId, @date";
            Model.sP_AdminDashboards = _dbContext._sp_AdminDashboard.FromSql(usp, param,param2).ToList();

            usp = "LMS.usp_GetAdminDashboardCountDetails @locationId,@date";
            Model.sP_AdminDashboardCount = _dbContext._sp_AdminDashboardcount.FromSql(usp, param,param2).FirstOrDefault();


            usp = "LMS.usp_GetAdminDashboardLeaveDetails @locationId";
            Model.sp_AdminDashboardLeaves = _dbContext._sp_AdminDashboardLeave.FromSql(usp, param).ToList();


            //foreach (var x in results)
            //{
            //    x.p.Attendance = x.Attendances.ToList();
            //}
            //var result = results.Select(x => x.p).ToList();
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
            Employee_Dashboard Model = new Employee_Dashboard();
            Model.SP_EmployeeDashboardCount = new SP_EmployeeDashboardCount();
            Model.SP_EmployeeDashboards = new List<SP_EmployeeDashboard>();

            var param = new SqlParameter("@PersonId", PersonId);
            string usp = "LMS.usp_GetEmployeeDashboardDetails @PersonId";
            Model.SP_EmployeeDashboards = _dbContext._sp_EmployeeDashboard.FromSql(usp, param).ToList();

            usp = "LMS.usp_GetEmployeeDashboardCountDetails @PersonId";
            Model.SP_EmployeeDashboardCount = _dbContext._sp_EmployeeDashboardcount.FromSql(usp, param).FirstOrDefault();

            usp = "LMS.usp_GetEmployeeLeavesData @PersonId";
            Model.SP_EmployeeLeaveRequest = _dbContext._sp_EmployeeLeaveRequest.FromSql(usp, param).ToList();
            return Model;
        }


        public List<CalendarData> GetAdminCalendarDetails(int location, DateTime beginDate, DateTime stopDate)
        {
            List<CalendarData> calendarDataList = new List<CalendarData>();
            IEnumerable<Holiday> holidays = new List<Holiday>();
            IEnumerable<LeaveRequest> leaveList = new List<LeaveRequest>();
            string LocationName = "";
            if (location == 0)
            {
                holidays = _dbContext.Holidays.Include(x => x.Location).Where(x => x.Location.IsActive == true &&x.IsActive==true).ToList();
                leaveList = _dbContext.LeaveRequests.Include(x => x.Person.Location).Include(x => x.Person.WeeklyOff).Where(x => x.Person.Location.IsActive == true).ToList();
            }
            else
            {
                holidays = _dbContext.Holidays.Include(x => x.Location).Where(x => x.LocationId == location && x.Location.IsActive == true && x.IsActive==true).ToList();
                LocationName = _dbContext.Locations.Where(x => x.Id == location).FirstOrDefault().LocationName.ToUpper();
                leaveList = _dbContext.LeaveRequests.Include(x => x.Person).Include(x => x.Person.Location).Include(x=>x.Person.WeeklyOff).Where(x => x.Person.Location.Id == location && x.Person.Location.IsActive == true).ToList();
            }
            for (DateTime date = beginDate; date < stopDate; date = date.AddDays(1))
            {
                Holiday holiday = holidays.Where(x => x.Date == date).FirstOrDefault();
                if (holiday != null)
                {
                    CalendarData holidayCalanderData = new CalendarData();
                    holidayCalanderData.Title = holiday.Vacation;
                    holidayCalanderData.Description = "Holiday on occasion of " + holiday.Vacation;
                    holidayCalanderData.StartDate = holiday.Date;
                    holidayCalanderData.EndDate = holiday.Date;
                    holidayCalanderData.Color = "darkviolet";
                    holidayCalanderData.IsFullDay = true;

                    calendarDataList.Add(holidayCalanderData);
                }
                if (date.DayOfWeek == DayOfWeek.Sunday)
                {
                    CalendarData holidayCalanderData = new CalendarData();
                    holidayCalanderData.Title = "Weekly Off";
                    holidayCalanderData.Description = "Weekly Off";
                    holidayCalanderData.StartDate = date;
                    holidayCalanderData.EndDate = date;
                    holidayCalanderData.Color = "royalblue";
                    holidayCalanderData.IsFullDay = true;
                    calendarDataList.Add(holidayCalanderData);

                }
                if (date.DayOfWeek == DayOfWeek.Saturday && (LocationName == "BANER" || LocationName == ""))
                {
                    string alternateDateStatus = CalculateDate(date);
                    if (!string.IsNullOrEmpty(alternateDateStatus))
                    {
                        CalendarData holidayCalanderData = new CalendarData();
                        holidayCalanderData.Title = alternateDateStatus;
                        holidayCalanderData.Description = "Weekly Off";
                        holidayCalanderData.StartDate = date;
                        holidayCalanderData.EndDate = date;
                        holidayCalanderData.Color = "royalblue";
                        holidayCalanderData.IsFullDay = true;
                        calendarDataList.Add(holidayCalanderData);
                    }
                }

                List<LeaveRequest> leaveRequest = leaveList.Where(x => x.FromDate <= date && x.ToDate >= date).ToList();
                if (leaveRequest.Count > 0)
                {
                    foreach (var leave in leaveRequest)
                    {
                        if (date.DayOfWeek != DayOfWeek.Sunday && holiday==null)
                        {
                            if (date.DayOfWeek == DayOfWeek.Saturday && leave.Person.WeeklyOff.Type == "AlternateSaturday")
                            {
                                string alternateDateStatus = CalculateDate(date);
                                if (string.IsNullOrEmpty(alternateDateStatus))
                                {
                                    CalendarData calendarData = new CalendarData();
                                    calendarData.Title = leave.EmployeeName + " (Leave " + leave.Status + ")";
                                    calendarData.Description = "<br/>Leave Status : " + leave.Status + "<br/>Leave Reason :" + leave.Reason;
                                    calendarData.StartDate = date;
                                    calendarData.EndDate = date;
                                    if (leave.Status == "Pending")
                                    {
                                        calendarData.Color = "Violet";
                                    }
                                    else if (leave.Status.Contains("Approved"))
                                    {
                                        calendarData.Color = "dodgerblue";
                                    }
                                    else if (leave.Status == "Rejected")
                                    {
                                        calendarData.Color = "crimson";
                                    }
                                    else
                                    {
                                        calendarData.Color = "palevioletred";
                                    }
                                    calendarData.IsFullDay = true;
                                    calendarDataList.Add(calendarData);
                                }
                            }
                            else
                            {
                                CalendarData calendarData = new CalendarData();
                                calendarData.Title = leave.EmployeeName + " (Leave " + leave.Status + ")";
                                calendarData.Description = "<br/>Leave Status : " + leave.Status + "<br/>Leave Reason :" + leave.Reason;
                                calendarData.StartDate = date;
                                calendarData.EndDate = date;
                                if (leave.Status == "Pending")
                                {
                                    calendarData.Color = "Violet";
                                }
                                else if (leave.Status.Contains("Approved"))
                                {
                                    calendarData.Color = "dodgerblue";
                                }
                                else if (leave.Status == "Rejected")
                                {
                                    calendarData.Color = "crimson";
                                }
                                else
                                {
                                    calendarData.Color = "palevioletred";
                                }
                                calendarData.IsFullDay = true;
                                calendarDataList.Add(calendarData);
                            }

                        }

                    }
                }

                 var results = _dbContext.Person.Include(x => x.Location).Include(x => x.Role).Where(x => x.Role.Name != "Admin" && x.IsActive == true && x.Location.IsActive == true)
                           .Select(p => new
                           {
                               p,
                               Attendances = p.Attendance.Where(a => a.DateIn == date && a.IsActive==true)
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
                    CalendarData presentCalanderData = new CalendarData();
                    presentCalanderData.Title = "Present Count : " + presentCount;
                    presentCalanderData.StartDate = date;
                    presentCalanderData.EndDate = date;
                    presentCalanderData.Color = "Green";
                    presentCalanderData.IsFullDay = true;
                    StringBuilder sb1 = new StringBuilder();
                    int presentNumber = 1;
                    sb1.AppendLine("<br/>");
                    sb1.AppendLine("<table class='table' style='height:200px; overflow-y:auto; display:block;'>");
                    sb1.AppendLine("<thead>");
                    sb1.AppendLine("<tr>");
                    sb1.AppendLine("<th>S.No</th><th>NAME</th><th>ClockIn</th><th>ClockOut</th>");
                    sb1.AppendLine("</tr>");
                    sb1.AppendLine("</thead>");
                    sb1.AppendLine("<tbody>");
                    foreach (var d in resultPresent)
                    {
                        sb1.AppendLine("<tr>");
                        sb1.AppendLine("<td>" + presentNumber + ")</td>");
                        sb1.AppendLine("<td>" + d.FullName + "</td>");
                        if (d != null)
                        {
                            DateTime timeIn = DateTime.ParseExact(d.Attendance.FirstOrDefault().TimeIn.ToString(), "HH:mm:ss", CultureInfo.InvariantCulture);
                            sb1.AppendLine("<td>" + timeIn.ToString("hh:mm tt") + "</td>");
                            if (d.Attendance.FirstOrDefault().TimeOut != null)
                            {
                                DateTime timeOut = DateTime.ParseExact(d.Attendance.FirstOrDefault().TimeOut.ToString(), "HH:mm:ss", CultureInfo.InvariantCulture);
                                sb1.AppendLine("<td>" + timeOut.ToString("hh:mm tt") + "</td>");
                            }
                            else
                            {
                                sb1.AppendLine("<td>-</td>");
                            }
                            presentNumber++;
                        }
                        sb1.AppendLine("<tr>");
                    }
                    sb1.AppendLine("</tbody>");
                    sb1.AppendLine("</table>");
                    presentCalanderData.Description = sb1.ToString();
                    calendarDataList.Add(presentCalanderData);

                    var resultAbsent = location == 0 ? result.Where(x => x.Attendance.Count() == 0).ToList() : result.Where(x => x.LocationId == location && x.Attendance.Count() == 0).ToList();
                    absentCount = resultAbsent.Count();

                    CalendarData absentCalanderData = new CalendarData();
                    absentCalanderData.Title = "OnLeave Count : " + absentCount;
                    absentCalanderData.StartDate = date;
                    absentCalanderData.EndDate = date;
                    absentCalanderData.Color = "orangered";
                    absentCalanderData.IsFullDay = true;
                    StringBuilder sb2 = new StringBuilder();
                    int absentNumber = 1;
                    sb2.AppendLine("<br/>");
                    sb2.AppendLine("<table class='table' style='height:200px; overflow-y:auto; display:block;'>");
                    sb2.AppendLine("<thead>");
                    sb2.AppendLine("<tr>");
                    sb2.AppendLine("<th>S.No</th><th>NAME</th>");
                    sb2.AppendLine("</tr>");
                    sb2.AppendLine("</thead>");
                    sb2.AppendLine("<tbody>");
                    foreach (var d in resultAbsent)
                    {
                        if (d != null)
                        {
                            sb2.AppendLine("<td>" + absentNumber + ")</td>");
                            sb2.AppendLine("<td>" + d.FullName + "</td>");
                            absentNumber++;
                        }
                        sb2.AppendLine("<tr>");
                    }

                    sb2.AppendLine("</tbody>");
                    sb2.AppendLine("</table>");
                    absentCalanderData.Description = sb2.ToString();
                    calendarDataList.Add(absentCalanderData);
                }
            }
            return calendarDataList;
        }

        public List<CalendarData> GetEmployeeCalendarDetails(int personId, DateTime beginDate, DateTime stopDate)
        {
            List<CalendarData> calendarDataList = new List<CalendarData>();
            IEnumerable<Holiday> holidays = new List<Holiday>();
            IEnumerable<LeaveRequest> leaveList = new List<LeaveRequest>();
            IEnumerable<Attendance> attendances = new List<Attendance>();
            Person person = _dbContext.Person.Include(x => x.Location).Include(x => x.WeeklyOff).Where(x => x.Id == personId).FirstOrDefault();
            int? locationId = person.LocationId;

            attendances = _dbContext.Attendances.Where(x => x.PersonId == personId);
            if (locationId != null)
            {
                holidays = _dbContext.Holidays.Where(x => x.LocationId == locationId && x.IsActive == true);
            }

            leaveList = _dbContext.LeaveRequests.Where(x => x.PersonId == personId);

            for (DateTime date = beginDate; date < stopDate; date = date.AddDays(1))
            {

                Attendance attendance = attendances.Where(x => x.DateIn == date && x.IsActive==true).FirstOrDefault();
                Holiday holiday = holidays.Where(x => x.Date == date).FirstOrDefault();
                if (attendance != null)
                {
                    CalendarData attendanceCalendarData = new CalendarData();
                    string timeout = attendance.TimeOut == null ? "Nil" : DateTime.Today.Add(attendance.TimeOut.GetValueOrDefault()).ToString("hh:mm tt");
                    string totalHours = attendance.TotalHours == null ? "Nil" : attendance.TotalHours.ToString();
                    attendanceCalendarData.Title = "Present (" + DateTime.Today.Add(attendance.TimeIn).ToString("hh:mm tt") + " - " + timeout + ")";
                    attendanceCalendarData.Description = "Total Working Hours : " + totalHours;
                    attendanceCalendarData.Color = "Green";
                    attendanceCalendarData.StartDate = date;
                    attendanceCalendarData.EndDate = date;
                    attendanceCalendarData.IsFullDay = true;
                    calendarDataList.Add(attendanceCalendarData);
                }

                if (holiday != null)
                {
                    CalendarData holidayCalanderData = new CalendarData();
                    holidayCalanderData.Title = holiday.Vacation;
                    holidayCalanderData.Description = "Holiday on occasion of " + holiday.Vacation;
                    holidayCalanderData.StartDate = holiday.Date;
                    holidayCalanderData.EndDate = holiday.Date;
                    holidayCalanderData.Color = "darkviolet";
                    holidayCalanderData.IsFullDay = true;
                    calendarDataList.Add(holidayCalanderData);
                }
                if (date.DayOfWeek == DayOfWeek.Sunday)
                {
                    CalendarData holidayCalanderData = new CalendarData();
                    holidayCalanderData.Title = "Weekly Off";
                    holidayCalanderData.Description = "Weekly Off";
                    holidayCalanderData.StartDate = date;
                    holidayCalanderData.EndDate = date;
                    holidayCalanderData.Color = "royalblue";
                    holidayCalanderData.IsFullDay = true;
                    calendarDataList.Add(holidayCalanderData);

                }

                if (person.WeeklyOff.Type == "AlternateSaturday" && date.DayOfWeek == DayOfWeek.Saturday)
                {
                    string alternateDateStatus = CalculateDate(date);
                    if (!string.IsNullOrEmpty(alternateDateStatus))
                    {
                        CalendarData holidayCalanderData = new CalendarData();
                        holidayCalanderData.Title = alternateDateStatus;
                        holidayCalanderData.Description = "Weekly Off";
                        holidayCalanderData.StartDate = date;
                        holidayCalanderData.EndDate = date;
                        holidayCalanderData.Color = "royalblue";
                        holidayCalanderData.IsFullDay = true;
                        calendarDataList.Add(holidayCalanderData);
                    }
                }
                LeaveRequest leaveRequest = leaveList.Where(x => x.FromDate <= date && x.ToDate >= date).FirstOrDefault();
                if (attendance != null || holiday != null || date.DayOfWeek == DayOfWeek.Sunday || (person.WeeklyOff.Type == "AlternateSaturday" && date.DayOfWeek == DayOfWeek.Saturday))
                {
                }
                else
                {
                    if (leaveRequest != null)
                    {
                        CalendarData calendarData = new CalendarData();
                        calendarData.Title = "Leave Status (" + leaveRequest.Status + ")";
                        calendarData.Description = "<br/>Leave Status : " + leaveRequest.Status + "<br/>Leave Reason :" + leaveRequest.Reason;
                        calendarData.StartDate = date;
                        calendarData.EndDate = date;
                        if (leaveRequest.Status == "Pending")
                        {
                            calendarData.Color = "Violet";
                        }
                        else if (leaveRequest.Status.Contains("Approved"))
                        {
                            calendarData.Color = "dodgerblue";
                        }
                        else if (leaveRequest.Status == "Rejected")
                        {
                            calendarData.Color = "crimson";
                        }
                        else
                        {
                            calendarData.Color = "palevioletred";
                        }
                        calendarData.IsFullDay = true;
                        calendarDataList.Add(calendarData);
                    }
                    else
                    {
                        if (date.Date <= DateTime.Now.Date)
                        {
                            CalendarData absentCalanderData = new CalendarData();
                            absentCalanderData.Title = "Absent";
                            absentCalanderData.Description = "Absent";
                            absentCalanderData.StartDate = date;
                            absentCalanderData.EndDate = date;
                            absentCalanderData.Color = "orangered";
                            absentCalanderData.IsFullDay = true;
                            calendarDataList.Add(absentCalanderData);
                        }

                    }
                }
            }
            return calendarDataList;
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
