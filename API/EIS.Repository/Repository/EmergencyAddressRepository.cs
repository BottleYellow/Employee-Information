using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using EIS.Data.Context;
using EIS.Entities.Address;
using EIS.Repositories.IRepository;
using System.Linq;

namespace EIS.Repositories.Repository
{
    public class EmergencyAddressRepository :RepositoryBase<Emergency>,IEmergencyAddressRepository
    {
        public EmergencyAddressRepository(ApplicationDbContext dbContext) : base(dbContext)
        { }
    }
}
