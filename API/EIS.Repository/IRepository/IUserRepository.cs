
using EIS.Entities.User;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace EIS.Repositories.IRepository
{
    public interface IUserRepository : IRepositorybase<Users>
    {
        string ValidateUser(Users user);
        Users FindByUserName(string Username);
        JwtSecurityToken GenerateToken(int UserId);
        bool VerifyPassword(int Id, string Password);
        void ChangePassword(int Id, string NewPassword);

        void CreateUserAndSave(Users users);
    }
}
