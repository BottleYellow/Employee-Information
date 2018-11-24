using EIS.Data.Context;
using EIS.Entities.User;
using EIS.Repositories.Helpers;
using EIS.Repositories.IRepository;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;

namespace EIS.Repositories.Repository
{
    public class UserRepository : RepositoryBase<Users>, IUserRepository
    {
        ApplicationDbContext dbContext;
        public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
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
            var user = dbContext.Users.Where(u => u.UserName == Username).FirstOrDefault();
            return user;
        }

        public string ValidateUser(Users user)
        {
            string result = "Failed";
            Users u = FindByUserName(user.UserName);
            if (u != null)
            {
                string hp = dbContext.Users.Where(u1 => u1.UserName == u.UserName).Select(u1 => u1.Password).FirstOrDefault();
                if (Helper.VerifyHashedPassword(hp, user.Password) == "Success")
                {
                    result = "success";
                }
            }
            return result;
            
        }
    }
}
