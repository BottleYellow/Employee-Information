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
            //if (token != null)
            //{
            //    string hostName = Dns.GetHostName(); // Retrive the Name of HOST  
            //    var addresses = Dns.GetHostAddresses(hostName);
            //    string myIP = addresses[1].ToString();
            //    //string myIP = Dns.GetHostEntry(hostName).AddressList[2].ToString();
            //    AccessToken accessToken = new AccessToken();    
            //    accessToken.TokenName = tokenstring;
            //    accessToken.DeviceName = Environment.MachineName;
            //    accessToken.IPAddress = myIP;
            //    accessToken.IssuedAt = token.ValidFrom;
            //    accessToken.Expiry = token.ValidTo;
            //    accessToken.UserId = UserId;
            //    dbContext.Tokens.Add(accessToken);
            //    dbContext.SaveChanges();
            //}
            return token;
        }
        public static string GetBoardMaker()
        {

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem");

            foreach (ManagementObject wmi in searcher.Get())
            {
                try
                {
                    return wmi.GetPropertyValue("Manufacturer").ToString()+" "+wmi.GetPropertyValue("Model").ToString()+" "+ wmi.GetPropertyValue("System Type").ToString();
                }

                catch { }

            }

            return "Board Maker: Unknown";

        }
        public string GetToken(int id)
        {
            string token = null;
            if(isTokenExists(id))
            {
                token = dbContext.Tokens.Where(t => t.UserId == id).Select(t => t.TokenName).FirstOrDefault();
                return token;
            }
            return null;
        }

        public bool isTokenExists(int id)
        {
            string token = dbContext.Tokens.Where(t => t.UserId == id).Select(t => t.TokenName).FirstOrDefault();
            if(token!=null)
            {
                return true;
            }
            return false;
        }

        public bool isTokenExpired(int id)
        {
            DateTime d = dbContext.Tokens.Where(t => t.UserId == id).Select(t => t.Expiry).FirstOrDefault();
            if (d > DateTime.Now)
            {
                return false;
            }
            return true;
        }

        public bool CheckIfFromSameIpAddress(int id, string address)
        {
            string add = dbContext.Tokens.Where(t => t.UserId == id).Select(t => t.IPAddress).FirstOrDefault();
            if (add == address)
            {
                return true;
            }
            return false;
        }

        public bool IsValidToken(string token, out Exception ex)
        {
            Exception e=null;
            bool result=false;
            string uid = dbContext.Tokens.Where(t => t.TokenName == token).Select(t => t.UserId).FirstOrDefault().ToString();
            if (uid != "0")
            {
                int id = Convert.ToInt32(uid);
                string tk = GetToken(id);
                if (tk == token)
                {
                    if (!isTokenExpired(id))
                    {
                        string add = dbContext.Tokens.Where(t => t.UserId == id).Select(t => t.IPAddress).FirstOrDefault();
                        if (CheckIfFromSameIpAddress(id, add))
                        {
                            e = null;
                            result= true;
                        }
                    }
                    else
                    {
                        e = new SecurityTokenExpiredException("Token is Expired");
                        result= false;
                    }
                }
                else
                {
                    e = new SecurityTokenInvalidSignatureException("Signature of Token is not valid");
                    result= false;
                }
            }
            else
            {
                e = new UnauthorizedAccessException("Access Denied");
                result= false;
            }
            ex = e;
            return result;
        }


    }
}
