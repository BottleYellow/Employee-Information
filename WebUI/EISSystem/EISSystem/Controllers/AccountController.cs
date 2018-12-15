using EIS.Entities.Employee;
using EIS.Entities.User;
using EIS.WebApp.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;

using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EIS.WebApp.Controllers
{
    public class AccountController : Controller
    {
        public static string imageBase64Data;
        public readonly IEISService service;
        public readonly IDistributedCache distributedCache;
        public AccountController(IEISService service,IDistributedCache distributedCache)
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
                return RedirectToAction("Index", "People", data);
            }
            return View("Login");
        }
        [HttpPost]
        public IActionResult Login(Users user)
        {
            string pid = "";
            HttpClient client = service.GetService();
            HttpResponseMessage response = client.PostAsJsonAsync("api/account/login", user).Result;
            if (response.IsSuccessStatusCode == false)
            {
                ViewBag.Message = "<p style='color: red'>Please check username or password</p>";
                return View("Login");
            }
            else
            {
                Task<string> tokenResult = response.Content.ReadAsAsync<string>();
                pid = tokenResult.Result.ToString();
            }
            return RedirectToAction("Index","People",new { id=Convert.ToInt32(pid)});
        }
        [HttpGet]
        public IActionResult LogOut()
        { 
            HttpResponseMessage response = service.PostResponse("api/account/logout",null);
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
                ViewBag.Message = "success";
            }
            else
            {
                ViewBag.Message = "error";
            }
            return View("Login");
        }
    }
}