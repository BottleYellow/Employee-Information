﻿using EIS.Data.Context;
using EIS.Entities.Employee;
using EIS.Entities.SP;
using EIS.Entities.User;
using EIS.Repositories.Helpers;
using EIS.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
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
        public Users FindByUserName(string Username)
        {
            var user = _dbContext.Users.Where(u => u.UserName == Username).FirstOrDefault();
            return user;
        }
        public string ValidateUser(Users user)
        {
            string result = "Failed";
            Users u = FindByUserName(user.UserName);
            if (u != null && u.IsActive==true)
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

        public void ChangePasswordAndSave(int Id, string NewPassword)
        {
            Users u = FindByCondition(x => x.PersonId == Id);
            if (u != null)
            {
                u.UpdatedBy = Id;
                u.Password = Helper.Encrypt(NewPassword);
                Update(u);
                Save();
            }
        }

        public void CreateUserAndSave(Users users)
        {
            CreateUser(users);
            Save();
        }

        public MailConfiguration GetMailConfiguration()
        {
            return _dbContext._sp_MailConfigurations.FromSql("LMS.usp_MailConfiguration").FirstOrDefault();
        }
    }
}
