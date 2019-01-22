using EIS.Entities.Dashboard;
using EIS.Entities.User;
using System.IdentityModel.Tokens.Jwt;

namespace EIS.Repositories.IRepository
{
    public interface IDashboardRepository : IRepositorybase<AdminDashboard>
    {
        AdminDashboard GetAdminDashboard(int TenantId);
        ManagerDashboard GetManagerDashboard(int TenantId);
        EmployeeDashboard GetEmployeeDashboard(int TenantId,int PersonId);
    }
}
