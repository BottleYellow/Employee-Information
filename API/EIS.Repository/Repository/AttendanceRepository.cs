﻿using EIS.Data.Context;
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

        public IQueryable<Person> GetAttendanceYearly(int year)
        {
            var results = _dbContext.Person.Include(x => x.Role).Where(x => x.Role.Name != "Admin")
                .Select(p => new
                {
                    p,
                    Attendances = p.Attendance.Where(a => a.DateIn.Year == year)
                });
                
            foreach (var x in results)
            {
                x.p.Attendance = x.Attendances.ToList();
            }
            var result = results.Select(x => x.p);
            return result;
        }
       
        public IQueryable<Person> GetAttendanceMonthly(int month, int year)
        {

            var results = _dbContext.Person.Include(x=>x.Role).Where(x=>x.Role.Name!="Admin")
                .Select(p => new
                {
                    p,
                    Attendances = p.Attendance.Where(a => a.DateIn.Year == year && a.DateIn.Month == month)
                }).ToList();
            foreach (var x in results)
            {
                x.p.Attendance = x.Attendances.ToList();
            }
            var result = results.Select(x => x.p).ToList();
            return result.AsQueryable();
        }

        public IQueryable<Person> GetAttendanceWeekly(DateTime startOfWeek, DateTime endOfWeek)
        {
            var results = _dbContext.Person.Include(x => x.Role).Where(x => x.Role.Name != "Admin")
              .Select(p => new
              {
                  p,
                  Attendances = p.Attendance.Where(a => a.DateIn >= startOfWeek && a.DateIn<= endOfWeek)
              });
            foreach (var x in results)
            {
                x.p.Attendance = x.Attendances.ToList();
            }
            var result = results.Select(x => x.p);
            
            return result;
        }

        public AttendanceReport GetAttendanceReportSummary(int TotalDays, IQueryable<Attendance> attendanceData)
        {
            AttendanceReport attendanceReport = new AttendanceReport();
            attendanceReport.TotalDays = TotalDays;

            attendanceReport.PresentDays = attendanceData.Count();
            attendanceReport.AbsentDays = attendanceReport.TotalDays - attendanceReport.PresentDays;
            if (attendanceReport.PresentDays == 0)
            {
                attendanceReport.TimeIn = "00:00";
                attendanceReport.TimeOut = "00:00";
                attendanceReport.AverageTime = "00:00";
            }
            else
            {

                TimeSpan averageTimeIn = new TimeSpan(Convert.ToInt64(attendanceData.Average(x => x.TimeIn.Ticks)));
                DateTime timeIn = DateTime.Today.Add(averageTimeIn);
                attendanceReport.TimeIn = timeIn.ToString("hh:mm tt");
                TimeSpan averageTimeOut = new TimeSpan(Convert.ToInt64(attendanceData.Average(x => x.TimeOut!=null?  Convert.ToDateTime(x.TimeOut).Ticks:DateTime.Now.Ticks )));
                DateTime timeOut = DateTime.Today.Add(averageTimeOut);
                attendanceReport.TimeOut = timeOut.ToString("hh:mm tt");
                var hour = timeOut - timeIn;
                DateTime avgHours = DateTime.Today.Add(hour);
                attendanceReport.AverageTime = hour.Hours.ToString()+":"+hour.Minutes.ToString();
            }
            return attendanceReport;
        }

        public IEnumerable<Attendance> GetAttendanceReportByDate(DateTime startDate, DateTime endDate, IQueryable<Attendance> attendanceData)
        {
            IList<Attendance> attendances = new List<Attendance>();
            for (DateTime date = startDate; date < endDate; date = date.AddDays(1))
            {
                Attendance newlist = new Attendance();
                newlist.DateIn = date;
                var attendance = attendanceData.Where(x => x.DateIn == date).Select(x => new { x.TimeIn, x.TimeOut,x.TotalHours }).FirstOrDefault();
                if (attendance == null || attendance.TimeIn == attendance.TimeOut)
                {
                    newlist.TimeIn = new TimeSpan();
                    newlist.TimeOut = new TimeSpan();
                    newlist.IsActive = false;
                    newlist.TotalHours= new TimeSpan();
                }
                else
                {
                    newlist.TimeIn = attendance.TimeIn;
                    newlist.TimeOut = attendance.TimeOut;
                    newlist.IsActive = true;
                    newlist.TotalHours = attendance.TotalHours;
                }
                attendances.Add(newlist);
            }
            IEnumerable<Attendance> attendancelist = attendances;
            return  attendancelist;
        }

        public IEnumerable<Attendance> GetAttendanceDateWise(DateTime startDate, DateTime endDate, IEnumerable<Attendance> attendanceData)
        {
            IList<Attendance> attendances = new List<Attendance>();
            for (DateTime date = startDate; date < endDate; date = date.AddDays(1))
            {
                Attendance newlist = new Attendance();
                newlist.DateIn = date;

                var attendance = attendanceData.Where(x => x.DateIn == date).Select(x => new { x.TimeIn, x.TimeOut, x.TotalHours }).FirstOrDefault();

                if (attendance == null || attendance.TimeIn == attendance.TimeOut)
                {
                    if (date < DateTime.Now.Date)
                    {

                        newlist.TimeIn = new TimeSpan();
                        newlist.TimeOut = new TimeSpan();
                        newlist.IsActive = false;
                        newlist.TotalHours = new TimeSpan();

                    }
                }
                else
                {
                    newlist.TimeIn = attendance.TimeIn;
                    newlist.TimeOut = attendance.TimeOut;
                    newlist.IsActive = true;
                    newlist.TotalHours = attendance.TotalHours;
                }
                attendances.Add(newlist);
            }
            IEnumerable<Attendance> attendancelist = attendances;
                return attendancelist;
            }
        }
}
