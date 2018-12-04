using EIS.Entities.User;
using EIS.WebApp.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EIS.WebApp.Controllers
{
    public class LoginController : Controller
    {
        public readonly IEISService service;
        public LoginController(IEISService service)
        {
            this.service = service;

        }
        [HttpGet]
        public IActionResult Login()
        {
            return View("Login");
        }
        [HttpPost]
        public IActionResult Login(Users user)
        {
            HttpClient client = service.GetService();
            HttpResponseMessage response = client.PostAsJsonAsync("api/account/login", user).Result;
            if (response.IsSuccessStatusCode == false)
            {
                TempData["Message"] = "<p style='color: orange'>Please check username or password</p>";
                return View("Login");
            }
            else
            {
                
            }
            return RedirectToAction("Index","People");
        }

        [HttpPost]
        public IActionResult LogOut(int id)
        { 
            HttpClient client = service.GetService();
            HttpResponseMessage response = client.PostAsJsonAsync("api/account/logout",id).Result;
            return View("Login");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(string username)
        { 
            HttpClient client = service.GetService();
            HttpResponseMessage response = client.PostAsJsonAsync("api/account/forgot/"+username+"",username).Result;
            if(response.IsSuccessStatusCode==true)
            {
                TempData["Msg"] ="<script>alert('Your password has been reset successfully. Your new password has been sent to your primary email address.');</script>";
                return RedirectToAction("ForgotPassword");
            }
            else
            {
                TempData["Msg"] = "<script>alert('Something went wrong!')</script>";
            }
            return View("Login");
        }
    }
}