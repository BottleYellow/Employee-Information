using EIS.Entities.Employee;
using EIS.Entities.User;
using EIS.Repositories.Helpers;
using EIS.Repositories.IRepository;
using EIS.WebAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;

namespace EIS.WebAPI.Controllers
{
    [DisplayName("Account Management")]
    [EnableCors("MyPolicy")]
    [Route("api/account")]
    [ApiController]
    public class AccountController : BaseController
    {
        private IHttpContextAccessor _accessor;
        private readonly IConfiguration _configuration;
        public AccountController(IHttpContextAccessor accessor,IRepositoryWrapper repository, IConfiguration configuration) : base(repository)
        {
            _accessor = accessor;
            _configuration = configuration;
        }

        [DisplayName("Login")]
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login(Users user)
        {
            if (!(string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Password)))
            {

                string status = _repository.Users.ValidateUser(user);
                if (status == "success")
                {
                    var b = Request.Headers[HeaderNames.UserAgent].ToString();
                    var ip = _accessor.HttpContext?.Connection?.RemoteIpAddress?.MapToIPv4()?.ToString();
                    Users newUser = _repository.Users.FindByUserName(user.UserName);
                    JwtSecurityToken token = _repository.Users.GenerateToken(newUser.Id);
                    string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
                    int pid = newUser.PersonId;
                    string role = "";
                    Person person = new Person();
                    if (tokenValue != null)
                    {
                        person = _repository.Employee.FindByCondition(x => x.Id == pid);
                        role = _repository.Employee.GetDesignationById(person.RoleId).Name;
                        var data = _repository.Employee.GetDesignationById(person.RoleId).Access;
                        Cache.SetStringValue("TokenValue", tokenValue);
                        Cache.SetStringValue("PersonId", pid.ToString());
                        Cache.SetStringValue("Access", data);
                        Cache.SetStringValue("TenantId", person.TenantId.ToString());
                        Cache.SetStringValue("EmployeeCode", person.EmployeeCode);
                    }
                    Cache.SetStringValue("Role", role);
                    return Ok(person);
                }
            }
            return NotFound("");

        }

        [DisplayName("Logout")]
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


        [HttpGet("{id}")]
        public IActionResult GetLogin([FromRoute] int id)
        {
            var login = _repository.Users.FindByCondition(x => x.Id == id);
            if (login == null)
            {
                return NotFound();
            }
            return Ok(login);
        }
        
        [AllowAnonymous]
        [HttpGet("VerifyPassword/{id}/{password}")]
        public IActionResult VerifyPasswordForChange([FromRoute]int id,[FromRoute]string password)
        {
            bool result=_repository.Users.VerifyPassword(id, password);
            return Ok(result);
        }

        [DisplayName("Change Password")]
        [AllowAnonymous]
        [HttpGet("ChangePassword/{id}/{password}")]
        public IActionResult ChangePassword([FromRoute]int id, [FromRoute]string password)
        {
            _repository.Users.ChangePasswordAndSave(id, password);
            return Ok();
        }

        
        [HttpDelete("{id}")]
        public IActionResult DeleteLogin([FromRoute] int id)
        {
            var login = _repository.Users.FindByCondition(x => x.Id == id);
            if (login == null)
            {
                return NotFound();
            }
            _repository.Users.DeleteAndSave(login);            
            return Ok(login);
        }

        [DisplayName("Forgot Password")]
        [HttpPost("forgot/{username}")]
        [AllowAnonymous]
        public IActionResult ForGot_Pass(string username)
        {
            string password = EmailManager.CreateRandomPassword(8);
            string To = username;
            string subject = "New Password";
            string body = "Hello!" +"\n"+
                "Your new password is : " + password;

            new EmailManager(_configuration).SendEmail(subject, body, To);
            var user = _repository.Users.FindByUserName(username);
            user.Password = Helper.Encrypt(password);
            _repository.Users.UpdateAndSave(user);
            return Ok();
        }

        
    }
}