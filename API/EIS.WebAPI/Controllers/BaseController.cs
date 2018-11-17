using EIS.Repositories.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace EIS.WebAPI.Controllers
{
    public class BaseController : ControllerBase
    {
        public readonly IRepositoryWrapper _repository;
        public BaseController(IRepositoryWrapper repository)
        {
            _repository = repository;
        }
    }
}