using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Yooocan.Dal;
using Yooocan.Entities;
using Yooocan.Models;
using System.Runtime.CompilerServices;

namespace Yooocan.Logic
{
    public class CategoriesLogic : ICategoriesLogic
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<CategoriesLogic> _logger;
        private readonly IMapper _mapper;

        public CategoriesLogic(ApplicationDbContext context, IMemoryCache memoryCache, ILogger<CategoriesLogic> logger, IMapper mapper)
        {
            _context = context;
            _memoryCache = memoryCache;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Dictionary<int, string>> GetMainCategoriesForSearchAsync()
        {
            return await GetOrCreateWithMemoryCacheAsync(async () =>
            {
                var results = (await _context.Categories
                        .Where(x => x.IsActiveForFeed && x.ParentCategoryId == null)
                        .OrderBy(x => x.Name == "OTHERS")                        
                        .ThenBy(x => x.ParentCategory.Name)
                        .ThenBy(x => x.Name)
                        .AsNoTracking()
                        .ToListAsync())
                        .ToDictionary(x => x.Id, x => x.Name);

                return results;
            });
        }

        public async Task<Dictionary<string, Dictionary<int, string>>> GetCategoriesForStoryAsync()
        {
            return await GetOrCreateWithMemoryCacheAsync(async () =>
            {
                var results = (await _context.Categories
                        .Include(x => x.ParentCategory)
                        .Where(x => x.ParentCategoryId != null && x.IsChoosableForStory)
                        .OrderBy(x => x.ParentCategory.Name == "OTHERS")
                        .ThenBy(x => x.ParentCategory.Name)
                        .ThenBy(x => x.Name)
                        .ToListAsync())
                    .GroupBy(x => x.ParentCategory)
                    .ToDictionary(x => x.Key.Name, x => x.ToDictionary(category => category.Id, category => category.Name));                

                return results;
            });            
        }

        public async Task<Dictionary<string, Dictionary<int, string>>> GetCategoriesForProductAsync()
        {
            return await GetOrCreateWithMemoryCacheAsync(async () =>
            {
                var results = (await _context.Categories
                        .Include(x => x.ParentCategory)
                        .Where(x => x.ParentCategoryId != null && x.IsChoosableForProduct)
                        .OrderBy(x => x.ParentCategory.Name == "OTHERS")
                        .ThenBy(x => x.ParentCategory.Name)
                        .ThenBy(x => x.Name)
                        .ToListAsync())
                    .GroupBy(x => x.ParentCategory)
                    .ToDictionary(x => x.Key.Name, x => x.ToDictionary(category => category.Id, category => category.Name));

                return results;
            });
        }

        public async Task<List<CategoryModel>> GetMenuFeedCategories()
        {
            return await GetOrCreateWithMemoryCacheAsync(async () =>
            {
                var categories = await _context.Categories
                .Where(x => x.IsActiveForFeed && x.ParentCategoryId == null)
                .OrderBy(x => x.Name == "OTHERS")
                .ThenBy(x => x.Name)
                .ToListAsync();

                var model = _mapper.Map<List<CategoryModel>>(categories);
                return model;
            });
        }

        public async Task<List<CategoryModel>> GetMenuShopAndServiceProvidersCategories()
        {
            return await GetOrCreateWithMemoryCacheAsync(async () =>
            {
                var categories = await _context.Categories
                    .Where(x => x.IsActiveForShop && x.ParentCategoryId == null)
                    .OrderBy(x => x.Name == "OTHERS")
                    .ThenBy(x => x.Name)
                    .ToListAsync();

                var model = _mapper.Map<List<CategoryModel>>(categories);
                return model;
            });
        }

        private async Task<T> GetOrCreateWithMemoryCacheAsync<T>(Func<Task<T>> getter, [CallerMemberName] string keySuffix = null, int expiryInHours = 12)
        {
            if(keySuffix == null)
            {
                throw new ArgumentNullException("keySuffix should have been filled by the compiler");
            }

            var key = $"CategoriesLogic:{keySuffix}";

            var categories = await _memoryCache.GetOrCreateAsync(key, async entry =>
            {
                var results = await getter();

                entry.SetAbsoluteExpiration(TimeSpan.FromHours(expiryInHours));
                entry.SetValue(results);

                return results;
            });

            return categories;
        }

        public async Task FollowCategoryAsync(int id, string userId)
        {
            if (await _context.CategoryFollowers.AnyAsync(x => x.UserId == userId && x.CategoryId == id && x.DeleteDate == null))
            {
                _logger.LogWarning("Category {id} is already followed by {userId}", id, userId);
                return;
            }

            var categoryFollower = new CategoryFollower
            {
                UserId = userId,
                CategoryId = id,
            };
            _context.Add(categoryFollower);

            var sql = $"UPDATE Categories SET {nameof(Category.FollowersCount)} = {nameof(Category.FollowersCount)} +1 WHERE Id = {id}";
            await _context.Database.ExecuteSqlCommandAsync(sql);
            await _context.SaveChangesAsync();
        }

        public async Task UnfollowCategoryAsync(int id, string userId)
        {
            var categoryFollowers = await _context.CategoryFollowers.Where(x => x.UserId == userId && x.CategoryId == id && x.DeleteDate == null).ToListAsync();
            if (categoryFollowers.Count == 0)
            {
                _logger.LogWarning("Category {id} is not followed by {userId}", id, userId);
                return;
            }

            foreach (var category in categoryFollowers)
            {
                category.DeleteDate = DateTime.UtcNow;
            }

            var sql = $"UPDATE Categories SET {nameof(Category.FollowersCount)} = {nameof(Category.FollowersCount)} -1 WHERE Id = {id}";
            await _context.Database.ExecuteSqlCommandAsync(sql);
            await _context.SaveChangesAsync();
        }
    }
}