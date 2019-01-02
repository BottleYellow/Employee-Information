using EIS.WebApp.IServices;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace EIS.WebApp.Services
{
    public class EISService<T> : IEISService<T> where T: class
    {
        IDistributedCache distributedCache;
        public EISService(IDistributedCache distributedCache)
        {
            this.distributedCache = distributedCache;
        }

        public HttpResponseMessage DeleteResponse(string url)
        {
            HttpClient client = GetService();
            HttpResponseMessage response = client.DeleteAsync(url).Result;
            return response;
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
            client.DefaultRequestHeaders.Accept.Add(contentType);
            return client;
        }

        public HttpResponseMessage PostResponse(string url,HttpContent content)
        {
            HttpClient client = GetService();
            HttpResponseMessage response = client.PostAsJsonAsync(url, content).Result;
            return response;
        }

        public HttpResponseMessage PostResponse(string url, T entity)
        {
            HttpClient client = GetService();
            HttpResponseMessage response = client.PostAsJsonAsync(url, entity).Result;
            return response;
        }

        public HttpResponseMessage PutResponse(string url, HttpContent content)
        {
            HttpClient client = GetService();
            HttpResponseMessage response = client.PutAsJsonAsync(url, content).Result;
            return response;
        }

        public HttpResponseMessage PutResponse(string url, T entity)
        {
            HttpClient client = GetService();
            HttpResponseMessage response = client.PutAsJsonAsync(url, entity).Result;
            return response;
        }
    }
}
