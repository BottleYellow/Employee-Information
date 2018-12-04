using EIS.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EIS.WebApp.IServices
{
    public interface IControllerService
    {
        IEnumerable<MvcControllerInfo> GetControllers();
    }
}
