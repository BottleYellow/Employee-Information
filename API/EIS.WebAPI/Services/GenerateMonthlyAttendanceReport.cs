using EIS.Data.Context;
using EIS.Entities.Employee;
using EIS.Entities.Enums;
using EIS.Repositories.IRepository;
using EIS.WebAPI.Controllers;
using EIS.WebAPI.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EIS.WebAPI.Services
{
    public interface IGenerateMonthlyAttendanceReport
    {
        void EmailSentToAllEmployee();
    }
    public class GenerateMonthlyAttendanceReport : IGenerateMonthlyAttendanceReport
    {
        private readonly IRepositoryWrapper _repository;
        protected RedisAgent Cache;
        internal int TenantId = 0;
        protected ApplicationDbContext _dbContext;
        public readonly IConfiguration _configuration;
        private static object Lock = new object();

        public GenerateMonthlyAttendanceReport(IRepositoryWrapper repository,ApplicationDbContext dbContext, IConfiguration configuration) 
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _repository = repository;
            Cache = new RedisAgent();
            TenantId = Convert.ToInt32(Cache.GetStringValue("TenantId"));
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
            int TotalDays = DateTime.DaysInMonth(year, month);

            var employees = _repository.Attendances.GetAttendanceMonthly(month, year);
            foreach (Person p in employees)
            {
                
                string logPath = @"C:\Temp\"+p.EmployeeCode+"AttendanceReport.txt";

                if (File.Exists(logPath))
                {
                    File.Delete(logPath);
                }

                using (StreamWriter sw = File.CreateText(logPath))
                {
                    
                    sw.WriteLine("Employee Name:-"+p.FullName);
                    sw.WriteLine("Employee Code:-"+p.EmployeeCode);
                    sw.WriteLine("Monthly Attendance Report:-"+month+"/"+year);        
                    DateTime startDate = new DateTime(year, month, 1);
                    DateTime endDate = startDate.AddMonths(1);
                    StringBuilder newlist = new StringBuilder();
                    newlist.AppendLine("   DATE     STATUS   TIME IN   TIME OUT   TOTAL HOURS");
                    int counter = 0;
                    for (DateTime date = startDate; date < endDate; date = date.AddDays(1))
                    {                   
                        newlist.Append(date.ToShortDateString()+"   ");
                        var attendance = p.Attendance.Where(x => x.DateIn == date).Select(x => new { x.TimeIn, x.TimeOut, x.TotalHours }).FirstOrDefault();
                        if (attendance == null || attendance.TimeIn == attendance.TimeOut)
                        {
                            if (date < DateTime.Now.Date)
                            {
                                newlist.Append("Absent   ");
                                newlist.Append("   -      ");
                                newlist.Append("   -      ");
                                newlist.Append("   -      ");
                            }
                        }
                        else
                        {
                            newlist.Append("Present  ");
                            newlist.Append(attendance.TimeIn.ToString()+"   ");
                            newlist.Append(attendance.TimeOut.ToString()+"   ");                          
                            newlist.Append(attendance.TotalHours.ToString()+"   ");
                            counter++;

                        }
                        newlist.AppendLine();                      
                    }
                    sw.WriteLine(newlist);
                    sw.WriteLine("File created: {0}", DateTime.Now.ToString());
                    sw.WriteLine("Total No of Days:-" + TotalDays);
                    sw.WriteLine("No of Days Present:-"+counter);
                    sw.WriteLine("No of Days Absent:-"+(TotalDays-counter));
                    sw.WriteLine("For any assistance please contact accounts department");
                    sw.Flush();
                    sw.Close();
                }


                string To = p.EmailAddress;
                string subject = "Monthly Attendance Report";
                string body = "Hello " + GetTitle(p.Gender) + " " + p.FirstName + " " + p.LastName + "\n" +
                    "Monthly report is attached. : \n" +
                    "Your Code Number: " + p.EmployeeCode + "\n" +
                    "User Name: " + p.EmailAddress;

                new EmailManager(_configuration).SendEmailWithAttachment(subject, body, To, logPath);
            }
        }

        public string GetTitle(Gender gender)
        {
            string Title = "";
            if (gender == Gender.Male) Title = "Mr.";
            else if (gender == Gender.Female) Title = "Ms.";
            return Title;
        }
       
    }


    public class StringAttendance
    {
        public string Date { get; set; }
        public string Status { get; set; }
        public string DateIn { get; set; }
        public string Dateout { get; set; }
        public string TotalHours { get; set; }
    }
}


