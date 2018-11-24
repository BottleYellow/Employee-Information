using EIS.Data.Context;
using EIS.Entities.Address;
using EIS.Repositories.IRepository;


namespace EIS.Repositories.Repository
{
    public class PermanentAddressRepository :RepositoryBase<Permanent>,IPermanentAddressRepository
    {
        public PermanentAddressRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
