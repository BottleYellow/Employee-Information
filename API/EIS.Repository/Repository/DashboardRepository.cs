using EIS.Data.Context;
using EIS.Entities.Dashboard;
using EIS.Entities.Employee;
using EIS.Entities.User;
using EIS.Repositories.Helpers;
using EIS.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace EIS.Repositories.Repository
{
    public class DashboardRepository : RepositoryBase<AdminDashboard>, IDashboardRepository
    {
        public DashboardRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public AdminDashboard GetAdminDashboard(int TenantId)
        {
            var employees = _dbContext.Person.Include(x=>x.Attendance).Where(x => x.TenantId == TenantId && x.IsActive == true).AsQueryable();
            var leaves = _dbContext.LeaveRequests.Where(x => x.Status == "Pending" && x.TenantId == TenantId).Count();
            int pcount = 0;
            foreach(var pa in employees)
            {
                foreach (var a in pa.Attendance)
                {
                    if (a.DateIn.Date == DateTime.Now.Date)
                    {
                        pcount++;
                    }
                }
            }
            AdminDashboard dashboard = new AdminDashboard
            {
                AllEmployeesCount = employees.Count(),
                PresentEmployees = pcount,
                AbsentEmployees = employees.Count() - pcount,
                PendingLeavesCount = leaves
            };
            return dashboard;
        }

        public ManagerDashboard GetManagerDashboard(int TenantId)
        {
            var employees = _dbContext.Person.Include(x => x.Attendance).Where(x => x.TenantId == TenantId && x.IsActive == true).AsQueryable();
            var leaves = _dbContext.LeaveRequests.Where(x => x.Status == "Pending" && x.TenantId == TenantId).Count();
            int pcount = 0;
            foreach (var pa in employees)
            {
                foreach (var a in pa.Attendance)
                {
                    if (a.DateIn.Date == DateTime.Now.Date)
                    {
                        pcount++;
                    }
                }
            }
            ManagerDashboard dashboard = new ManagerDashboard
            {
            
            };


            return dashboard;
        }
        public EmployeeDashboard GetEmployeeDashboard(int TenantId,int PersonId)
        {
            int currentMonth = new DateTime().Month;
            int currentYear = new DateTime().Year;
            int TotalDays = DateTime.DaysInMonth(currentYear, currentMonth);
            var attendance = _dbContext.Attendances.Where(x => x.PersonId == PersonId && x.DateIn.Month==currentMonth&& x.DateIn.Year==currentYear);
            var leaves = _dbContext.LeaveRequests.Where(x => x.PersonId == PersonId && x.TenantId == TenantId && x.Status == "Approved").Select(x => x.RequestedDays);


            EmployeeDashboard dashboard = new EmployeeDashboard
            {
                MonthlyPresentDays = attendance.Count(),
                MonthlyAbsentDays= TotalDays - attendance.Count(),
                TotalLeavesAvailable=9,
                TotalLeavesTaken=6
            };
            return dashboard;
        }


    }
}
