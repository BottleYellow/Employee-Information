using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EIS.WebAPI.RedisCache
{
    public class RedisAgent
    {
        private static IDatabase _database;
        public RedisAgent()
        {
            var connection = RedisConnector.GetConnection();

            _database = connection.GetDatabase();
        }

        public string GetStringValue(string key)
        {
            return _database.StringGet(key);
        }

        public void SetStringValue(string key, string value)
        {
            _database.StringSet(key, value);
        }

        public void DeleteStringValue(string key)
        {
            _database.KeyDelete(key);
        }
    }
}
