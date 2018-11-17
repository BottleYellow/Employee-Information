using EIS.Data.Context;
using EIS.Entities.Address;
using EIS.Repositories.IRepository;

namespace EIS.Repositories.Repository
{
    public class EmergencyAddressRepository :RepositoryBase<Emergency>,IEmergencyAddressRepository
    {
        public EmergencyAddressRepository(DbContext dbContext) : base(dbContext)
        { }
    }
}
