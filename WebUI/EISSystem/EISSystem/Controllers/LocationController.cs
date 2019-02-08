using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EIS.Entities.Employee;
using EIS.Entities.Hoildays;
using EIS.WebApp.Filters;
using EIS.WebApp.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EIS.WebApp.Controllers
{
    [SessionTimeOut]
    [DisplayName("Locations")]
    public class LocationController : BaseController<Locations>
    {
        public LocationController(IEISService<Locations> service) : base(service)
        {

        }
        // GET: /<controller>/
        [DisplayName("Manage Locations")]
        public IActionResult Index()
        {
            HttpResponseMessage response = _service.GetResponse(ApiUrl+"/api/Location");
            string stringData = response.Content.ReadAsStringAsync().Result;
            List<Locations> data = JsonConvert.DeserializeObject<List<Locations>>(stringData);
            return View(data);
        }
        [DisplayName("Add Location")]
        public IActionResult AddLocation()
        {
            Locations location = new Locations();
            return PartialView("AddLocation", location);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddLocation(Locations location)
        {
            location.CreatedDate = DateTime.Now.Date;
            location.UpdatedDate = DateTime.Now.Date;
            location.IsActive = true;
            HttpResponseMessage response = _service.PostResponse(ApiUrl+"/api/Location", location);
            string stringData = response.Content.ReadAsStringAsync().Result;

            if (response.IsSuccessStatusCode == true)
            {
                return View();
            }
            else
            {
                dynamic data = JObject.Parse(stringData);
                var Message = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
                string error = data.LocationName.ToString();
                error=error.Replace("[", null); error=error.Replace("]", null);

                ModelState.AddModelError("Date", error);
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            return PartialView("AddLocation", location);
        }
    }
}
