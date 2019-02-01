using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;

namespace EIS.WebApp.Services
{
    public class RedisConnector
    {
        private static readonly Lazy<ConnectionMultiplexer> Connection;

        static RedisConnector()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            //var connectionString = config[REDIS_CONNECTIONSTRING];

            //if (connectionString == null)
            //{
            //    throw new KeyNotFoundException($"Environment variable for {REDIS_CONNECTIONSTRING} was not found.");
            //}

            ConfigurationOptions options = ConfigurationOptions.Parse("localhost");

            Connection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(options));
        }

        public static ConnectionMultiplexer GetConnection() => Connection.Value;
    }
}
