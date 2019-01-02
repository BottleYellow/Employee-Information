using EIS.WebApp.Models;
using System.Collections.Generic;

namespace EIS.WebApp.IServices
{
    public interface IControllerService
    {
        IEnumerable<MvcControllerInfo> GetControllers();
    }
}
