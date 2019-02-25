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
            ViewBag.Locations = GetLocations();
            return View();
        }
        [ActionName("Index")]
        [HttpPost]
        public IActionResult GetLocations(bool type)
        {
            int LocationId = 0;
            return LoadData<Locations>(ApiUrl + "/api/Location/data", type, LocationId);

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
            location.CreatedDate = DateTime.Now;
            location.IsActive = true;
            location.CreatedBy = Convert.ToInt32(GetSession().PersonId);
            HttpResponseMessage response = _service.PostResponse(ApiUrl+"/api/Location/"+0, location);
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
          [DisplayName("Edit Location")]
        public IActionResult EditLocation(int LocationId)
        {
            Locations location = GetLocations().Where(x => x.Id == LocationId).FirstOrDefault();
            return PartialView("EditLocation", location);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditLocation(Locations location)
        {
            location.UpdatedDate = DateTime.Now;
            location.IsActive = true;
            location.UpdatedBy = Convert.ToInt32(GetSession().PersonId);
            HttpResponseMessage response = _service.PostResponse(ApiUrl+"/api/Location/"+location.Id, location);
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
            return PartialView("EditLocation", location);
        }
        [DisplayName("Delete Location")]
        public void Delete(int id)
        {
            HttpResponseMessage response = _service.PostResponse(ApiUrl + "/api/Location/DeleteLocation/" + id, null);
            if (response != null)
            {
                
            }

        }
        [DisplayName("Activate Location")]
        public IActionResult ActivateLocation(int id)
        {
            HttpResponseMessage response = _service.GetResponse(ApiUrl + "/api/Location/ActivateLocation/" + id + "");
            if (response.IsSuccessStatusCode)
                ViewBag.Message = "Location activated successfully!";
            return RedirectToAction("Index", "Location");
        }
    }
}
