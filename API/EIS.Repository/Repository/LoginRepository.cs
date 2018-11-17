using EIS.Data.Context;
using EIS.Entities.Admin;
using EIS.Repositories.IRepository;


namespace EIS.Repositories.Repository
{
    public class LoginRepository : RepositoryBase<Login>, ILoginRepository
    {
        public LoginRepository(DbContext dbContext) : base(dbContext)
        {

        }
    }
}
