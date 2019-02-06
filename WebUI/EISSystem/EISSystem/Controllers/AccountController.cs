using EIS.Entities.Employee;
using EIS.Entities.OtherEntities;
using EIS.Entities.User;
using EIS.WebApp.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Net.Http;

namespace EIS.WebApp.Controllers
{
    [DisplayName("Account Management")]
    public class AccountController : BaseController<Users>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
    
        public AccountController(IEISService<Users> service, IHttpContextAccessor httpContextAccessor):base(service)
        {
            _httpContextAccessor = httpContextAccessor;
          
        }
        [AllowAnonymous]
        [DisplayName("Login")]
        [HttpGet]
        public IActionResult Login()
        {
            return View("Login");
        }
        [HttpPost]
        public IActionResult Login(Users user)
        {
            string EmployeeCode = "";
            HttpResponseMessage response = _service.PostResponse(ApiUrl+"/api/account/login", user ); 
            if (response.IsSuccessStatusCode == false)
            {
                ViewBag.CheckCreadentials = "<p style='color: red'>Please check username or password</p>";
                return View("Login");
            }
            else
            {
                string stringData = response.Content.ReadAsStringAsync().Result;
                PersonWithCookie CookiesData = JsonConvert.DeserializeObject<PersonWithCookie>(stringData);

                string CookieJson = JsonConvert.SerializeObject(CookiesData.Cookies);
                HttpContext.Session.SetString("CookieData", CookieJson);
                HttpContext.Session.SetString("IdCard", CookiesData.Person.EmployeeCode);
                HttpContext.Session.SetString("Name", CookiesData.Person.FirstName + " " + CookiesData.Person.LastName);
                HttpContext.Session.SetString("EmailId", CookiesData.Person.EmailAddress);
                string role = CookiesData.Cookies.Role;
                //Person person = JsonConvert.DeserializeObject<Person>(stringData);
                //Task<string> tokenResult = response.Content.ReadAsAsync<string>();
                ////pid = tokenResult.Result.ToString();
                //HttpContext.Session.SetString("EmployeeCode", person.EmployeeCode);
                //pid = person.Id.ToString();
                //EmployeeCode = person.EmployeeCode;
                //Response.Cookies.Append("IdCard", person.EmployeeCode);
                //Response.Cookies.Append("Name", person.FirstName+" "+person.LastName);
                //Response.Cookies.Append("EmailId", person.EmailAddress);
                //string role = Cache.GetStringValue("Role");
                if (role == "Admin")
                {
                    return RedirectToAction("AdminDashboard", "Dashboard");
                }else
                if (role == "Employee")
                {
                    return RedirectToAction("EmployeeDashboard", "Dashboard");
                }else
                if (role == "Manager")
                {
                    return RedirectToAction("ManagerDashboard", "Dashboard");
                }
            }
           


            return RedirectToAction("Profile","People",new { PersonId=EmployeeCode});
        }

        [DisplayName("Logout")]
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("CookieData");
            HttpContext.Session.Remove("IdCard");
            HttpContext.Session.Remove("Name");
            HttpContext.Session.Remove("EmailId");
            HttpResponseMessage response = _service.PostResponse(ApiUrl+"/api/account/logout",null );
            return RedirectToAction("Login","Account");
        }
        [DisplayName("Forgot Password")]
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(string username)
        { 
            HttpClient client = _service.GetService();
            HttpResponseMessage response = client.PostAsJsonAsync(ApiUrl+"/api/account/forgot/"+username+"",username).Result;
            if(response.IsSuccessStatusCode==true)
            { 
                ViewBag.Message = "Success! Password has been changed Successfully. Please check your email.";
            }
            else
            {
                ViewBag.Message = "Something went wrong!";
            }
            return View();
        }
        [DisplayName("Change Password")]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return PartialView();
        }
        [DisplayName("Access Denied Page")]
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [DisplayName("Error Page")]
        [HttpGet]
        public IActionResult ErrorPage(string Errordata,string StackTrace)
        {
            ViewBag.Error = Errordata;
            ViewBag.Trace = StackTrace;
            return View();
        }
    }
}
public class PersonWithCookie
{
    public virtual Person Person { get; set; }
    public virtual CookieModel Cookies { get; set; }
}