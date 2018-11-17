using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EIS.Repositories.IRepository
{
    public interface IRepositorybase<T> 
    {
        IEnumerable<T> FindAll();
        T FindByCondition(Expression<Func<T, bool>> expression);
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Save();
    }
}
