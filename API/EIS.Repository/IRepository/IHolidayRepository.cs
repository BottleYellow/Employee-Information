using EIS.Entities.Hoildays;
using System.Collections.Generic;

namespace EIS.Repositories.IRepository
{
    public interface IHolidayRepository : IRepositorybase<Holiday>
    {
        IEnumerable<Holiday> GetHolidayByYear(string year, int loc);
        IEnumerable<Holiday> GetHolidayByMonth(string month,string year, int loc);
        IEnumerable<Holiday> GetHolidayByWeek(string firstDate, string lastDate, int loc);
    }
}
