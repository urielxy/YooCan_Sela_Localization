using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Alto.Dal
{
    public class RedisWrapper
    {
        private readonly IDatabase _redisDatabase;
        private readonly ILogger<RedisWrapper> _logger;

        public RedisWrapper(IDatabase redisDatabase, ILogger<RedisWrapper> logger)
        {
            _redisDatabase = redisDatabase;
            _logger = logger;
        }
        public async Task<T> GetModelAsync<T>(string cacheKey, Func<Task<T>> getFromDbFunc)
        {
            T model;
            RedisValue redisValue;
            try
            {
                redisValue = await _redisDatabase.StringGetAsync(cacheKey);
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
                _redisDatabase.StringSet(cacheKey, redisValue, TimeSpan.FromHours(24), flags: CommandFlags.FireAndForget);
            }
            catch (Exception e)
            {
                _logger.LogError(112233, e, "Writing to Redis of {resource} failed", cacheKey);
            }
            return model;
        }
    }
}
