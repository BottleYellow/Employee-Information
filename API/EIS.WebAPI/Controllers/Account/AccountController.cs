<<<<<<< HEAD
﻿using EIS.Entities.User;
using EIS.Repositories.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.IdentityModel.Tokens.Jwt;
using EIS.WebAPI.Messanger;
using EIS.Repositories.Helpers;
using Microsoft.Extensions.Configuration;
using EIS.WebAPI.RedisCache;
using Microsoft.AspNetCore.Cors;
using EIS.Entities.Employee;
=======
﻿using EIS.Entities.Employee;
using EIS.Entities.User;
using EIS.Repositories.Helpers;
using EIS.Repositories.IRepository;
using EIS.WebAPI.RedisCache;
using EIS.WebAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.IdentityModel.Tokens.Jwt;
>>>>>>> eab0133b5e8f6e86eb09bb18611280e9b8dcee1c

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

        #region Account Management
        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public IActionResult Login(Users user)
        {
            string checkUserStatus = _repository.Users.ValidateUser(user);
            if (checkUserStatus == "success")
            {
                Users newUser = _repository.Users.FindByUserName(user.UserName);
                JwtSecurityToken token = _repository.Users.GenerateToken(newUser.Id);
                string s1 = new JwtSecurityTokenHandler().WriteToken(token);
                int pid = newUser.PersonId;
                string role = "";
                if (s1 != null)
                {
                    Person person = _repository.Employee.FindByCondition(x => x.Id == pid);
                    role = _repository.Employee.GetDesignationById(person.RoleId).Name;
                    var data = _repository.Employee.GetDesignationById(person.RoleId).Access;
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
<<<<<<< HEAD

=======
>>>>>>> eab0133b5e8f6e86eb09bb18611280e9b8dcee1c
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
<<<<<<< HEAD
=======
        // GET: api/Logins/5
        [HttpGet("{id}")]
        public IActionResult GetLogin([FromRoute] int id)
        {
            var login = _repository.Users.FindByCondition(x => x.Id == id);
>>>>>>> eab0133b5e8f6e86eb09bb18611280e9b8dcee1c

        [HttpGet("VerifyPassword/{id}/{password}")]
        public IActionResult VerifyPasswordForChange([FromRoute]int id,[FromRoute]string password)
        {
            var result = _repository.Users.VerifyPassword(id, password);
            var boolResult = false;
            if (result == true)
                boolResult = true;
            return Ok(boolResult);
        }
        [HttpGet("ChangePassword/{id}/{password}")]
        public IActionResult ChangePassword([FromRoute]int id, [FromRoute]string password)
        {
            _repository.Users.ChangePassword(id, password);
            return Ok();
        }

<<<<<<< HEAD
=======

        // DELETE: api/Logins/5
        [HttpDelete("{id}")]
        public IActionResult DeleteLogin([FromRoute] int id)
        {
            var login = _repository.Users.FindByCondition(x => x.Id == id);
            if (login == null)
            {
                return NotFound();
            }
            _repository.Users.Delete(login);
            _repository.Users.Save();
            return Ok(login);
        }

>>>>>>> eab0133b5e8f6e86eb09bb18611280e9b8dcee1c
        [HttpPost]
        [Route("forgot/{username}")]
        [AllowAnonymous]
        public IActionResult ForGot_Pass(String username)
        {
            string password = CreateRandomPassword(8);
            string To = username;
            string subject = "New Password";
            string body = "Hello!" + "\n" +
                "Your new password is : " + password;
            new EmailManager(configuration).SendEmail(subject, body, To);
            var user = _repository.Users.FindByUserName(username);
            user.Password = Helper.Encrypt(password);
            _repository.Users.UpdateAndSave(user);
            return Ok();
        }
        #endregion

        #region Methods
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
        #endregion
    }
}