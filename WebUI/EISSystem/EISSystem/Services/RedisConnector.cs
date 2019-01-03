﻿using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;

namespace EIS.WebApp.Services
{
    public class RedisConnector
    {
        private static readonly Lazy<ConnectionMultiplexer> Connection;

        private static readonly string REDIS_CONNECTIONSTRING = "localhost";

        static RedisConnector()
        {
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            //var connectionString = config[REDIS_CONNECTIONSTRING];

            //if (connectionString == null)
            //{
            //    throw new KeyNotFoundException($"Environment variable for {REDIS_CONNECTIONSTRING} was not found.");
            //}

            var options = ConfigurationOptions.Parse("localhost");

            Connection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(options));
        }

        public static ConnectionMultiplexer GetConnection() => Connection.Value;
    }
}
