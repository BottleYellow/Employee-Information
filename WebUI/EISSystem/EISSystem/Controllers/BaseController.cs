using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using EIS.Entities.Employee;
using EIS.WebApp.IServices;
using EIS.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EIS.WebApp.Controllers
{
    public class BaseController<T> : Controller
    {
        public IEISService<T> pservice;
        public BaseController(IEISService<T> pservice)
        {
            this.pservice = pservice;
        }
        public ArrayList LoadData<T1>(string Url)
        {
            SortEmployee sortEmployee = new SortEmployee();
            // Skiping number of Rows count
            var start = Request.Form["start"].FirstOrDefault();
            // Paging Length 10,20
            var length = Request.Form["length"].FirstOrDefault();
            // Sort Column Name
            sortEmployee.SortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            // Sort Column Direction ( asc ,desc)
            sortEmployee.SortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            // Search Value from (Search box)
            var search = Request.Form["search[value]"].FirstOrDefault();
            //Paging Size (10,20,50,100)
            sortEmployee.Skip = start != null ? Convert.ToInt32(start) : 0;
            sortEmployee.PageSize = length != null ? Convert.ToInt32(length) : 0;
            //int recordsTotal = 0;
            sortEmployee.Search = search;
            HttpClient client = pservice.GetService();
            string stringData = JsonConvert.SerializeObject(sortEmployee);
            var contentData = new StringContent(stringData, Encoding.UTF8, "application/json");
             
            HttpResponseMessage response = client.PostAsync(Url, contentData).Result;

            ArrayList arrayData = response.Content.ReadAsAsync<ArrayList>().Result;
            return arrayData;
            //recordsTotal = JsonConvert.DeserializeObject<int>(arrayData[0].ToString());
            //IList<T1> data = JsonConvert.DeserializeObject<IList<T1>>(arrayData[1].ToString());
            //ViewModel<T1> viewModel = new ViewModel<T1>();
            //viewModel.TotalCount = recordsTotal;
            //viewModel.collection = data;
            //return viewModel;

            //return Json(new { recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = employees });
        }
    }
}