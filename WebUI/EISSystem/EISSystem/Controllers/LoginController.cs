using EIS.Entities.User;
using EIS.WebApp.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
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
            HttpResponseMessage response = client.GetAsync("api/Users").Result;
            string stringData = response.Content.ReadAsStringAsync().Result;
            List<Users> data = JsonConvert.DeserializeObject<List<Users>>(stringData);
            Users users = data.Find(p => p.UserName == user.UserName && p.Password==user.Password && p.Role==user.Role);
            if(users != null)
            {
                HttpContext.Session.SetInt32("Id", users.PersonId);
                return RedirectToAction("Details", "People", new { id = users.PersonId });
            }
            return View();
        }
    }
}