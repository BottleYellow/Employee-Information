using EIS.Data.Context;
using EIS.Entities.Employee;
using EIS.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EIS.Repositories.Repository
{
    public class AttendanceRepository : RepositoryBase<Attendance>, IAttendanceRepository
    {
<<<<<<< HEAD
        //private readonly ApplicationDbContext _dbContext;
        public AttendanceRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            //_dbContext = dbContext;
=======
        public AttendanceRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
>>>>>>> eab0133b5e8f6e86eb09bb18611280e9b8dcee1c
        }
        public IEnumerable<Person> GetAttendanceYearly(int year)
        {          
            var results = _dbContext.Person
                .Select(p => new
                {
                    p,
                    Attendances = p.Attendance.Where(a => a.DateIn.Year == year)
                })
                .ToList();
            foreach (var x in results)
            {
                x.p.Attendance = x.Attendances.ToList();
            }
            var result = results.Select(x => x.p).ToList();
            return result;
        }

        public IEnumerable<Person> GetAttendanceMonthly(int month, int year)
        {
            var results = _dbContext.Person
                .Select(p => new
                {
                    p,
                    Attendances = p.Attendance.Where(a => a.DateIn.Year == year && a.DateIn.Month==month)
                })
                .ToList();
            foreach (var x in results)
            {
                x.p.Attendance = x.Attendances.ToList();
            }
            var result = results.Select(x => x.p).ToList();
            return result;
        }

        public IEnumerable<Person> GetAttendanceWeekly(DateTime startOfWeek, DateTime endOfWeek)
        {
<<<<<<< HEAD
            var results = _dbContext.Person.Include("Attendance")
                .Select(p => new
                {
                    p,
                    Attendances = p.Attendance.Where(a => a.DateIn.Year == year).ToList()
                })
                .ToList();
            //foreach (var x in results)
            //{
            //    x.p.Attendance = x.Attendances.ToList();
            //}
            var result = results.Select(x => x.p).ToList();
            return result;
        }

        public IEnumerable<Person> GetAttendanceMonthly(int month, int year)
        {
            var results = _dbContext.Person
                .Select(p => new
                {
                    p,
                    Attendances = p.Attendance.Where(a => a.DateIn.Year == year && a.DateIn.Month == month)
                })
                .ToList();
            foreach (var x in results)
            {
                x.p.Attendance = x.Attendances.ToList();
            }
            var result = results.Select(x => x.p).ToList();
            return result;
        }

        public IEnumerable<Person> GetAttendanceWeekly(DateTime startOfWeek, DateTime endOfWeek)
        {
=======
>>>>>>> eab0133b5e8f6e86eb09bb18611280e9b8dcee1c
            var results = _dbContext.Person
              .Select(p => new
              {
                  p,
<<<<<<< HEAD
                  Attendances = p.Attendance.Where(a => a.DateIn >= startOfWeek && a.DateIn <= endOfWeek)
=======
                  Attendances = p.Attendance.Where(a => a.DateIn >= startOfWeek && a.DateIn<= endOfWeek)
>>>>>>> eab0133b5e8f6e86eb09bb18611280e9b8dcee1c
              })
              .ToList();
            foreach (var x in results)
            {
                x.p.Attendance = x.Attendances.ToList();
            }
            var result = results.Select(x => x.p).ToList();
            return result;
        }
    }
}
