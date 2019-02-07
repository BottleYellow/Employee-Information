﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EIS.Entities.Employee;
using EIS.Entities.Generic;
using EIS.Entities.OtherEntities;
using EIS.WebApp.IServices;
using EIS.WebApp.Models;
using EIS.WebApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EIS.WebApp.Controllers
{
    public class BaseController<T> : Controller
    {
        public IEISService<T> _service;
        public static string ApiUrl;
        public BaseController(IEISService<T> service)
        {
            ApiUrl = MyHttpContext.APIBaseURL;
           _service = service;
          

        }
        public CookieModel GetSession()
        {
            CookieModel Cookies = new CookieModel();
            try
            {
                string val = HttpContext.Session.GetString("CookieData");
                Cookies = JsonConvert.DeserializeObject<CookieModel>(val);
            }
            catch (NullReferenceException)
            {
                Cookies = new CookieModel();
            }
            return Cookies;
        }
        [NonAction]
        public List<Locations> GetLocations()
        {
            HttpResponseMessage response = _service.GetResponse(ApiUrl + "/api/Location");
            string stringData = response.Content.ReadAsStringAsync().Result;
            List<Locations> locations = JsonConvert.DeserializeObject<List<Locations>>(stringData);
            return locations;
        }
        public IActionResult LoadData<T1>(string Url,bool? type,int? LocationId)
        {
            SortGrid sortEmployee = new SortGrid();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            sortEmployee.SortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            sortEmployee.SortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var search = Request.Form["search[value]"].FirstOrDefault();
            sortEmployee.Skip = start != null ? Convert.ToInt32(start) : 0;
            sortEmployee.PageSize = length != null ? Convert.ToInt32(length) : 0;
            sortEmployee.Search=string.IsNullOrEmpty(search)?null:search;
            sortEmployee.IsActive = type;
            sortEmployee.LocationId = LocationId;
            HttpClient client = _service.GetService();
            string stringData = JsonConvert.SerializeObject(sortEmployee);
            var contentData = new StringContent(stringData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(Url, contentData).Result;
            ArrayList arrayData = response.Content.ReadAsAsync<ArrayList>().Result;         
            int recordsTotal = JsonConvert.DeserializeObject<int>(arrayData[0].ToString());
            IList<T1> data = JsonConvert.DeserializeObject<IList<T1>>(arrayData[1].ToString());
            return Json(new { recordsFiltered = recordsTotal, recordsTotal, data = data });
        }    
    }
}