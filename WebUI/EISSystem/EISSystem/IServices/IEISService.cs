using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace EIS.WebApp.IServices
{
    public interface IEISService
    {
        HttpClient GetService();
        HttpResponseMessage GetResponse(string url);

        HttpResponseMessage PostResponse(string url,HttpContent content);
    }
}
