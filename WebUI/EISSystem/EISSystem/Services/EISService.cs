using EIS.WebApp.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace EIS.WebApp.Services
{
    public class EISService : ControllerBase,IEISService
    {
        IDistributedCache distributedCache;
        public EISService(IDistributedCache distributedCache)
        {
            this.distributedCache = distributedCache;
        }
        public HttpResponseMessage GetResponse(string url)
        {
            HttpClient client = GetService();
            HttpResponseMessage response = client.GetAsync(url).Result;
            return response;
        }

        public HttpClient GetService()
        {

            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:54830")
            };
            
            MediaTypeWithQualityHeaderValue contentType = new MediaTypeWithQualityHeaderValue("application/json");
            string ActionName = distributedCache.GetString("Action");
            string ControllerName = distributedCache.GetString("Controller");
            client.DefaultRequestHeaders.Add("Action", ActionName);
            client.DefaultRequestHeaders.Add("Controller", ControllerName);
            client.DefaultRequestHeaders.Accept.Add(contentType);
            return client;
        }

        public HttpResponseMessage PostResponse(string url,HttpContent content)
        {
            HttpClient client = GetService();
            HttpResponseMessage response = client.PostAsJsonAsync(url, content).Result;
            return response;
        }
    }
}
