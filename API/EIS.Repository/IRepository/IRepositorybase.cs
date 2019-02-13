using EIS.Entities.Employee;
using EIS.Entities.Generic;
using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;

namespace EIS.Repositories.IRepository
{
    public interface IRepositorybase<T> 
    {
        IQueryable<T> FindAll();
        IQueryable<T> FindAllWithNoTracking();
        T FindByCondition(Expression<Func<T, bool>> expression);
        T FindByConditionWithNoTracking(Expression<Func<T, bool>> expression);
        void CreateAndSave(T entity);
        void UpdateAndSave(T entity);
        void DeleteAndSave(T entity);
        IQueryable<T> FindAllByCondition(Expression<Func<T, bool>> expression);
        ArrayList GetDataByGridCondition(Expression<Func<T, bool>> expression, SortGrid sortGrid, IQueryable<T> data);
        void ConnectionOpen();
        void ConnectionClose();
        void Dispose();
    }
}
