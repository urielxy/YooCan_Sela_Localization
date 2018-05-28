using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Yooocan.Dal
{
    public class RedisWrapper
    {
        public IDatabase RedisDatabase { get; }
        private readonly ILogger<RedisWrapper> _logger;

        public RedisWrapper(IDatabase redisDatabase, ILogger<RedisWrapper> logger)
        {
            RedisDatabase = redisDatabase;
            _logger = logger;
        }
        public async Task<T> GetModelAsync<T>(string cacheKey, Func<Task<T>> getFromDbFunc, TimeSpan expiry)
        {
            T model;
            RedisValue redisValue;
            try
            {
                redisValue = await RedisDatabase.StringGetAsync(cacheKey);
            }
            catch (Exception e)
            {
                _logger.LogError(321322, e, "Error when trying to get cache from Redis for {resource}", cacheKey);
                model = await getFromDbFunc();
                return model;
            }

            if (redisValue.HasValue)
                return JsonConvert.DeserializeObject<T>(redisValue);

            model = await getFromDbFunc();

            redisValue = JsonConvert.SerializeObject(model, Formatting.None, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });
            try
            {
                RedisDatabase.StringSet(cacheKey, redisValue, expiry, flags: CommandFlags.FireAndForget);
            }
            catch (Exception e)
            {
                _logger.LogError(112233, e, "Writing to Redis of {resource} failed", cacheKey);
            }
            return model;
        }
    }
}
