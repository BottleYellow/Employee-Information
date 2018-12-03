using EIS.Entities.Employee;
using EIS.Entities.User;
using EIS.WebApp.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Net;
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
        public IActionResult Index()
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
            HttpResponseMessage response = client.PostAsJsonAsync("api/account/login",user).Result;
            Task<AccessToken> tokenResult = response.Content.ReadAsAsync<AccessToken>();
            string token = tokenResult.Result.TokenName;
            int pid = 0;
            if (token != null)
            {
                pid = tokenResult.Result.UserId;
            }
            if (response.IsSuccessStatusCode == false)
            {
                TempData["Message"] = "<script>swal('','Access Denied','error');</script>";

                return View("Login");
            }
            else
            {
                TempData["Message"] = "<script>swal('Good job!', 'You are login successfully', 'success');</script>";
                return RedirectToAction("Profile", "People", new { id = pid });
            } 
            //.then(function(){ window.location.href = '../People/Index'; })
        }
    }
}