using EIS.Data.Context;
using EIS.Entities.Address;
using EIS.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Linq;

namespace EIS.Repositories.Repository
{
    public class OtherAddressRepository : RepositoryBase<Other>, IOtherAddressRepository
    {
        public OtherAddressRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

       
    }
}
