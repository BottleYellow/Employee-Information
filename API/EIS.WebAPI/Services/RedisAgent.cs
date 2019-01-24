using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace EIS.WebAPI.Services
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
            _database.KeyExpire(key, DateTime.Now.AddDays(1));
        }

        public void DeleteStringValue(string key)
        {
            _database.KeyDelete(key);
        }
        public void SetList(string EmployeeCode, List<string> list)
        {
            HashEntry[] entry = new HashEntry[]
            {
                new HashEntry("PersonId",list[0]),
                new HashEntry("TokenValue",list[1]),
                new HashEntry("Role",list[2]),
                new HashEntry("Access",list[3]),
                new HashEntry("TenantId",list[4])
            };
            _database.HashSet("user:" + EmployeeCode, entry);
        }
        public string GetValueFromList(string key, string field)
        {
            string result = null;
            result=_database.HashGet(key, field);
            return result;
        }
        public void DeleteValueFromList(string key, string field)
        {
            _database.HashDelete(key, field);
        }
    }
}
