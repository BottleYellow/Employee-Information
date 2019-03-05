using EIS.Data.Context;
using EIS.Entities.Hoildays;
using EIS.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EIS.Repositories.Repository
{
    public class HolidayRepository : RepositoryBase<Holiday>, IHolidayRepository
    {
        public HolidayRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }
        public IEnumerable<Holiday> GetHolidayByYear(string year, int loc)
        {
            List<Holiday> holiday = new List<Holiday>();
            if (loc == 0)
            {
                holiday = _dbContext.Holidays.Include(x => x.Location).Where(x=>x.IsActive==true && x.Date.Year == Convert.ToInt32(year)).ToList();
            }
            else
            {
                holiday = _dbContext.Holidays.Include(x => x.Location).Where(x => x.IsActive == true && x.LocationId == loc && x.Date.Year == Convert.ToInt32(year)).ToList();
            } 
            return holiday;
        }
        public IEnumerable<Holiday> GetHolidayByMonth(string month, string year, int loc)
        {
            List<Holiday> holiday = loc == 0 ? _dbContext.Holidays.Include(x => x.Location).Where(x => x.IsActive == true && x.Date.Month == Convert.ToInt32(month) && x.Date.Year == Convert.ToInt32(year)).ToList() : _dbContext.Holidays.Include(x => x.Location).Where(x => x.IsActive == true && x.LocationId == loc && x.Date.Month == Convert.ToInt32(month) && x.Date.Year == Convert.ToInt32(year)).ToList();
            return holiday;
        }
        public IEnumerable<Holiday> GetHolidayByWeek(string firstDate, string lastDate, int loc)
        {
            List<Holiday> holiday = loc == 0 ? _dbContext.Holidays.Include(x => x.Location).Where(x => x.IsActive == true && x.Date>=Convert.ToDateTime(firstDate) && x.Date<=Convert.ToDateTime(lastDate)).ToList() : _dbContext.Holidays.Include(x => x.Location).Where(x => x.IsActive == true && x.LocationId == loc && (x.Date >= Convert.ToDateTime(firstDate) && x.Date <= Convert.ToDateTime(lastDate))).ToList();
            return holiday;
        }
    }
}
