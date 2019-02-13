using EIS.Data.Context;
using EIS.Entities.Employee;
using EIS.Entities.Generic;
using EIS.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;


namespace EIS.Repositories.Repository
{
    public abstract class RepositoryBase<T> : IDisposable , IRepositorybase<T> where T : class
    {
        protected ApplicationDbContext _dbContext { get; set; }

        public RepositoryBase(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<T> FindAll()
        {
            IQueryable<T> data = _dbContext.Set<T>().AsNoTracking();
            return data;
        }

        public T FindByCondition(Expression<Func<T, bool>> expression)
        {
            T entity = _dbContext.Set<T>().Where(expression).AsNoTracking().FirstOrDefault();
            return entity;
        }

        public void Create(T entity)
        {
            _dbContext.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            //_dbContext.Set<T>().Update(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
            _dbContext.Set<T>().Update(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
        }

        public void Save()
        {
            _dbContext.SaveChanges();

        }

        public IQueryable<T> FindAllByCondition(Expression<Func<T, bool>> expression)
        {
            IQueryable<T> data = _dbContext.Set<T>().Where(expression);
            return data;
        }

        public ArrayList GetDataByGridCondition(Expression<Func<T, bool>> expression, SortGrid sortGrid, IQueryable<T> data)
        {
            int totalcount = 0;
            ArrayList arrayList= new ArrayList();

            if (string.IsNullOrEmpty(sortGrid.SortColumn) || string.IsNullOrEmpty(sortGrid.SortColumnDirection))
            { }
            else { 
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
            arrayList.Add(totalcount);
            arrayList.Add(totaldata);

            return arrayList;
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

        public IQueryable<T> FindAllWithNoTracking()
        {
            IQueryable<T> data = _dbContext.Set<T>().AsNoTracking();
            return data;
        }

        public T FindByConditionWithNoTracking(Expression<Func<T, bool>> expression)
        {
            T entity = _dbContext.Set<T>().AsNoTracking().Where(expression).FirstOrDefault();
            return entity;
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void ConnectionOpen()
        {
            if (_dbContext != null)
            {
                _dbContext.Database.OpenConnection();
            }
        }

        public void ConnectionClose()
        {
            if (_dbContext != null)
            {
                _dbContext.Database.CloseConnection();
            }
        }
    }
}


