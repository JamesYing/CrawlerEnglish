using System;
using System.Collections.Generic;
using System.Text;

namespace CrawlerEnglish
{
    public interface ICacheExample
    {
        void Add(string cacheKey, object obj, TimeSpan expiredTime);

        void Save(string cacheKey, object obj, TimeSpan expiredTime);

        T Get<T>(string cacheKey);

        T Retrieval<T>(string cacheKey, Func<T> func, TimeSpan expiredTime);

        bool Delete(string cacheKey);

        Dictionary<string, object> CacheManagerDict { get; }
    }

    public class BusinessClass
    {
        public void DoExampleMethod(string id, string name, string age)
        {
            //old Do anything
        }
    }
}
