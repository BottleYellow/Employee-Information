using EIS.Data.Context;
using EIS.Entities.User;
using EIS.Repositories.Helpers;
using EIS.Repositories.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Management;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace EIS.Repositories.Repository
{
    public class UserRepository : RepositoryBase<Users>, IUserRepository
    {
        public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
        public void CreateUser(Users user)
        {
            var password = Helper.Encrypt(user.Password);
            user.Password = password;
            user.EmailConfirmed = false;
            user.PhoneConfirmed = false;
            user.TwoFactorEnabled = false;
            user.IsActive = true;
            user.CreatedDate = DateTime.Now;
            user.UpdatedDate = DateTime.Now;
            Create(user);
        }
<<<<<<< HEAD

        public void CreateUserAndSave(Users user)
        {
            CreateUser(user);
            Save();
        }

=======
>>>>>>> eab0133b5e8f6e86eb09bb18611280e9b8dcee1c
        public Users FindByUserName(string Username)
        {
            var user = _dbContext.Users.Where(u => u.UserName == Username).FirstOrDefault();
            return user;
        }
        public string ValidateUser(Users user)
        {
            string result = "Failed";
            Users u = FindByUserName(user.UserName);
            if (u != null)
            {
                string hp = _dbContext.Users.Where(u1 => u1.UserName == u.UserName).Select(u1 => u1.Password).FirstOrDefault();
                if (Helper.VerifyHashedPassword(hp, user.Password) == "Success")
                {
                    result = "success";
                }
            }
            return result;     
        }

        public JwtSecurityToken GenerateToken(int UserId)
        {
            var date = DateTime.Now.ToString();
            DateTime convertedDate = DateTime.SpecifyKind(
                DateTime.Parse(date),
                DateTimeKind.Utc);
            DateTime dt = convertedDate.ToLocalTime();
            var claimsdata = new[] { new Claim(ClaimTypes.Name, UserId.ToString()) };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("askjdkasdakjsdaksdasdjaksjdadfgdfgkjdda"));
            var signInCred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(
                issuer: "mysite.com",
                audience: "mysite.com",
                notBefore: dt,
                expires: dt.AddDays(1),
                claims: claimsdata,
                signingCredentials: signInCred
                );
            var tokenstring = new JwtSecurityTokenHandler().WriteToken(token);
            return token;
        }

        public bool VerifyPassword(int Id, string Password)
        {
            var result = false;
            Users u = FindByCondition(x => x.PersonId == Id);
            if(u!=null)
            {
                if (Helper.VerifyHashedPassword(u.Password, Password) == "Success")
                {
                    result = true;
                }
            }
            return result;
        }

        public void ChangePassword(int Id, string NewPassword)
        {
            Users u = FindByCondition(x => x.PersonId == Id);
            if (u != null)
            {
                u.Password = Helper.Encrypt(NewPassword);
                Update(u);
                Save();
            }
        }
    }
}
