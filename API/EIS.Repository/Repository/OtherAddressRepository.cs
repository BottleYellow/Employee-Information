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

        public IEnumerable<Other> FindAllByCondition(Expression<Func<Other, bool>> expression)
        {
            return _dbcontext.Set<Other>().Where(expression);
        }
    }
}
