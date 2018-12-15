using EIS.Data.Context;
using EIS.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;


namespace EIS.Repositories.Repository
{
    public abstract class RepositoryBase<T> : IRepositorybase<T> where T : class
    {
            protected ApplicationDbContext _dbcontext { get; set; }

            public RepositoryBase(ApplicationDbContext dbcontext)
            {
                _dbcontext = dbcontext;
            }

            public IEnumerable<T> FindAll()
            {
                return _dbcontext.Set<T>();
            }

            public T FindByCondition(Expression<Func<T, bool>> expression)
            {
                return _dbcontext.Set<T>().Where(expression).FirstOrDefault();
            }

            public void Create(T entity)
            {
                  _dbcontext.Set<T>().Add(entity);
            }

            public void Update(T entity)
            {
                _dbcontext.Set<T>().Update(entity);
                
            }

            public void Delete(T entity)
            {
                 _dbcontext.Set<T>().Remove(entity);
            }

            public void Save()
            {
                 _dbcontext.SaveChanges();
          
            }
    }
}


