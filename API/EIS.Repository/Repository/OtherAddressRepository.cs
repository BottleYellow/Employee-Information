using EIS.Data.Context;
using EIS.Entities.Address;
using EIS.Repositories.IRepository;

namespace EIS.Repositories.Repository
{
    public class OtherAddressRepository : RepositoryBase<Other>, IOtherAddressRepository
    {
        public OtherAddressRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

       
    }
}
