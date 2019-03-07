using EIS.Data.Context;
using EIS.Entities.Employee;
using EIS.Entities.Enums;
using EIS.Entities.Models;
using EIS.Entities.SP;
using EIS.Repositories.IRepository;
using EIS.WebAPI.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
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
            SendAttendanceReportToAdminHRManager();
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
                    Id = p.Id,
                    FullName = p.FullName,
                    EmployeeCode = p.EmployeeCode,
                    EmailAddress = p.EmailAddress
                }).ToList();

            int SrId = 0;
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
                List<EmployeeAttendanceData> data = Data(SrId,"Month", p.Id, InputOne, InputTwo, data1);
                SrId = SrId+data.Count() + 4;
                var memory = new MemoryStream();
                using (var sw = new FileStream(attendanceReportPath, FileMode.Create, FileAccess.Write))
                {
                    int i = 1;

                    IWorkbook workbook;
                    workbook = new XSSFWorkbook();
                    ISheet sheet = workbook.CreateSheet("demo");
                    IRow row = sheet.CreateRow(0);
                    ICellStyle headerStyle = workbook.CreateCellStyle();
                    headerStyle.BorderBottom = BorderStyle.Medium;
                    headerStyle.FillForegroundColor = IndexedColors.LightBlue.Index;
                    headerStyle.FillPattern = FillPattern.SolidForeground;

                    ICellStyle presentStyle = workbook.CreateCellStyle();
                    presentStyle.BorderBottom = BorderStyle.Medium;
                    presentStyle.FillForegroundColor = IndexedColors.LightGreen.Index;
                    presentStyle.FillPattern = FillPattern.SolidForeground;

                    ICellStyle absentStyle = workbook.CreateCellStyle();
                    absentStyle.BorderBottom = BorderStyle.Medium;
                    absentStyle.FillForegroundColor = IndexedColors.LightOrange.Index;
                    absentStyle.FillPattern = FillPattern.SolidForeground;

                    ICellStyle dateStyle = workbook.CreateCellStyle();
                    dateStyle.BorderBottom = BorderStyle.Medium;
                    dateStyle.FillForegroundColor = IndexedColors.LightBlue.Index;
                    dateStyle.FillPattern = FillPattern.SolidForeground;

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
                    ICell cell0 = row.CreateCell(0);
                    cell0.CellStyle = headerStyle;
                    cell0.SetCellValue("DATE");
                    ICell cell1 = row.CreateCell(1);
                    cell1.SetCellValue("STATUS");
                    cell1.CellStyle = headerStyle;
                    ICell cell2 = row.CreateCell(2);
                    cell2.CellStyle = headerStyle;
                    cell2.SetCellValue("TIME IN");
                    ICell cell3 = row.CreateCell(3);
                    cell3.CellStyle = headerStyle;
                    cell3.SetCellValue("TIME OUT");
                    ICell cell4 = row.CreateCell(4);
                    cell4.CellStyle = headerStyle;
                    cell4.SetCellValue("TOTAL HOURS");

                    foreach (var attendance in data)
                    {
                        row = sheet.CreateRow(i);
                        DateTime.Today.Add(attendance.TimeOut.GetValueOrDefault()).ToString("hh:mm tt");
          
                        if (attendance.Status == "Present")
                        {
                            ICell cell00 = row.CreateCell(0);
                            cell00.CellStyle = presentStyle;
                            cell00.SetCellValue(Convert.ToDateTime(attendance.DateIn).ToString("dd/MM/yyyy").ToString());
                            ICell cell01 = row.CreateCell(1);
                            cell01.SetCellValue(attendance.Status);
                            cell01.CellStyle = presentStyle;
                            ICell cell02 = row.CreateCell(2);
                            cell02.CellStyle = presentStyle;
                            cell02.SetCellValue(attendance.TimeIn == null ? "" : DateTime.Today.Add(attendance.TimeIn.GetValueOrDefault()).ToString("hh:mm tt"));
                            ICell cell03 = row.CreateCell(3);
                            cell03.CellStyle = presentStyle;
                            cell03.SetCellValue(attendance.TimeOut == null ? "" : DateTime.Today.Add(attendance.TimeOut.GetValueOrDefault()).ToString("hh:mm tt"));
                            ICell cell04 = row.CreateCell(4);
                            cell04.CellStyle = presentStyle;
                            cell04.SetCellValue(attendance.TotalHours == null ? "" : attendance.TotalHours.ToString());
                        }
                        else
                        {
                            ICell cell00 = row.CreateCell(0);
                            cell00.CellStyle = absentStyle;
                            cell00.SetCellValue(Convert.ToDateTime(attendance.DateIn).ToString("dd/MM/yyyy").ToString());
                            ICell cell01 = row.CreateCell(1);
                            cell01.SetCellValue(attendance.Status);
                            cell01.CellStyle = absentStyle;
                            ICell cell02 = row.CreateCell(2);
                            cell02.CellStyle = absentStyle;
                            cell02.SetCellValue(attendance.TimeIn == null ? "" : DateTime.Today.Add(attendance.TimeIn.GetValueOrDefault()).ToString("hh:mm tt"));
                            ICell cell03 = row.CreateCell(3);
                            cell03.CellStyle = absentStyle;
                            cell03.SetCellValue(attendance.TimeOut == null ? "" : DateTime.Today.Add(attendance.TimeOut.GetValueOrDefault()).ToString("hh:mm tt"));
                            ICell cell04 = row.CreateCell(4);
                            cell04.CellStyle = absentStyle;
                            cell04.SetCellValue(attendance.TotalHours == null ? "" : attendance.TotalHours.ToString());
                        }

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
                    workbook.Close();
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

        public List<EmployeeAttendanceData> Data(int SrId,string Type, int PersonId, string InputOne, string InputTwo, List<EmployeeAttendanceData> data)
        {
            var SP_SrId = new SqlParameter("@SrId", SrId);
            var SP_SelectType = new SqlParameter("@SelectType", "Month");
            var SP_PersonId = new SqlParameter("@PersonId", PersonId);
            var SP_InputOne = new SqlParameter("@InputOne", InputOne);
            var SP_InputTwo = new SqlParameter("@InputTwo", InputTwo);
            string usp = "LMS.usp_GetEmployeewiseAttendanceData @SrId, @PersonId, @SelectType, @InputOne, @InputTwo";
            data = _dbContext._sp_GetEmployeeAttendanceData.FromSql(usp, SP_SrId, SP_PersonId, SP_SelectType, SP_InputOne, SP_InputTwo).ToList();         
            return data;
        }

        public void SendAttendanceReportToAdminHRManager()
        {
            var year = DateTime.Now.ToString("yyyy");
            DateTime d = DateTime.Now;
            d = d.AddMonths(-1);
            int month = d.Month;
            string monthName = new DateTime(2000, month, 1).ToString("MMM", CultureInfo.InvariantCulture);
            var rootPath = @"C:\Temp\AttendanceReportToAdmin\";
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

            int locationId = 0;
            var SearchFor = "Month";
            char c = '0';
            var InputOne = DateTime.Now.AddMonths(-1).Month.ToString().PadLeft(2, c);
            var InputTwo = DateTime.Now.Year.ToString();

            IList<AttendanceData> attendanceData = new List<AttendanceData>();
            Attendance_Report Model = new Attendance_Report();
            Model.sP_GetAttendanceCountReports = new List<SP_GetAttendanceCountReport>();

            var SP_locationId = new SqlParameter("@locationId", locationId);
            var SP_SelectType = new SqlParameter("@SelectType", SearchFor);
            var SP_InputOne = new SqlParameter("@InputOne", InputOne);
            var SP_InputTwo = new SqlParameter("@InputTwo", InputTwo);
            string usp = "LMS.usp_GetAttendanceCountReport @locationId, @SelectType, @InputOne, @InputTwo";
            Model.sP_GetAttendanceCountReports = _dbContext._sp_GetAttendanceCountReport.FromSql(usp, SP_locationId, SP_SelectType, SP_InputOne, SP_InputTwo).ToList();

            string attendanceReportPath = @"C:\Temp\AttendanceReportToAdmin\" + year + "\\" + monthName + "\\" + "AttendanceReport.xlsx";
            if (File.Exists(attendanceReportPath))
            { 
                File.Delete(attendanceReportPath);
            }
            var memory = new MemoryStream();
            using (var sw = new FileStream(attendanceReportPath, FileMode.Create, FileAccess.Write))
            {
                int i = 1;

                IWorkbook workbook;
                ICell cell;
                workbook = new XSSFWorkbook();
                ICellStyle headerStyle = workbook.CreateCellStyle();
                ISheet sheet = workbook.CreateSheet("Attendance Report");
                IRow row = sheet.CreateRow(0);
                row = sheet.CreateRow(i++);
                cell = row.CreateCell(0);
                cell.SetCellValue("Aadyam Consultant llp.");
                row = sheet.CreateRow(i++);
                cell = row.CreateCell(0);
                cell.SetCellValue("Employee Management System");
                row = sheet.CreateRow(i++);
                cell = row.CreateCell(0);
                cell.SetCellValue("Monthly Attendance Report:-");
                cell = row.CreateCell(1);
                cell.SetCellValue(month + "/" + year);
                row = sheet.CreateRow(i++);
                row = sheet.CreateRow(i++);
                cell = row.CreateCell(0);
                headerStyle.BorderBottom = BorderStyle.Medium;
                headerStyle.FillForegroundColor = IndexedColors.LightYellow.Index;
                headerStyle.FillPattern = FillPattern.SolidForeground;
                cell.CellStyle = headerStyle;
                cell.SetCellValue("Employee Code");
                cell = row.CreateCell(1);
                headerStyle.BorderBottom = BorderStyle.Medium;
                headerStyle.FillForegroundColor = IndexedColors.LightYellow.Index;
                headerStyle.FillPattern = FillPattern.SolidForeground;
                cell.CellStyle = headerStyle;
                cell.SetCellValue("Employee Name");
                cell = row.CreateCell(2);
                headerStyle.BorderBottom = BorderStyle.Medium;
                headerStyle.FillForegroundColor = IndexedColors.LightYellow.Index;
                headerStyle.FillPattern = FillPattern.SolidForeground;
                cell.CellStyle = headerStyle;
                cell.SetCellValue("Working Days");
                cell = row.CreateCell(3);
                headerStyle.FillBackgroundColor = IndexedColors.LightYellow.Index;
                cell.CellStyle = headerStyle;
                cell.SetCellValue("Leave Without Approval");
                cell = row.CreateCell(4);
                headerStyle.BorderBottom = BorderStyle.Medium;
                headerStyle.FillForegroundColor = IndexedColors.LightYellow.Index;
                headerStyle.FillPattern = FillPattern.SolidForeground;
                cell.CellStyle = headerStyle;
                cell.SetCellValue("Leave With Approval");
                cell = row.CreateCell(5);
                headerStyle.BorderBottom = BorderStyle.Medium;
                headerStyle.FillForegroundColor = IndexedColors.LightYellow.Index;
                headerStyle.FillPattern = FillPattern.SolidForeground;
                cell.CellStyle = headerStyle;
                cell.SetCellValue("Total Leaves");
                cell = row.CreateCell(6);
                headerStyle.BorderBottom = BorderStyle.Medium;
                headerStyle.FillForegroundColor = IndexedColors.LightYellow.Index;
                headerStyle.FillPattern = FillPattern.SolidForeground;
                cell.CellStyle = headerStyle;
                cell.SetCellValue("Present Days");

                ICellStyle headerStyle1 = workbook.CreateCellStyle();
                //headerStyle1.BorderBottom = BorderStyle.Medium;
                headerStyle1.FillForegroundColor = IndexedColors.LightGreen.Index;
                headerStyle1.FillPattern = FillPattern.SolidForeground;

                ICellStyle headerStyle2 = workbook.CreateCellStyle();
                //headerStyle2.BorderBottom = BorderStyle.Medium;
                headerStyle2.FillForegroundColor = IndexedColors.PaleBlue.Index;
                headerStyle2.FillPattern = FillPattern.SolidForeground;

                ICellStyle headerStyle3 = workbook.CreateCellStyle();
                //headerStyle3.BorderBottom = BorderStyle.Medium;
                headerStyle3.FillForegroundColor = IndexedColors.LightCornflowerBlue.Index;
                headerStyle3.FillPattern = FillPattern.SolidForeground;

                ICellStyle headerStyle4 = workbook.CreateCellStyle();
                //headerStyle4.BorderBottom = BorderStyle.Medium;
                headerStyle4.FillForegroundColor = IndexedColors.Rose.Index;
                headerStyle4.FillPattern = FillPattern.SolidForeground;

                ICellStyle headerStyle5 = workbook.CreateCellStyle();
                //headerStyle5.BorderBottom = BorderStyle.Medium;
                headerStyle5.FillForegroundColor = IndexedColors.LightOrange.Index;
                headerStyle5.FillPattern = FillPattern.SolidForeground;

                ICellStyle headerStyle6 = workbook.CreateCellStyle();
                //headerStyle6.BorderBottom = BorderStyle.Medium;
                headerStyle6.FillForegroundColor = IndexedColors.LemonChiffon.Index;
                headerStyle6.FillPattern = FillPattern.SolidForeground;

                ICellStyle headerStyle7 = workbook.CreateCellStyle();
                //headerStyle7.BorderBottom = BorderStyle.Medium;
                headerStyle7.FillForegroundColor = IndexedColors.LightGreen.Index;
                headerStyle7.FillPattern = FillPattern.SolidForeground;

                sheet.SetColumnWidth(0, 5000);
                sheet.SetColumnWidth(1, 7000);
                sheet.SetColumnWidth(2, 4000);
                sheet.SetColumnWidth(3, 5000);
                sheet.SetColumnWidth(4, 5000);
                sheet.SetColumnWidth(5, 3000);
                sheet.SetColumnWidth(6, 3000);

                foreach (var attendance in Model.sP_GetAttendanceCountReports)
                {
                    row = sheet.CreateRow(i);
                    cell=row.CreateCell(0);
                    cell.CellStyle = headerStyle1;
                    cell.SetCellValue(attendance.EmployeeCode);
                    cell=row.CreateCell(1);
                    cell.CellStyle = headerStyle2;
                    cell.SetCellValue(attendance.EmployeeName);
                    cell=row.CreateCell(2);
                    cell.CellStyle = headerStyle3;
                    cell.SetCellValue(Convert.ToInt32(attendance.WorkingDay));
                    cell=row.CreateCell(3);
                    cell.CellStyle = headerStyle4;
                    cell.SetCellValue(Convert.ToInt32(attendance.AbsentDay));
                    cell=row.CreateCell(4);
                    cell.CellStyle = headerStyle5;
                    cell.SetCellValue(Convert.ToInt32(attendance.NoLeave));
                    cell=row.CreateCell(5);
                    cell.CellStyle = headerStyle6;
                    cell.SetCellValue(Convert.ToInt32(attendance.NoLeave) + Convert.ToInt32(attendance.AbsentDay));
                    cell=row.CreateCell(6);
                    cell.CellStyle = headerStyle7;
                    cell.SetCellValue(Convert.ToInt32(attendance.PresentDays));
                    i++;
                }
                workbook.Write(sw);
                workbook.Close();
            }

            var persons = _repository.Employee.getAdminHrManager()
            .Select(p => new
            {
                EmailAddress = p.EmailAddress
            }).ToList();

            foreach (var p in persons)
            {
                string To = p.EmailAddress;
                string subject = "EMS Monthly Attendance Report";
                string body = "Dear Sir/Maa'm \n" +
                "Kindly find monthly attendance report in attachment";
                new EmailManager(_configuration, _repository).SendEmail(subject, body, To, attendanceReportPath);
            }
        }

    }
}