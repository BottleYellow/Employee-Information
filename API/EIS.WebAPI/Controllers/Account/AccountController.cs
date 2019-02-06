using EIS.Entities.Employee;
using EIS.Entities.OtherEntities;
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
using Newtonsoft.Json;
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
                    string b = Request.Headers[HeaderNames.UserAgent].ToString();
                    string ip = _accessor.HttpContext?.Connection?.RemoteIpAddress?.MapToIPv4()?.ToString();
                    Users newUser = _repository.Users.FindByUserName(user.UserName);
                    JwtSecurityToken token = _repository.Users.GenerateToken(newUser.Id);
                    string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
                    int pid = newUser.PersonId;
                    string role = "";
                    Person person = new Person();
                    CookieModel CookiesData = new CookieModel();
                    if (tokenValue != null)
                    {
                        person = _repository.Employee.FindByCondition(x => x.Id == pid);
                        role = _repository.Employee.GetDesignationById(person.RoleId).Name;
                        string data = _repository.Employee.GetDesignationById(person.RoleId).Access;
                        //Cache.SetStringValue("TokenValue", tokenValue);
                        //Cache.SetStringValue("PersonId", pid.ToString());
                        //Cache.SetStringValue("Access", data);
                        //Cache.SetStringValue("TenantId", person.TenantId.ToString());
                        //Cache.SetStringValue("EmployeeCode", person.EmployeeCode);
                        CookiesData = new CookieModel()
                        {
                            TokenValue = tokenValue,
                            PersonId = pid.ToString(),
                            Access = data,
                            TenantId = person.TenantId.ToString(),
                            EmployeeCode = person.EmployeeCode,
                            Role = role
                        };
                        //string CookieJson = JsonConvert.SerializeObject(Cookies);
                        //Response.Cookies.Append("CookieData", CookieJson);
                        //Response.Cookies.Append("TokenValue", tokenValue);
                        //Response.Cookies.Append("PersonId", pid.ToString());
                        //Response.Cookies.Append("Access", data);
                        //Response.Cookies.Append("TenantId", person.TenantId.ToString());
                        //Response.Cookies.Append("EmployeeCode", person.EmployeeCode);
                        //Response.Cookies.Append("Role", role);
                    }
                    PersonWithCookie pc = new PersonWithCookie()
                    {
                        Person = person,
                        Cookies = CookiesData
                    };
                    Personid = pid.ToString();
                    //Cache.SetStringValue("Role", role);
                    return Ok(pc);
                }
            }
            return NotFound("");

        }

        [DisplayName("Logout")]
        [HttpPost]
        [Route("logout")]
        public IActionResult Logout()
        {
            //Response.Cookies.Delete("CookieData");
            //Response.Cookies.Delete("PersonId");
            //Response.Cookies.Delete("TokenValue");
            //Response.Cookies.Delete("Access");
            //Response.Cookies.Delete("Role");
            //Response.Cookies.Delete("TenantId");
            //Response.Cookies.Delete("EmployeeCode");

            //Cache.DeleteStringValue("PersonId");
            //Cache.DeleteStringValue("TokenValue");
            //Cache.DeleteStringValue("Access");
            //Cache.DeleteStringValue("Role");
            return Ok("Successfully Logged out.");
        }


        [HttpGet("{id}")]
        public IActionResult GetLogin([FromRoute] int id)
        {
            Users login = _repository.Users.FindByCondition(x => x.Id == id);
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
            Users login = _repository.Users.FindByCondition(x => x.Id == id);
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
                "Your new password is : " + password+
                "\n"+ "Click here http://aclpune.com/ems to login";

            new EmailManager(_configuration).SendEmail(subject, body, To,null);
            Users user = _repository.Users.FindByUserName(username);
            user.Password = Helper.Encrypt(password);
            _repository.Users.UpdateAndSave(user);
            return Ok();
        }

        
    }
}
public class PersonWithCookie
{
    public virtual Person Person { get; set; }
    public virtual CookieModel Cookies { get; set; }
}