using EIS.Entities.User;
using EIS.WebApp.IServices;
using EIS.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.ComponentModel;

namespace EIS.WebApp.Controllers
{
    [DisplayName("Account Management")]
    public class AccountController : Controller
    {
        public readonly IEISService<Users> _service;
        RedisAgent Cache;
        public AccountController(IEISService<Users> service)
        {
            _service = service;
            Cache = new RedisAgent();
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
                Task<string> tokenResult = response.Content.ReadAsAsync<string>();
                pid = tokenResult.Result.ToString();
                string role = Cache.GetStringValue("Role");
                if (role == "Admin")
                {
                    return RedirectToAction("Index", "People");
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
    }
}