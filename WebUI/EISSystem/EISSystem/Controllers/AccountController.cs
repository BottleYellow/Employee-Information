﻿using EIS.Entities.Employee;
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
                ViewBag.Message = "<p style='color: red'>Please check username or password</p>";
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
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}