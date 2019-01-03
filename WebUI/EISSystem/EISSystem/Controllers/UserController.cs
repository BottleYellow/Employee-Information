using EIS.Entities.Employee;
using EIS.Entities.User;
using EIS.WebApp.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EIS.WebApp.Controllers
{

    [DisplayName("User Management")]
    public class UserController : Controller
    {
        public readonly IEISService<Users> service;
        public static List<Users> Users;
        public static List<Person> Persons;
        public UserController(IEISService<Users> service)
        {
            this.service = service;
            HttpResponseMessage response = service.GetResponse("api/User");
            string stringData = response.Content.ReadAsStringAsync().Result;
            Users = JsonConvert.DeserializeObject<List<Users>>(stringData);

            response = service.GetResponse("api/Employee");
            string stringData2 = response.Content.ReadAsStringAsync().Result;
            Persons = JsonConvert.DeserializeObject<List<Person>>(stringData2);
            
            foreach (var p in Users)
            {
                foreach (var d in Persons)
                {
                    if (p.PersonId == d.Id)
                        p.Person = d;
                }
            }
           
        }
        [DisplayName("List Of Users")]
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View(Users);
        }
    }
}
