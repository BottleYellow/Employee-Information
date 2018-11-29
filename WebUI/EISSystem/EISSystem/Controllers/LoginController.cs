using EIS.Entities.User;
using EIS.WebApp.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace EIS.WebApp.Controllers
{
    public class LoginController : Controller
    {
        public readonly IEISService service;
        public LoginController(IEISService service)
        {
            this.service = service;

        }
        public IActionResult Index()
        {

            return View("Login");
        }
        [HttpPost]
        public IActionResult Login(Users user)
        {
            HttpClient client = service.GetService();
            HttpResponseMessage response = client.PostAsJsonAsync("api/account/login",user).Result;
            if (response.IsSuccessStatusCode == false)
            {
                TempData["Message"] = "<script>swal('','Access Denied','error');</script>";

                return View("Login");
            }
            else
            {
                TempData["Message"] = "<script>swal('Good job!', 'You are login successfully', 'success').then(function(){window.location.href='../People/Index';});</script>";
                return View("Login");
            } 
        }
    }
}