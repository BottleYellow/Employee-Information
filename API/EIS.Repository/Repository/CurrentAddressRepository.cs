using EIS.Data.Context;
using EIS.Entities.Address;
using EIS.Repositories.IRepository;

namespace EIS.Repositories.Repository
{
    public class CurrentAddressRepository :RepositoryBase<Current>,ICurrentAddressRepository
    {
        public CurrentAddressRepository(DbContext dbContext) : base(dbContext)
        { }
    }
}
