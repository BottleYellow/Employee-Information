using EIS.Data.Context;
using EIS.Entities.Employee;
using EIS.Repositories.IRepository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EIS.Repositories.Repository
{
    public class AttendanceRepository: RepositoryBase<Attendance>,IAttendanceRepository
    {
        public AttendanceRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }
       
    }
}
