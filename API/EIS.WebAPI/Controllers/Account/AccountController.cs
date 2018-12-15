using System.Collections.Generic;
using EIS.Entities.User;
using EIS.Repositories.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Text;
using System;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using EIS.WebAPI.Filters;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net;
using Microsoft.Extensions.Caching.Distributed;

using EIS.WebAPI.Messanger;
using EIS.Repositories.Helpers;
using Microsoft.Extensions.Configuration;
using EIS.WebAPI.RedisCache;

namespace EIS.WebAPI.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : BaseController
    {
        public RedisAgent Cache;
        public readonly IConfiguration configuration;
        public AccountController(IRepositoryWrapper repository, IConfiguration configuration) : base(repository)
        {
            this.configuration = configuration;
            Cache = new RedisAgent();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public IActionResult Login(Users user)
        {
            string s = _repository.Users.ValidateUser(user);
            if (s == "success")
            {
                Users u = _repository.Users.FindByUserName(user.UserName);
                JwtSecurityToken token = _repository.Users.GenerateToken(u.Id);
                string s1 = new JwtSecurityTokenHandler().WriteToken(token);
                int pid = u.PersonId;
                string role = "";
                if (s1 != null)
                {

                    var options = new DistributedCacheEntryOptions();
                    options.SetAbsoluteExpiration(TimeSpan.FromDays(1));
                    role = _repository.RoleManager.GetRole(u.Id);
                    var data = _repository.RoleManager.FindByCondition(r => r.Name == role).Access;
                    Cache.SetStringValue("TokenValue", s1);
                    Cache.SetStringValue("PersonId", pid.ToString());
                    Cache.SetStringValue("Access", data);
                }
                Response.Cookies.Append("Role", role);
                Response.Headers.Add("Role", role);
                Cache.SetStringValue("Role", role);
                return Ok(pid.ToString());
            }

            else
            {
                return NotFound("");
            }

        }
        #region comment
        //[HttpPost("token")]
        //public IActionResult Token()
        //{
        //    var header = Request.Headers["Authorization"];
        //    if (header.ToString().StartsWith("Basic"))
        //    {
        //        var credValue = header.ToString().Substring("Basic ".Length).Trim();
        //        var usernameAndPassenc = Encoding.UTF8.GetString(Convert.FromBase64String(credValue));
        //        var usernameAndPass = usernameAndPassenc.Split(":");
        //        if (usernameAndPass[0] == "Admin" && usernameAndPass[1] == "pass")
        //        {
        //            var claimsdata = new[] { new Claim(ClaimTypes.Name, "username"), new Claim(ClaimTypes.Role, "Admin") };
        //            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("askjdkasdakjsdaksdasdjaksjdadfgdfgkjdda"));
        //            var signInCred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
        //            var token = new JwtSecurityToken(
        //                issuer: "mysite.com",
        //                audience: "mysite.com",
        //                expires: DateTime.Now.AddMinutes(1),
        //                claims: claimsdata,
        //                signingCredentials: signInCred
        //                );
        //            var tokenstring = new JwtSecurityTokenHandler().WriteToken(token);
        //            return Ok(tokenstring);
        //        }
        //    }
        //    return BadRequest("wrong request");
        //}
        #endregion

        [HttpPost]
        [Route("logout")]
        public IActionResult Logout()
        {
            Cache.DeleteStringValue("PersonId");
            Cache.DeleteStringValue("TokenValue");
            Cache.DeleteStringValue("Access");
            Cache.DeleteStringValue("Role");
            return Ok("Successfully Logged out.");
        }
        // GET: api/Logins/5
        [HttpGet("{id}")]
        public IActionResult GetLogin([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var login = _repository.Users.FindByCondition(x => x.Id == id);

            if (login == null)
            {
                return NotFound();
            }

            return Ok(login);
        }




        // DELETE: api/Logins/5
        [HttpDelete("{id}")]
        public IActionResult DeleteLogin([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var login = _repository.Users.FindByCondition(x => x.Id == id);
            if (login == null)
            {
                return NotFound();
            }
            _repository.Users.Delete(login);
            _repository.Users.Save();
            return Ok(login);
        }

        [HttpPost]
        [Route("forgot/{username}")]
        [AllowAnonymous]
        public IActionResult ForGot_Pass(String username)
        {
            string password = CreateRandomPassword(8);

            string To, UserID, Password, SMTPPort, Host;
            UserID = configuration["appSettings:UserID"];
            To = username;
            Password = configuration["appSettings:Password"];
            SMTPPort = configuration["appSettings:SMTPPort"];
            Host = configuration["appSettings:Host"];
            string subject = "New Password";
            //var password = ;
            string body = "Hello!" +"\n"+
                "Your new password is :" + password;

            EmailManager.SendEmail(UserID, subject, body, To, UserID, Password, SMTPPort, Host);
            var user = _repository.Users.FindByUserName(username);
            user.Password = Helper.Encrypt(password);
            _repository.Users.Save();
            return Ok();
        }

        public static string CreateRandomPassword(int PasswordLength)
        {
            string _allowedChars = "0123456789abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ";
            Random randNum = new Random();
            char[] chars = new char[PasswordLength];
            int allowedCharCount = _allowedChars.Length;
            for (int i = 0; i < PasswordLength; i++)
            {
                chars[i] = _allowedChars[(int)((_allowedChars.Length) * randNum.NextDouble())];
            }
            return new string(chars);
        }
    }
}