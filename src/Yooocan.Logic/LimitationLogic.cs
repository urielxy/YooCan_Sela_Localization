using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Yooocan.Dal;

namespace Yooocan.Logic
{
    public class LimitationLogic : ILimitationLogic
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _memoryCache;

        public LimitationLogic(ApplicationDbContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }

        public async Task<Dictionary<int, string>> GetLimitationsAsync()
        {
            var limitations = await _memoryCache.GetOrCreateAsync("LimitationsDictionary", async entry =>
            {
                var results = await _context.Limitations
                    .Where(x => x.ParentLimitationId == null)
                    .OrderBy(x => x.Name == "Other")
                    .ThenBy(x => x.Name)
                    .ToDictionaryAsync(x => x.Id, x => x.Name);
                entry.SetValue(results);
                entry.SetAbsoluteExpiration(TimeSpan.FromDays(1));
                return results;
            });

            return limitations;
        }
    }
}