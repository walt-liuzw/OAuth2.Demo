using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Beisen.RedisV2.Provider;
using Newtonsoft.Json;

namespace OAuth2.Demo.Common
{
    public static class RedisCacheHelper
    {
        private static readonly string REDIS_CACHE_KEYSPACE = "UPaaSRemoteCache";
        public static void Add(int tenantId,string key, string value, DateTime? expire = null)
        {
            using (var redis = new RedisNativeProviderV2(REDIS_CACHE_KEYSPACE, tenantId))
            {
                var serializedValue = JsonConvert.SerializeObject(value);
                if (expire.HasValue)
                    redis.SetEx(key, expire.Value.Second, StringToBytes(serializedValue));
                else
                    redis.Set(key, StringToBytes(serializedValue));
            }
        }

       
        public static T Get<T>(int tenantId, string key)
        {
            T result = default(T);
            using (var redis = new RedisNativeProviderV2(REDIS_CACHE_KEYSPACE, tenantId))
            {
                var value = redis.Get(key);
                if (value != null)
                    result = JsonConvert.DeserializeObject<T>(BytesToString(value));
            }

            return result;
        }
        public static int Remove(int tenantId, string key)
        {
            using (var redis = new RedisNativeProviderV2(REDIS_CACHE_KEYSPACE, tenantId))
            {
                return redis.Del(key);
            }
        }


        #region Pipeline
        public static void PipelineSet<T>(int tenantId, string key, T value, Action onSuccess = null, Action<Exception> onException = null)
        {
            using (var redis = new RedisNativePipelineProviderV2(REDIS_CACHE_KEYSPACE, tenantId))
            {
                var serializedValue = JsonConvert.SerializeObject(value);
                redis.Set(key, StringToBytes(serializedValue), onSuccess, onException);
                redis.Flush();
            }
        }
        #endregion



        private static string BytesToString(byte[] value)
        {
            if (value == null)
                return null;

            return Encoding.Default.GetString(value);
        }
        private static byte[] StringToBytes(string serializedValue)
        {
            if (serializedValue == null)
                return null;

            return Encoding.Default.GetBytes(serializedValue);
        }


    }
}