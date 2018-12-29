using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EIS.WebAPI.RedisCache
{
    public class RedisConnector
    {
        private static readonly Lazy<ConnectionMultiplexer> Connection;

        static RedisConnector()
        {
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();
            var options = ConfigurationOptions.Parse("localhost");

            Connection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(options));
        }

        public static ConnectionMultiplexer GetConnection() => Connection.Value;
    }
}
