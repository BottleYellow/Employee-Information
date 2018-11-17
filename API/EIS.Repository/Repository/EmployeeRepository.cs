using EIS.Data.Context;
using EIS.Entities.Employee;
using EIS.Repositories.IRepository;

namespace EIS.Repositories.Repository
{
    public class EmployeeRepository :RepositoryBase<Person>,IEmployeeRepository
    {
        public EmployeeRepository(DbContext dbContext) : base(dbContext)
        {

        }

    }
}
