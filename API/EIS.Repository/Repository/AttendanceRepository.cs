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
        protected ApplicationDbContext _dbContext { get; set; }
        public AttendanceRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Person> GetAttendanceYearly(int year)
        {
            var results = _dbContext.Person.Where(x => x.Attendance.Any(y => y.DateIn.Year == year)).Select(x => new Person() { FirstName = x.FirstName, LastName = x.LastName, Attendance = x.Attendance.Where(y => y.DateIn.Year == year).ToList() }).ToList();

            //var results = _dbContext.Person.Where(x=>x.Attendance.All(y=>y.DateIn.Year==year)).Select(x => new Person { FirstName = x.FirstName, LastName = x.LastName, Attendance = x.Attendance, }).ToList();
            return results;
        }
        
    }
}
