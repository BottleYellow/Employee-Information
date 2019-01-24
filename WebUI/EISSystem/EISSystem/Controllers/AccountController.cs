using EIS.Entities.User;
using EIS.WebApp.IServices;
using EIS.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.ComponentModel;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using EIS.Entities.Employee;

namespace EIS.WebApp.Controllers
{
    [DisplayName("Account Management")]
    public class AccountController : BaseController<Users>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
    
        public AccountController(IEISService<Users> service, IHttpContextAccessor httpContextAccessor):base(service)
        {
            _httpContextAccessor = httpContextAccessor;
          
        }
        [DisplayName("Login")]
        [HttpGet]
        public IActionResult Login()
        {
            return View("Login");
        }
        [HttpPost]
        public IActionResult Login(Users user)
        {
            string pid = "";
            HttpResponseMessage response = _service.PostResponse("api/account/login", user); 
            if (response.IsSuccessStatusCode == false)
            {
                ViewBag.CheckCreadentials = "<p style='color: red'>Please check username or password</p>";
                return View("Login");
            }
            else
            {
                string stringData = response.Content.ReadAsStringAsync().Result;
                var person = JsonConvert.DeserializeObject<Person>(stringData);
                Task<string> tokenResult = response.Content.ReadAsAsync<string>();
                //pid = tokenResult.Result.ToString();
                pid = person.Id.ToString();
                Response.Cookies.Append("IdCard", person.EmployeeCode);
                Response.Cookies.Append("Name", person.FirstName+" "+person.LastName);
                Response.Cookies.Append("EmailId", person.EmailAddress);
                string role = Cache.GetStringValue("Role");
                if (role == "Admin")
                {
                    return RedirectToAction("AdminDashboard", "Dashboard");
                }
            }
            
            return RedirectToAction("Profile","People",new { PersonId=Convert.ToInt32(pid)});
        }

        [DisplayName("Logout")]
        [HttpGet]
        public IActionResult LogOut()
        {
            HttpResponseMessage response = _service.PostResponse("api/account/logout",null);
            return View("Login");
        }
        [DisplayName("Forgot Password")]
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(string username)
        { 
            HttpClient client = _service.GetService();
            HttpResponseMessage response = client.PostAsJsonAsync("api/account/forgot/"+username+"",username).Result;
            if(response.IsSuccessStatusCode==true)
            { 
                ViewBag.Message = "Success! Password has been changed Successfully. Please check your email.";
            }
            else
            {
                ViewBag.Message = "Something went wrong!";
            }
            return View();
        }
        [DisplayName("Change Password")]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return PartialView();
        }
        [DisplayName("Access Denied Page")]
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [DisplayName("Error Page")]
        [HttpGet]
        public IActionResult ErrorPage()
        {
            return View();
        }
    }
}