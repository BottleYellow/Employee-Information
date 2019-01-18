using EIS.Data.Context;
using EIS.Entities.Employee;
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
            var results = _dbContext.Person
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

            var results = _dbContext.Person
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
            var results = _dbContext.Person
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
    }
}
