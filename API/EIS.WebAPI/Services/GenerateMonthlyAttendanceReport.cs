using EIS.Data.Context;
using EIS.Entities.Employee;
using EIS.Entities.Enums;
using EIS.Entities.Leave;
using EIS.Repositories.IRepository;
using EIS.WebAPI.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace EIS.WebAPI.Services
{
    public interface IGenerateMonthlyAttendanceReport
    {
        void EmailSentToAllEmployee();
    }
    public class GenerateMonthlyAttendanceReport : IGenerateMonthlyAttendanceReport
    {
        private readonly IRepositoryWrapper _repository;

        internal int TenantId = 0;
        protected ApplicationDbContext _dbContext;
        public readonly IConfiguration _configuration;
        private static object Lock = new object();

        public GenerateMonthlyAttendanceReport(IRepositoryWrapper repository, ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _repository = repository;
        }

        public void EmailSentToAllEmployee()
        {

            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            if (month != 1)
            {
                month = month - 1;
            }
            else
            {
                year = year - 1;
                month = 12;
            }
            string monthName = new DateTime(2000, month, 1).ToString("MMM", CultureInfo.InvariantCulture);
            int TotalDays = DateTime.DaysInMonth(year, month);

            var rootPath = @"C:\Temp\";
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }
            if (!Directory.Exists(rootPath + year + "\\"))
            {
                Directory.CreateDirectory(rootPath + year + "\\");
            }
            if (!Directory.Exists(rootPath + year + "\\" + monthName + "\\"))
            {
                Directory.CreateDirectory(rootPath + year + "\\" + monthName + "\\");
            }

            var results = _dbContext.Person.Include(x => x.Location).Include(x => x.Role).Where(x => x.Role.Name != "Admin" && x.Location.IsActive == true).Include(x => x.LeaveRequests)
                .Select(person => new
                {   Id= person.Id,
                    FullName = person.FullName,
                    LocationId= person.LocationId,
                    LocationName= person.Location.LocationName,
                    EmployeeCode = person.EmployeeCode,
                    EmailAddress = person.EmailAddress,
                    Attendances = person.Attendance.Where(a => a.DateIn.Year == year && a.DateIn.Month == month)
                }).ToList();
            foreach (var p in results)
            {
                if (p.EmailAddress== "ud.soni2009@gmail.com")
                {
                    string attendanceReportPath = @"C:\Temp\" + year + "\\" + monthName + "\\" + p.EmployeeCode + "AttendanceReport.txt";

                    if (File.Exists(attendanceReportPath))
                    {
                        File.Delete(attendanceReportPath);
                    }

                    using (StreamWriter sw = File.CreateText(attendanceReportPath))
                    {
                        sw.WriteLine("Employee Name:-" + p.FullName);
                        sw.WriteLine("Employee Code:-" + p.EmployeeCode);
                        sw.WriteLine("Monthly Attendance Report:-" + month + "/" + year);
                        DateTime startDate = new DateTime(year, month, 1);
                        DateTime endDate = startDate.AddMonths(1);
                        StringBuilder newlist = new StringBuilder();
                        newlist.AppendLine("   DATE        STATUS      TIME IN      TIME OUT      TOTAL HOURS");
                        int counter = 0;
                        for (DateTime date = startDate; date < endDate; date = date.AddDays(1))
                        {
                            newlist.Append(date.ToShortDateString() + "   ");
                            var attendance = p.Attendances.Where(x => x.DateIn == date).Select(x => new { x.TimeIn, x.TimeOut, x.TotalHours }).FirstOrDefault();
                            if (attendance == null || attendance.TimeIn == attendance.TimeOut)
                            {
                                var holiday = _dbContext.Holidays.Where(x => x.LocationId == p.LocationId && x.Date == date).FirstOrDefault();
                                if (holiday != null)
                                {
                                    newlist.Append("  " + holiday.Vacation);
                                }
                                else
                                {
                                    if (date.DayOfWeek == DayOfWeek.Sunday)
                                    {
                                        newlist.Append("  WeeklyOff");
                                    }
                                    else if (p.LocationName.ToUpper() == "BANER")
                                    {
                                        string alternateSaturday = _repository.Attendances.CalculateDate(date);
                                        if (!string.IsNullOrEmpty(alternateSaturday))
                                        {
                                            newlist.Append("  " + alternateSaturday);
                                        }
                                        else
                                        {
                                            LeaveRequest leaveRequest = _repository.LeaveRequest.FindAllByCondition(x => x.FromDate <= date && x.ToDate >= date && x.PersonId==p.Id).Where(x => x.Status == "Pending" || x.Status == "Approved").FirstOrDefault();
                                            if (leaveRequest != null)
                                            {
                                                newlist.Append("  OnLeave  ");
                                            }
                                            else
                                            {
                                                newlist.Append("  Absent  ");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        LeaveRequest leaveRequest = _repository.LeaveRequest.FindAllByCondition(x => x.FromDate <= date && x.ToDate >= date && x.PersonId==p.Id).Where(x=>x.Status=="Pending"||x.Status=="Approved").FirstOrDefault();
                                        if(leaveRequest!=null)
                                        {
                                            newlist.Append("  OnLeave  ");
                                        }
                                        else
                                        {
                                            newlist.Append("  Absent  ");
                                        }
                                       
                                    }
                                }
                                newlist.Append("             ");
                                newlist.Append("            ");
                                newlist.Append("             ");
                            }
                            else
                            {
                                newlist.Append("  Present  ");
                                string timeout = attendance.TimeOut == null ? "Nil" : DateTime.Today.Add(attendance.TimeOut.GetValueOrDefault()).ToString("hh:mm tt");
                                string totalHours = attendance.TotalHours == null ? "Nil" : attendance.TotalHours.ToString();
                                newlist.Append("   " + DateTime.Today.Add(attendance.TimeIn).ToString("hh:mm tt") + "   ");
                                newlist.Append("   " + timeout + "   ");
                                newlist.Append("   " + totalHours + "   ");
                                counter++;
                            }
                            newlist.AppendLine();
                        }
                        sw.WriteLine(newlist);
                        sw.WriteLine("Total No of Days:-" + TotalDays);
                        sw.WriteLine("No of Days Present:-" + counter);
                        sw.WriteLine("For any assistance please contact HR department");
                        sw.Flush();
                        sw.Close();
                    }
                    string To = p.EmailAddress;
                    string subject = "Monthly Attendance Report";
                    string body = "Dear " + p.FullName + "\n" +
                        "Kindly find monthly attendance report in attachment.\n" +
                        "Your Code Number: " + p.EmployeeCode + "\n" +
                        "User Name: " + p.EmailAddress;
                    new EmailManager(_configuration, _repository).SendEmail(subject, body, To, attendanceReportPath);
                }
            }
        }     

    }
}