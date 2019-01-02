using EIS.Data.Context;
using EIS.Entities.Employee;
using EIS.Repositories.IRepository;
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
            var results = _dbContext.Person
              .Select(p => new
              {
                  p,
                  Attendances = p.Attendance.Where(a => a.DateIn >= startOfWeek && a.DateIn<= endOfWeek)
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
