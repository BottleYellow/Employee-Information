using StackExchange.Redis;
using System;
using System.Collections.Generic;

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
            _database.KeyExpire(key, DateTime.Now.AddDays(1));
        }

        public void DeleteStringValue(string key)
        {
            _database.KeyDelete(key);
        }
        public void SetList(int UserId,List<string> list)
        {
            HashEntry[] entry = new HashEntry[] 
            {
                new HashEntry("PersonId",list[0]),
                new HashEntry("TokenValue",list[1]),
                new HashEntry("Role",list[2]),
                new HashEntry("Access",list[3])
            };
            _database.HashSet("user:"+UserId, entry);
        }
    }
}
