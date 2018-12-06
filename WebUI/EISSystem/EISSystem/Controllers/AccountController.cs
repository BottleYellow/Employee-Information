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
        static string imageBase64Data;
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
                return RedirectToAction("Profile", "People", data);
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
                TempData["Message"] = "<p style='color: orange'>Please check username or password</p>";
                return View("Login");
            }
            else
            {
                Task<string> tokenResult = response.Content.ReadAsAsync<string>();
                pid = tokenResult.Result.ToString();
            }
            return RedirectToAction("Profile","People",new { id=Convert.ToInt32(pid)});
        }

        public IActionResult LogOut()
        { 
            HttpResponseMessage response = service.PostResponse("api/account/logout",null);
            return RedirectToAction("login");
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
                //TempData["Msg"] = "<div class='alert alert-success alert-dismissible col-md-8' style='position:absolute; margin:auto; top: 0; right: 0; left: 0; border-radius: 3px; height:50px'>Your password has been reset successfully. Your new password has been sent to your primary email address.</div>";

                TempData["Msg"] = "" +
                    "<div id='myAlert' class='alert alert-success col-md-9' style='position:absolute; margin:auto; top: 0; right: 0; left: 0; border-radius: 3px; height:50px'>" +
                    "<a href='#' class='close' data-dismiss='alert'>&times;</a>" +
                    "<strong>Your password has been reset successfully. Your new password has been sent to your primary email address.</strong></div>" +
                    "<script>" +
                        "$(document).ready(function(){" +
                            "setTimeout(function(){" +
                                "$('#myAlert').hide('fade');" +
                            "},2000);" +
                        "});" +
                    "</script>";
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