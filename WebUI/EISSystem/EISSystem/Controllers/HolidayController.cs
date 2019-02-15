﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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
    [DisplayName("Holidays")]
    public class HolidayController : BaseController<Holiday>
    {
        public HolidayController(IEISService<Holiday> service) : base(service)
        {

        }
        // GET: /<controller>/
        [DisplayName("List of Holidays")]
        public IActionResult Index()
        {
            ViewBag.Locations = GetLocations();
            return View();
        }

        [ActionName("Index")]
        [HttpPost]
        public IActionResult GetIndex(int id)
        {
            return LoadData<Holiday>(ApiUrl + "/api/Holiday/GetHolidays", null, id);
        }
        [DisplayName("Show Holidays")]
        public IActionResult EmployeeHolidays()
        {
            HttpResponseMessage response = _service.GetResponse(ApiUrl + "api/Holiday/"+GetSession().PersonId);
            string stringData = response.Content.ReadAsStringAsync().Result;
            List<Holiday> data = JsonConvert.DeserializeObject<List<Holiday>>(stringData);
            return View(data);
        }
        [DisplayName("Add Holiday")]
        public IActionResult AddHoliday()
        {
            ViewBag.Locations = GetLocations();
            Holiday holiday = new Holiday();
            return PartialView("AddHoliday", holiday);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddHoliday(Holiday holiday)
        {
            holiday.CreatedBy = Convert.ToInt32(GetSession().PersonId);
            ViewBag.Locations = GetLocations();
            holiday.CreatedDate = DateTime.Now.Date;
            holiday.UpdatedDate = DateTime.Now.Date;
            holiday.IsActive = true;
            HttpResponseMessage response = _service.PostResponse(ApiUrl+"/api/Holiday", holiday);
            string stringData = response.Content.ReadAsStringAsync().Result;

            if (response.IsSuccessStatusCode == true)
            {
                return View();
            }
            else
            {
                dynamic data = JObject.Parse(stringData);
                var Message = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
                string error = data.Date.ToString();
                error=error.Replace("[", null); error=error.Replace("]", null);

                ModelState.AddModelError("Date", error);
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            return PartialView("AddHoliday", holiday);
        }
    }
}
