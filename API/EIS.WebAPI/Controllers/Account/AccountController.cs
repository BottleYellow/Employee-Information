using EIS.Entities.Employee;
using EIS.Entities.User;
using EIS.Repositories.Helpers;
using EIS.Repositories.IRepository;
using EIS.WebAPI.Messanger;
using EIS.WebAPI.RedisCache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace EIS.WebAPI.Controllers
{
    [EnableCors("MyPolicy")]
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
                    Person person = _repository.Employee.FindByCondition(x => x.Id == pid);
                    role = _repository.Employee.GetDesignationById(person.DesignationId).Name;
                    var data = _repository.Employee.GetDesignationById(person.DesignationId).Access;
                    Cache.SetStringValue("TokenValue", s1);
                    Cache.SetStringValue("PersonId", pid.ToString());
                    Cache.SetStringValue("Access", data);
                }
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
        
        [HttpGet("VerifyPassword/{id}/{password}")]
        public IActionResult VerifyPasswordForChange([FromRoute]int id,[FromRoute]string password)
        {
            var result = _repository.Users.VerifyPassword(id, password);
            if (result == true)
            {
                return Ok(true);
            }
            else
            {
                return Ok(false);
            }
        }
        [HttpGet("ChangePassword/{id}/{password}")]
        public IActionResult ChangePassword([FromRoute]int id, [FromRoute]string password)
        {
            _repository.Users.ChangePassword(id, password);
            return Ok();
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
            string To = username;
            string subject = "New Password";
            //var password = ;
            string body = "Hello!" +"\n"+
                "Your new password is : " + password;

            new EmailManager(configuration).SendEmail(subject, body, To);
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