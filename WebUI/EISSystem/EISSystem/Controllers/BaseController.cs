using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EIS.Entities.Employee;
using EIS.WebApp.IServices;
using EIS.WebApp.Models;
using EIS.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EIS.WebApp.Controllers
{
    public class BaseController<T> : Controller
    {
        public IEISService<T> _service;
        public RedisAgent Cache;
        public BaseController(IEISService<T> service)
        {
           _service = service;
            Cache = new RedisAgent();
        }
        public ArrayList LoadData<T1>(string Url)
        {
            SortEmployee sortEmployee = new SortEmployee();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            sortEmployee.SortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            sortEmployee.SortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var search = Request.Form["search[value]"].FirstOrDefault();
            sortEmployee.Skip = start != null ? Convert.ToInt32(start) : 0;
            sortEmployee.PageSize = length != null ? Convert.ToInt32(length) : 0;
            sortEmployee.Search=string.IsNullOrEmpty(search)?null:search;          
            HttpClient client = _service.GetService();
            string stringData = JsonConvert.SerializeObject(sortEmployee);
            var contentData = new StringContent(stringData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(Url, contentData).Result;
            ArrayList arrayData = response.Content.ReadAsAsync<ArrayList>().Result;
            return arrayData;
            
        }
    }
}