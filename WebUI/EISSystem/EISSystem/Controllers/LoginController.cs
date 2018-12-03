using EIS.Entities.User;
using EIS.WebApp.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
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
                Task<AccessToken> tokenResult = response.Content.ReadAsAsync<AccessToken>();
                string token = tokenResult.Result.TokenName;
                if (token != null)
                {
                    HttpContext.Session.SetString("token", token);
                }
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
        public IActionResult ForGot_Pass()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForGot_Pass(Users users)
        {
            return View();
        }
    }
}