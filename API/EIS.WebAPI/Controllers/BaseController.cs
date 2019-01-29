using EIS.Entities.OtherEntities;
using EIS.Repositories.IRepository;
using EIS.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;

namespace EIS.WebAPI.Controllers
{
    public class BaseController : ControllerBase
    {
        protected RedisAgent Cache;
        internal int TenantId = 0;
        protected readonly IRepositoryWrapper _repository;
        public BaseController(IRepositoryWrapper repository)
        {
            _repository = repository;
            Cache = new RedisAgent();
            TenantId = Convert.ToInt32(Cache.GetStringValue("TenantId"));
            //try
            //{
            //    Request.Cookies.TryGetValue("CookieData", out string val);
            //    var Cookies = JsonConvert.DeserializeObject<CookieModel>(val);
            //    TenantId = Convert.ToInt32(Cookies.TenantId);
            //}
            //catch (NullReferenceException)
            //{
            //    TenantId = 0;
            //}
        }

        public string Personid = "";
    }
}