using EIS.Entities.Address;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace EIS.Repositories.IRepository
{
    public interface IOtherAddressRepository: IRepositorybase<Other>
    {
        IEnumerable<Other> FindAllByCondition(Expression<Func<Other, bool>> expression);
    }
}
