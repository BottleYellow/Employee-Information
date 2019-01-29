using EIS.Repositories.IRepository;
using EIS.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;
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
        }

        public string Personid = "";
    }
}