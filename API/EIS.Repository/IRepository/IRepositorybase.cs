using EIS.Entities.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EIS.Repositories.IRepository
{
    public interface IRepositorybase<T> 
    {
        IQueryable<T> FindAll();
        T FindByCondition(Expression<Func<T, bool>> expression);
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Save();
        IQueryable<T> FindAllByCondition(Expression<Func<T, bool>> expression);
        T FindByCondition2(Expression<Func<T, bool>> expression);
        ArrayList GetDataByGridCondition(Expression<Func<T, bool>> expression, SortGrid sortGrid);
    }
}
