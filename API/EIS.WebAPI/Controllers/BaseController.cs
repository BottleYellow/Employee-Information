using EIS.Repositories.IRepository;
using EIS.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace EIS.WebAPI.Controllers
{
    public class BaseController : ControllerBase
    {

        internal int TenantId = 0;
        protected readonly IRepositoryWrapper _repository;
        public BaseController(IRepositoryWrapper repository)
        {
            _repository = repository;

            TenantId = 1;
        }

        public string Personid = "";
    }
}