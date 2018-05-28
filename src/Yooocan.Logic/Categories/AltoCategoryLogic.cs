using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Yooocan.Dal;
using Yooocan.Entities.Benefits;
using Yooocan.Entities.Companies;
using Yooocan.Enums;
using Yooocan.Models.Benefits;
using Yooocan.Models.Cards;
using Yooocan.Models.Categories;

namespace Yooocan.Logic.Categories
{
    public class AltoCategoryLogic : IAltoCategoryLogic
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMemoryCache _memoryCache;
        private readonly RedisWrapper _redisDatabase;

        public AltoCategoryLogic(ApplicationDbContext context, IMapper mapper, IServiceProvider serviceProvider, IMemoryCache memoryCache,
            RedisWrapper redisDatabase)
        {
            _context = context;
            _mapper = mapper;
            _serviceProvider = serviceProvider;
            _memoryCache = memoryCache;
            _redisDatabase = redisDatabase;
        }

        public async Task<AltoCategoryFeedModel> GetFeedModelAsync(int id)
        {
            var cacheKey = GetCategoryCacheKey(id);
            return await _redisDatabase.GetModelAsync(cacheKey, () => GetCategoryFeedModelFromDbAsync(id), TimeSpan.FromDays(1));
        }

        private async Task<AltoCategoryFeedModel> GetCategoryFeedModelFromDbAsync(int id)
        {
            Task<List<Benefit>> benefitsTask;
            Task<AltoCategory> categoryTask;
            using (var benefitsContext = _serviceProvider.GetService<ApplicationDbContext>())
            using (var categoryContext = _serviceProvider.GetService<ApplicationDbContext>())
            {
                var tasks = new List<Task>();
                benefitsTask = benefitsContext.Benefits
                    .Include(x => x.Categories)
                    .Include(x => x.Images)
                    .Include(x => x.Company)
                    .Where(b => b.IsPublished && b.DeleteDate == null && b.Categories.Select(bc => bc.CategoryId).Contains(id))
                    .AsNoTracking()
                    .ToListAsync();
                tasks.Add(benefitsTask);

                categoryTask = categoryContext.AltoCategories
                    .Include(x => x.ParentCategory)
                    .ThenInclude(x => x.Images)
                    .SingleAsync(x => x.Id == id);
                tasks.Add(categoryTask);

                await Task.WhenAll(tasks);
            }
            
            var benefitsModels = _mapper.Map<List<BenefitCardModel>>(benefitsTask.Result);
            var category = categoryTask.Result;
            var subCategories = await _context.AltoCategories
                .Where(x => x.ParentCategoryId == category.ParentCategoryId && !x.IsDeleted && x.IsActive)
                .OrderBy(x => x.Name)
                .ToDictionaryAsync(x => x.Id, x => x.Name);
            var model = new AltoCategoryFeedModel
                        {
                            BenefitsStrips = new List<BenefitsStripModel>
                                             {
                                                 new BenefitsStripModel
                                                 {
                                                     Title = "MOST POPULAR BENEFITS",
                                                     Benefits = benefitsModels.Take(6).ToList()
                                                 },
                                                 new BenefitsStripModel
                                                 {
                                                     Title = "NEWEST BENEFITS",
                                                     Benefits = benefitsModels.Skip(6).ToList()
                                                 }
                                             },
                            Id = id,
                            CategoryName = category.Name,
                            ParentCategoryId = category.ParentCategoryId,
                            HeaderImageUrl = category.ParentCategory.Images.Where(i => i.Type == AltoImageType.Header).Select(i => i.CdnUrl).Single(),
                            ParentCategoryName = category.ParentCategory.Name,
                            SubCategories = subCategories
                        };          

            return model;
        }

        public async Task<AltoCategoryFeedModel> GetParentFeedModelAsync(int id)
        {
            var cacheKey = GetCategoryCacheKey(id);

            return await _redisDatabase.GetModelAsync(cacheKey, () => GetParentCategoryModelFromDbAsync(id), TimeSpan.FromDays(1));            
        }

        private string GetCategoryCacheKey(int id)
        {
            var cacheKey = string.Format(RedisKeys.AltoCategoryModel, id);
            return cacheKey;
        }

        private async Task<AltoCategoryFeedModel> GetParentCategoryModelFromDbAsync(int id)
        {
            var subCategories = await _context.AltoCategories
                .Where(x => x.ParentCategoryId == id && !x.IsDeleted && x.IsActive)
                .ToDictionaryAsync(x => x.Id, x => x.Name);

            var subCategoriesIds = subCategories.Select(x => x.Key).ToList();
            
            Task<List<Benefit>> benefitsTask;
            Task<AltoCategory> categoryTask;
            using (var benefitsContext = _serviceProvider.GetService<ApplicationDbContext>())
            using (var categoryContext = _serviceProvider.GetService<ApplicationDbContext>())
            {
                var tasks = new List<Task>();
                benefitsTask = benefitsContext.Benefits
                    .Include(x => x.Categories)
                    .Include(x => x.Company)
                    .Include(x => x.Images)
                    .Where(b => b.IsPublished &&
                                b.DeleteDate == null &&
                                b.Categories
                                    .Select(bc => bc.CategoryId)
                                    .Any(categoryId => subCategoriesIds.Contains(categoryId) || categoryId == id))
                    .AsNoTracking()
                    .ToListAsync();
                tasks.Add(benefitsTask);

                categoryTask = categoryContext.AltoCategories
                    .Include(x => x.Images)
                    .SingleAsync(x => x.Id == id);
                tasks.Add(categoryTask);

                await Task.WhenAll(tasks);
            }
            
            var benefitsModels = _mapper.Map<List<BenefitCardModel>>(benefitsTask.Result);
            var benefitsGroups = benefitsModels
                .Where(x => x.CategoryId != id)
                .GroupBy(x => x.CategoryId)
                .ToDictionary(x => new KeyValuePair<int, string>(x.Key, subCategories[x.Key]), x => x.ToList());
            var nonEmptySubCategoryIds = benefitsGroups.Select(x => x.Key.Key).ToList();
            var category = categoryTask.Result;

            var model = new AltoCategoryFeedModel
                        {
                            BenefitsStrips = benefitsGroups.Select(x => new BenefitsStripModel
                                                                        {
                                                                            Title = x.Key.Value + " BENEFITS",
                                                                            Benefits = x.Value
                                                                        }).OrderBy(x => x.Title).ToList(),
                            Id = id,
                            CategoryName = category.Name,
                            HeaderImageUrl = category.Images.Where(i => i.Type == AltoImageType.Header).Select(i => i.CdnUrl).SingleOrDefault()
                        };

            model.SubCategories = subCategories.Where(x => nonEmptySubCategoryIds.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);            

            return model;
        }

        public async Task<Dictionary<string, Dictionary<int, string>>> GetCategoriesOptionsAsync()
        {
            const string key = nameof(GetCategoriesOptionsAsync);

            var categories = await _memoryCache.GetOrCreateAsync(key, async entry =>
            {
                var results = (await _context.AltoCategories
                        .Include(x => x.ParentCategory)
                        .Where(x => x.ParentCategoryId != null && !x.IsDeleted)
                        .OrderBy(x => x.ParentCategory.Name)
                        .ThenBy(x => x.Name)
                        .ToListAsync())
                    .GroupBy(x => x.ParentCategory)
                    .ToDictionary(x => x.Key.Name, x => x.ToDictionary(category => category.Id, category => category.Name));

                entry.SetAbsoluteExpiration(TimeSpan.FromDays(1));
                entry.SetValue(results);

                return results;
            });

            return categories;
        }

        public async Task<List<AltoCategoryMenuModel>> GetMenuCategories()
        {
            var redisKey = RedisKeys.AltoCategoriesMenuModel;
            var benefitMainCategoryIds = new[] { 104, 77, 120, 111 };
            var result = await _redisDatabase.GetModelAsync(redisKey, async () =>
            {
                var parentCategories = (await _context.AltoCategories
                        .Include(x => x.SubCategories)
                        .Include(x => x.Images)
                        .Where(x => !x.IsDeleted && x.IsActive)
                        .OrderBy(x => x.Name)
                        .ToListAsync())
                    .Where(x => x.ParentCategoryId == null && benefitMainCategoryIds.Contains(x.Id))
                    .ToList();

                var model = _mapper.Map<List<AltoCategoryMenuModel>>(parentCategories);                
                return model;
            }, TimeSpan.FromDays(1));

            return result;
        }
    }
}