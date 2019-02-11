using EIS.Data.Context;
using EIS.Entities.Dashboard;
using EIS.Entities.Employee;
using EIS.Entities.Hoildays;
using EIS.Entities.User;
using EIS.Repositories.Helpers;
using EIS.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace EIS.Repositories.Repository
{
    public class LocationRepository : RepositoryBase<Locations>, ILocationRepository
    {
        public LocationRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
        public Locations ActivateLocation(int id)
        {
            Locations location = _dbContext.Locations.Where(x => x.Id == id).FirstOrDefault();
            location.IsActive = true;
            Save();
            return (location);
        }
    }
}
