using EIS.Data.Context;
using EIS.Entities.Generic;
using EIS.Repositories.IRepository;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;


namespace EIS.Repositories.Repository
{
    public abstract class RepositoryBase<T> : IRepositorybase<T> where T : class
    {
<<<<<<< HEAD
            public ApplicationDbContext _dbContext { get; set; }
=======
            protected ApplicationDbContext _dbContext { get; set; }
>>>>>>> eab0133b5e8f6e86eb09bb18611280e9b8dcee1c

            public RepositoryBase(ApplicationDbContext dbContext)
            {
<<<<<<< HEAD
                _dbContext = dbcontext;
=======
                _dbContext = dbContext;
>>>>>>> eab0133b5e8f6e86eb09bb18611280e9b8dcee1c
            }

            public IQueryable<T> FindAll()
            {
                return _dbContext.Set<T>();
            }

            public T FindByCondition(Expression<Func<T, bool>> expression)
            {
                return _dbContext.Set<T>().Where(expression).FirstOrDefault();
            }

            public void Create(T entity)
            {
                  _dbContext.Set<T>().Add(entity);
            }

            public void Update(T entity)
            {
                _dbContext.Set<T>().Update(entity);
                
            }

            public void Delete(T entity)
            {
                 _dbContext.Set<T>().Remove(entity);
<<<<<<< HEAD

=======
>>>>>>> eab0133b5e8f6e86eb09bb18611280e9b8dcee1c
            }

            public void Save()
            {
                 _dbContext.SaveChanges();
          
            }

        public IQueryable<T> FindAllByCondition(Expression<Func<T, bool>> expression)
        {
            return _dbContext.Set<T>().Where(expression);
        }

        public T FindByCondition2(Expression<Func<T, bool>> expression)
        {
            return _dbContext.Set<T>().Where(expression).LastOrDefault();
        }

        public ArrayList GetDataByGridCondition(Expression<Func<T, bool>> expression, SortGrid sortGrid)
        {

            int totalcount = 0;

            ArrayList list = new ArrayList();
            var data = FindAll();

            if (!(string.IsNullOrEmpty(sortGrid.SortColumn) && string.IsNullOrEmpty(sortGrid.SortColumnDirection)))
            {
                data = data.OrderBy(sortGrid.SortColumn + " " + sortGrid.SortColumnDirection);
            }
            if (expression != null)
            {
                data = data.Where(expression);
                totalcount = data.Count();
            }
            else
            {
                totalcount = data.Count();
                if (sortGrid.PageSize == -1)
                {
                    sortGrid.PageSize = totalcount;
                }              
            }

            data = data.Skip(sortGrid.Skip).Take(sortGrid.PageSize);          
            var totaldata = data.ToList();

            list.Add(totalcount);
            list.Add(totaldata);

            return list;
        }

        public void CreateAndSave(T entity)
        {
            Create(entity);
            Save();
        }

        public void UpdateAndSave(T entity)
        {
            Update(entity);
            Save();
        }
        public void DeleteAndSave(T entity)
        {
            Delete(entity);
            Save();
        }
    }
}


