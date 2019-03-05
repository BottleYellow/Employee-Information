using EIS.Data.Context;
using EIS.Entities.Employee;
using EIS.Entities.Enums;
using EIS.Entities.Models;
using EIS.Repositories.IRepository;
using EIS.WebAPI.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
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

            var results = _dbContext.Person.Include(x => x.Location).Include(x => x.Role).Where(x => x.Role.Name != "Admin" && x.Location.IsActive == true)
                .Select(p => new
                {
                    Id=p.Id,
                    FullName = p.FullName,
                    EmployeeCode = p.EmployeeCode,
                    EmailAddress = p.EmailAddress
                }).ToList();


            foreach (var p in results)
            {
                string attendanceReportPath = @"C:\Temp\" + year + "\\" + monthName + "\\" + p.EmployeeCode + "AttendanceReport.xlsx";

                if (File.Exists(attendanceReportPath))
                {
                    File.Delete(attendanceReportPath);
                }
                string InputOne = year.ToString();
                char c = '0';
                string InputTwo = month.ToString().PadLeft(2, c);
                List<EmployeeAttendanceData> data1 = new List<EmployeeAttendanceData>();
                List<EmployeeAttendanceData> data = Data("Month", p.Id, InputOne, InputTwo,data1);
                var memory = new MemoryStream();
                using (var sw = new FileStream(attendanceReportPath, FileMode.Create, FileAccess.Write))
                {
                    int i = 1;

                    IWorkbook workbook;
                    workbook = new XSSFWorkbook();
                    ISheet sheet = workbook.CreateSheet("demo");
                    IRow row = sheet.CreateRow(0);
                    row = sheet.CreateRow(i++);
                    row.CreateCell(0).SetCellValue("Employee Name:-");
                    row.CreateCell(1).SetCellValue(p.FullName);
                    row = sheet.CreateRow(i++);
                    row.CreateCell(0).SetCellValue("Employee Code:-");
                    row.CreateCell(1).SetCellValue(p.EmployeeCode);
                    row = sheet.CreateRow(i++);
                    row.CreateCell(0).SetCellValue("Monthly Attendance Report:-");
                    row.CreateCell(1).SetCellValue(month + "/" + year);
                    row = sheet.CreateRow(i++);
                    row = sheet.CreateRow(i++);
                    row.CreateCell(0).SetCellValue("DATE");
                    row.CreateCell(1).SetCellValue("STATUS");
                    row.CreateCell(2).SetCellValue("TIME IN");
                    row.CreateCell(3).SetCellValue("TIME OUT");
                    row.CreateCell(4).SetCellValue("TOTAL HOURS");
                    foreach(var attendance in data)
                    {
                        row = sheet.CreateRow(i);
                        DateTime.Today.Add(attendance.TimeOut.GetValueOrDefault()).ToString("hh:mm tt");
                        row.CreateCell(0).SetCellValue(Convert.ToDateTime(attendance.DateIn).ToString("dd/MM/yyyy").ToString());
                        row.CreateCell(1).SetCellValue(attendance.Status);
                        row.CreateCell(2).SetCellValue(attendance.TimeIn == null ? "" : DateTime.Today.Add(attendance.TimeIn.GetValueOrDefault()).ToString("hh:mm tt"));
                        row.CreateCell(3).SetCellValue(attendance.TimeOut == null ? "" : DateTime.Today.Add(attendance.TimeOut.GetValueOrDefault()).ToString("hh:mm tt"));
                        row.CreateCell(4).SetCellValue(attendance.TotalHours == null ? "" : attendance.TotalHours.ToString());
                        i++;
                    }                    
                    row = sheet.CreateRow(i++);
                    row = sheet.CreateRow(i++);
                    row.CreateCell(0).SetCellValue("Total No of Days: -");
                    row.CreateCell(1).SetCellValue(TotalDays);
                    row = sheet.CreateRow(i++);
                    row.CreateCell(0).SetCellValue("No of Days Present:-");
                    row.CreateCell(1).SetCellValue(data.Where(x => x.Status == "Present").Count());
                    workbook.Write(sw);
                }
                data = null;
                using (var stream = new FileStream(attendanceReportPath, FileMode.Open))
                {
                    stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                string To = p.EmailAddress;
                string subject = "Monthly Attendance Report";
                string body = "Dear " + p.FullName + "\n" +
                    "Kindly find monthly attendance report in attachment.\n" +
                    "Your Code Number: " + p.EmployeeCode + "\n" +
                    "User Name: " + p.EmailAddress;
                new EmailManager(_configuration, _repository).SendEmail(subject, body, To, attendanceReportPath);
            }
        }


        public List<EmployeeAttendanceData> Data( string Type,int PersonId,string InputOne,string InputTwo, List<EmployeeAttendanceData> data)
        {
            var SP_SelectType = new SqlParameter("@SelectType", "Month");
            var SP_PersonId = new SqlParameter("@PersonId", PersonId);
            var SP_InputOne = new SqlParameter("@InputOne", InputOne);
            var SP_InputTwo = new SqlParameter("@InputTwo", InputTwo);
            string usp = "LMS.usp_GetEmployeewiseAttendanceData @PersonId, @SelectType, @InputOne, @InputTwo";
            data = _dbContext._sp_GetEmployeeAttendanceData.FromSql(usp, SP_PersonId, SP_SelectType, SP_InputOne, SP_InputTwo).ToList();
            return data;
        }
    }
}

