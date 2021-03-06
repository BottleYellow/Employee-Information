﻿using EIS.Entities.Dashboard;
using EIS.Entities.Employee;
using EIS.Entities.Hoildays;
using EIS.Entities.User;
using System.IdentityModel.Tokens.Jwt;

namespace EIS.Repositories.IRepository
{
    public interface ILocationRepository : IRepositorybase<Locations>
    {
        Locations ActivateLocation(int id);
    }
}
