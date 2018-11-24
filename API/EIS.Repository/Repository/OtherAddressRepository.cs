using EIS.Data.Context;
using EIS.Entities.Address;
using EIS.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace EIS.Repositories.Repository
{
    public class OtherAddressRepository : RepositoryBase<Other>, IOtherAddressRepository
    {
        public OtherAddressRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
