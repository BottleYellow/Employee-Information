﻿using EIS.WebApp.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace EIS.WebApp.Services
{
    public class EISService : IEISService
    {
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

        
    }
}
