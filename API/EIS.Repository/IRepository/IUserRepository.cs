using EIS.Entities.SP;
using EIS.Entities.User;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

namespace EIS.Repositories.IRepository
{
    public interface IUserRepository : IRepositorybase<Users>
    {
        string ValidateUser(Users user);
        Users FindByUserName(string Username);
        JwtSecurityToken GenerateToken(int UserId);
        bool VerifyPassword(int Id, string Password);
        void ChangePasswordAndSave(int Id, string NewPassword);
        void CreateUserAndSave(Users users);
        List<MailConfiguration> GetMailConfiguration();
    }
}
 