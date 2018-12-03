using EIS.Entities.Employee;
using EIS.Entities.User;
using EIS.WebApp.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;

using System.Net.Http;
using System.Threading.Tasks;

namespace EIS.WebApp.Controllers
{
    public class LoginController : Controller
    {
        static string imageBase64Data;
        public readonly IEISService service;
        public readonly IDistributedCache distributedCache;
        public LoginController(IEISService service,IDistributedCache distributedCache)
        {
            this.service = service;
            this.distributedCache = distributedCache;

        }
        [HttpGet]
        public IActionResult Login()
        {
            string token = distributedCache.GetString("TokenValue");
            int pid = Convert.ToInt32(distributedCache.GetString("PersonId"));
            if (token != null)
            {
                HttpClient client = service.GetService();
                HttpResponseMessage response = client.GetAsync("api/employee/" + pid + "").Result;
                string stringData = response.Content.ReadAsStringAsync().Result;
                Person data = JsonConvert.DeserializeObject<Person>(stringData);
                //imageBase64Data = Convert.ToBase64String(data.Image);
                //string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
                //ViewBag.ImageData = imageDataURL;
                ViewBag.Name = data.FirstName + " " + data.LastName;
                return RedirectToAction("Profile", "People", data);
            }
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