using EIS.Entities.User;
using System.IdentityModel.Tokens.Jwt;

namespace EIS.Repositories.IRepository
{
    public interface IUserRepository : IRepositorybase<Users>
    {
        void CreateUser(Users user);
        string ValidateUser(Users user);
        Users FindByUserName(string Username);
        JwtSecurityToken GenerateToken(int UserId);
        bool VerifyPassword(int Id, string Password);
        void ChangePassword(int Id, string NewPassword);
    }
}
