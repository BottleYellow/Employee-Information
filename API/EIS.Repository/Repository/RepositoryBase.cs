﻿using EIS.Data.Context;
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
    public abstract class RepositoryBase<T> : IRepositorybase<T> where T : class
    {
            protected ApplicationDbContext _dbContext { get; set; }

            public RepositoryBase(ApplicationDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public IQueryable<T> FindAll()
            {
                return _dbContext.Set<T>();
            }

            public T FindByCondition(Expression<Func<T, bool>> expression)
            {
                return _dbContext.Set<T>().AsNoTracking().Where(expression).FirstOrDefault();
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
            }

            public void Save()
            {
                 _dbContext.SaveChanges();
          
            }

        public IQueryable<T> FindAllByCondition(Expression<Func<T, bool>> expression)
        {
            return _dbContext.Set<T>().Where(expression);
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
    }
}


