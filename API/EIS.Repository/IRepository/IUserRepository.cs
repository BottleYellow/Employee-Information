﻿
using EIS.Entities.User;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace EIS.Repositories.IRepository
{
    public interface IUserRepository : IRepositorybase<Users>
    {
        void CreateUser(Users user);
        string ValidateUser(Users user);
        Users FindByUserName(string Username);
        JwtSecurityToken GenerateToken(int UserId);
    }
}