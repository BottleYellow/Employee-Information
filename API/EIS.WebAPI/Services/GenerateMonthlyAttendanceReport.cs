using EIS.Data.Context;
using EIS.Entities.Employee;
using EIS.Entities.Enums;
using EIS.Entities.Models;
using EIS.Repositories.IRepository;
using EIS.WebAPI.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
            DateTime d = DateTime.Now;
            d = d.AddMonths(-1);
            int year = d.Year;
            int month = d.Month;
            //if (month != 1)
            //{
            //    month = month - 1;
            //}
            //else
            //{
            //    year = year - 1;
            //    month = 12;
            //}
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
                .Select(p => new
                {
                    p.Id,
                    FullName = p.FullName,
                    EmployeeCode = p.EmployeeCode,
                    EmailAddress = p.EmailAddress,
                    Attendances = p.Attendance.Where(a => a.DateIn.Year == year && a.DateIn.Month == month)
                }).ToList();
            foreach (var p in results)
            {
                string attendanceReportPath = @"C:\Temp\" + year + "\\" + monthName + "\\" + p.EmployeeCode + "AttendanceReport.txt";

                if (File.Exists(attendanceReportPath))
                {
                    File.Delete(attendanceReportPath);
                }

                using (StreamWriter sw = File.CreateText(attendanceReportPath))
                {
                    string InputOne = year.ToString();
                    char c = '0';
                    string InputTwo = month.ToString().PadLeft(2, c);

                    List<EmployeeAttendanceData> data = new List<EmployeeAttendanceData>();
                    var SP_SelectType = new SqlParameter("@SelectType", "Month");
                    var SP_PersonId = new SqlParameter("@PersonId", p.Id);
                    var SP_InputOne = new SqlParameter("@InputOne", InputOne);
                    var SP_InputTwo = new SqlParameter("@InputTwo", InputTwo);
                    string usp = "LMS.usp_GetEmployeewiseAttendanceData @PersonId, @SelectType, @InputOne, @InputTwo";
                    data = _dbContext._sp_GetEmployeeAttendanceData.FromSql(usp, SP_PersonId, SP_SelectType, SP_InputOne, SP_InputTwo).ToList();



                    sw.WriteLine("Employee Name:-" + p.FullName);
                    sw.WriteLine("Employee Code:-" + p.EmployeeCode);
                    sw.WriteLine("Monthly Attendance Report:-" + month + "/" + year);

                    StringBuilder newlist = new StringBuilder();
                    newlist.AppendLine("  DATE          STATUS     TIME IN      TIME OUT      TOTAL HOURS");
                    foreach (var attendance in data)
                    {
                        //DateTime.Today.Add(attendance.TimeOut.GetValueOrDefault()).ToString("hh:mm tt")
                        newlist.Append(Convert.ToDateTime(attendance.DateIn).ToString("dd/MM/yyyy").ToString() + "   ");
                        newlist.Append("   " + attendance.Status);
                        newlist.Append(attendance.TimeIn == null ? "" : "   " + DateTime.Today.Add(attendance.TimeIn.GetValueOrDefault()).ToString("hh:mm tt") + "   ");
                        newlist.Append(attendance.TimeOut == null ? "" : "   " + DateTime.Today.Add(attendance.TimeOut.GetValueOrDefault()).ToString("hh:mm tt") + "   ");
                        newlist.Append(attendance.TotalHours == null ? "" : "   " + attendance.TotalHours.ToString() + "   ");
                        newlist.AppendLine();
                    }
                    sw.WriteLine(newlist);
                    sw.WriteLine("File created: {0}", DateTime.Now.ToString());
                    sw.WriteLine("Total No of Days:-" + TotalDays);
                    sw.WriteLine("No of Days Present:-" + data.Where(x => x.Status == "Present").Count());
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