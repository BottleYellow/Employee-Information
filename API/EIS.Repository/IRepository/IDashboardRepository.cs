using EIS.Entities.Dashboard;
using EIS.Entities.Models;
using EIS.Entities.SP;
using EIS.Entities.User;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

namespace EIS.Repositories.IRepository
{
    public interface IDashboardRepository : IRepositorybase<AdminDashboard>
    {
        Admin_Dashboard GetAdminDashboard(string attendanceStatus, int location,int TenantId);
        ManagerDashboard GetManagerDashboard(int TenantId);
        Employee_Dashboard GetEmployeeDashboard(int TenantId,int PersonId);
        List<CalendarData> GetAdminCalendarDetails(int location,DateTime beginDate, DateTime stopDate);
        List<CalendarData> GetEmployeeCalendarDetails(int location, DateTime beginDate, DateTime stopDate);
        string CalculateDate(DateTime date);
    }
}
