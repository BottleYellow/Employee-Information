using EIS.Entities.Employee;
using EIS.Entities.User;
using EIS.Repositories.IRepository;
using EIS.WebApp.IServices;
using EIS.WebApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace EIS.WebApp.Controllers
{
    public class AccountController : Controller
    {
        RedisAgent Cache;
        static string imageBase64Data;
        public readonly IEISService<Users> service;
        public readonly IDistributedCache distributedCache;
        public AccountController(IEISService<Users> service,IDistributedCache distributedCache)
        {
            this.service = service;
            this.distributedCache = distributedCache;
            Cache = new RedisAgent();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View("Login");
        }
        [HttpPost]
        public IActionResult Login(Users user)
        {
            string pid = "";
            HttpResponseMessage response = service.PostResponse("api/account/login", user);
            
            if (response.IsSuccessStatusCode == false)
            {
                TempData["Message"] = "<p style='color: orange'>Please check username or password</p>";
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
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}