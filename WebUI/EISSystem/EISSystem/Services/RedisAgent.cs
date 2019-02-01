
using EIS.Entities.Address;
using Microsoft.AspNetCore.Http;
using StackExchange.Redis;

namespace EIS.WebApp.Services
{
    public class RedisAgent
    {
        private static IDatabase _database;
        public RedisAgent()
        {
            ConnectionMultiplexer connection = RedisConnector.GetConnection();

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
        public HashEntry[] GetList(string key)
        {
            return _database.HashGetAll(key);
        }
        public string GetValuefromList(string lkey, string vkey)
        {
            return _database.HashGet(lkey, vkey);
        }
    }
}
