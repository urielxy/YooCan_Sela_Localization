using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using Yooocan.Dal;
using Yooocan.Entities;
using Yooocan.Entities.Benefits;
using Yooocan.Enums;
using Yooocan.Logic.Extensions;
using Yooocan.Models;
using Yooocan.Models.Cards;
using Yooocan.Models.Feeds;
using Yooocan.Models.Products;
using Yooocan.Models.SearchIndexes;
using ServiceProvider = Yooocan.Entities.ServiceProviders.ServiceProvider;

namespace Yooocan.Logic
{
    public class SearchLogic
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SearchLogic> _logger;
        private readonly SearchServiceClient _searchClient;
        private readonly IMemoryCache _memoryCache;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;
        private readonly IDatabase _redisDatabase;

        private const string StoriesIndexName = "stories";
        private const string ProductsIndexName = "products";
        private const string ServiceProvidersIndexName = "service-providers";
        private const string BenefitsIndexName = "benefits";

        private Func<StoryIndexModel, string> StoryIndexIdSelector = model => model.StoryId;
        private Func<ProductIndexModel, string> ProductIndexIdSelector = model => model.ProductId;
        private Func<BenefitIndexModel, string> BenefitIndexIdSelector = model => model.BenefitId;
        private Func<ServiceProviderIndexModel, string> ServiceProviderIndexIdSelector = model => model.ServiceProviderId;

        public SearchLogic(ApplicationDbContext context, ILogger<SearchLogic> logger, SearchServiceClient searchClient, IMemoryCache memoryCache, IMapper mapper,
            IServiceProvider serviceProvider, IDatabase redisDatabase)
        {
            _context = context;
            _logger = logger;
            _searchClient = searchClient;
            _memoryCache = memoryCache;
            _mapper = mapper;
            _serviceProvider = serviceProvider;
            _redisDatabase = redisDatabase;
        }

        public async Task<SearchResult> SearchAsync(string text, int? categoryId, List<int> limitationIds, SearchType? searchType = null)
        {
            var filter = CreateODataFilter(categoryId, limitationIds);

            var azureStopwatch = new Stopwatch();
            azureStopwatch.Start();
            var storiesTask = Task.FromResult(new List<Story>());
            if (searchType == null || searchType == SearchType.Stories)
                storiesTask = SearchEntities(text, categoryId, limitationIds, StoriesIndexName, StoryIndexIdSelector, GetStoriesFromDb);

            var productsTask = SearchEntities(text, categoryId, limitationIds, ProductsIndexName, ProductIndexIdSelector, GetProductsFromDb);
            var benefitsTask = SearchEntities(text, null, null, BenefitsIndexName, BenefitIndexIdSelector, GetBenefitsFromDb);
            var serviceProviderTask = SearchEntities(text, categoryId, limitationIds, ServiceProvidersIndexName, ServiceProviderIndexIdSelector, GetServiceProvidersFromDb);

            await Task.WhenAll(storiesTask, serviceProviderTask, productsTask, benefitsTask);
            _logger.LogSearchResults(text, storiesTask.Result.Count, productsTask.Result.Count, serviceProviderTask.Result.Count, benefitsTask.Result.Count, azureStopwatch.ElapsedMilliseconds);
            return new SearchResult
            {
                Stories = storiesTask.Result,
                Products = productsTask.Result,
                ServiceProviders = serviceProviderTask.Result,
                Benefits = benefitsTask.Result
            };
        }

        #region Search implementation

        private Task<List<TEntity>> SearchEntities<TEntity, TIndexModel>(string text, int? categoryId, List<int> limitationIds, string indexName, Func<TIndexModel, string> idSelector,
            Func<List<int>, Task<List<TEntity>>> entitiesByIdsQueryFunction, int pageIndex = 0) where TIndexModel : class
        {
            //Use fuzzy search
            if (text != null && text.Length >= 3)
            {
                text = new string(text.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)).ToArray());
                text = string.Join(" ", text.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Select(word => $"{word}~1"));
            }
            var filter = CreateODataFilter(categoryId, limitationIds);

            const int resultsPerPage = 24;
            var azureStopwatch = new Stopwatch();
            azureStopwatch.Start();
            return _searchClient.Indexes.GetClient(indexName)
               .Documents.SearchAsync<TIndexModel>(text, new SearchParameters
               {
                   Top = resultsPerPage,
                   Skip = pageIndex * resultsPerPage,
                   Filter = filter,
                   QueryType = QueryType.Full
               })
               .ContinueWith(async t =>
               {
                   _logger.LogInformation((int)LoggingEvent.Search, "Search in Azure for {index} took {ElapsedMilliseconds}", indexName,
                       azureStopwatch.ElapsedMilliseconds);
                   var ids = t.Result.Results.OrderByDescending(x => x.Score).Select(x => int.Parse(idSelector(x.Document))).ToList();
                   if (ids.Count == 0)
                       return new List<TEntity>();

                   return await entitiesByIdsQueryFunction(ids);
               }).Unwrap();
        }

        private string CreateODataFilter(int? categoryId, List<int> limitationIds)
        {
            string filter = null;

            if (limitationIds != null && limitationIds.Any())
                filter = $"({string.Join(" or ", limitationIds.Select(limitationId => $"LimitationIds/any(l: l eq '{limitationId}')"))})";

            if (categoryId != null)
            {
                if (filter != null)
                    filter += " and ";
                filter += $"CategoryIds/any(l: l eq '{categoryId}')";
            }

            return filter;
        }

        #region Getting entities from the db by their ids

        private async Task<List<Story>> GetStoriesFromDb(List<int> ids)
        {
            using (var context = _serviceProvider.GetRequiredService<ApplicationDbContext>())
            {
                var stories = await context.Stories
                    .Include(x => x.Paragraphs)
                    .Include(x => x.Images)
                    .Include(x => x.User)
                    .Include(x => x.StoryCategories)
                    .ThenInclude(x => x.Category)
                    .ThenInclude(x => x.ParentCategory)
                    .Where(x => ids.Contains(x.Id) && x.IsPublished && !x.IsDeleted)
                    .AsNoTracking()
                    .ToListAsync();

                // http://stackoverflow.com/a/3946021/601179
                var storiesOrderedByScore = (from id in ids
                                             join story in stories
                                             on id equals story.Id
                                             select story).ToList();
                return storiesOrderedByScore;
            }
        }

        private async Task<List<Product>> GetProductsFromDb(List<int> ids)
        {
            using (var context = _serviceProvider.GetRequiredService<ApplicationDbContext>())
            {
                var products = await context.Products
                    .Where(x => ids.Contains(x.Id) && x.IsPublished && !x.IsDeleted)
                    .Include(x => x.Company)
                    .Include(x => x.Images)
                    .Include(x => x.ProductCategories)
                    .ThenInclude(x => x.Category)
                    .ThenInclude(x => x.ParentCategory)
                    .AsNoTracking()
                    .ToListAsync();

                // http://stackoverflow.com/a/3946021/601179
                var productsOrderedByScore = (from id in ids
                                              join product in products
                                              on id equals product.Id
                                              select product).ToList();
                return productsOrderedByScore;
            }
        }

        private async Task<List<Benefit>> GetBenefitsFromDb(List<int> ids)
        {
            using (var context = _serviceProvider.GetRequiredService<ApplicationDbContext>())
            {
                var benefits = await context.Benefits
                    .Where(x => ids.Contains(x.Id) && x.IsPublished && x.DeleteDate == null)
                    .Include(x => x.Company)
                        .ThenInclude(company => company.Images)
                    .Include(x => x.Images)
                    .AsNoTracking()
                    .ToListAsync();

                // http://stackoverflow.com/a/3946021/601179
                var benefitsOrderedByScore = (from id in ids
                                              join benefit in benefits
                                              on id equals benefit.Id
                                              select benefit).ToList();
                return benefitsOrderedByScore;
            }
        }

        private async Task<List<ServiceProvider>> GetServiceProvidersFromDb(List<int> ids)
        {
            using (var context = _serviceProvider.GetRequiredService<ApplicationDbContext>())
            {
                var serviceProviders = await context.ServiceProviders
                    .Where(x => ids.Contains(x.Id) && x.IsPublished && !x.IsDeleted)
                    .Include(x => x.Images)
                    .AsNoTracking()
                    .ToListAsync();

                // http://stackoverflow.com/a/3946021/601179
                var serviceProvidersOrderedByScore = (from id in ids
                                                      join serviceProvider in serviceProviders
                                                      on id equals serviceProvider.Id
                                                      select serviceProvider).ToList();
                return serviceProvidersOrderedByScore;
            }
        }

        #endregion

        #endregion

        #region Search Load More

        public async Task<List<StoryCardModel>> SearchStories(string text, int? categoryId, List<int> limitationIds, int pageIndex)
        {
            var results = await SearchEntities(text, categoryId, limitationIds, StoriesIndexName, StoryIndexIdSelector, GetStoriesFromDb, pageIndex);
            var models = _mapper.Map<List<StoryCardModel>>(results);
            return models;
        }

        public async Task<List<ProductCardModel>> SearchProducts(string text, int? categoryId, List<int> limitationIds, int pageIndex)
        {
            var results = await SearchEntities(text, categoryId, limitationIds, ProductsIndexName, ProductIndexIdSelector, GetProductsFromDb, pageIndex);
            var models = _mapper.Map<List<ProductCardModel>>(results);
            return models;
        }

        public async Task<List<BenefitCardModel>> SearchBenefits(string text, int pageIndex)
        {
            var results = await SearchEntities(text, null, null, BenefitsIndexName, BenefitIndexIdSelector, GetBenefitsFromDb, pageIndex);
            var models = _mapper.Map<List<BenefitCardModel>>(results);
            return models;
        }

        public async Task<List<RelatedServiceProviderModel>> SearchServiceProviders(string text, int? categoryId, List<int> limitationIds, int pageIndex)
        {
            var results = await SearchEntities(text, categoryId, limitationIds, ServiceProvidersIndexName, ServiceProviderIndexIdSelector, GetServiceProvidersFromDb, pageIndex);
            var models = _mapper.Map<List<RelatedServiceProviderModel>>(results);
            return models;
        }

        #endregion

        public async Task<List<Category>> GetMainCategoriesAsync(int page, int pageSize)
        {
            var key = $"GetMainCategoriesAsync-page{page}-pageSize{pageSize}";

            var categories = await _memoryCache.GetOrCreateAsync(key, async entry =>
            {
                var skip = page * pageSize;
                entry.SetSlidingExpiration(TimeSpan.FromHours(1));
                entry.SetAbsoluteExpiration(TimeSpan.FromHours(3));
                List<Category> categoriesItem =
                    await _context.Categories.Where(x => x.ParentCategoryId == null && x.IsActiveForFeed)
                        .Skip(skip)
                        .Take(pageSize)
                        .OrderBy(x => x.Name == "OTHERS")
                        .ThenBy(x => x.Name)
                        .AsNoTracking()
                        .ToListAsync();
                return categoriesItem;
            });

            return categories;
        }

        public async Task<Dictionary<int, int>> GetReadHistoryForMainFeedAsync(string userId)
        {
            var registrationDate = await _context.Users.Where(x => x.Id == userId).Select(x => x.InsertDate).SingleAsync();

            const string queryText = @"SELECT ParentCategory.Id, Count(1) as [Count]
FROM   Categories ParentCategory
INNER JOIN Categories ChildCategory
	ON ParentCategory.Id = ChildCategory.ParentCategoryId
INNER JOIN StoryCategories SC
	ON SC.CategoryId = ChildCategory.Id
INNER JOIN Stories S
	ON SC.StoryId = S.Id
WHERE ParentCategory.IsActiveForFeed = 1
AND   ChildCategory.IsActiveForFeed = 1
AND   SC.IsPrimary = 1
AND   S.IsPublished = 1
AND   NOT EXISTS (SELECT 1
		  		  FROM   ReadHistories RH
				  WHERE  RH.UserId = @userId
				  AND    RH.InsertDate >= S.PublishDate
				  AND    (RH.StoryId = S.Id OR RH.CategoryId = ParentCategory.Id))
AND   S.PublishDate > GetUtcDate() -30
AND   S.PublishDate > @registrationDate
GROUP BY ParentCategory.Id
HAVING   COUNT(S.Id) > 0";

            var con = (SqlConnection)_context.Database.GetDbConnection();
            await con.OpenAsync();
            var query = new SqlCommand(queryText, con);
            query.Parameters.AddWithValue("@userId", userId);
            query.Parameters.AddWithValue("@registrationDate", registrationDate);

            var reader = await query.ExecuteReaderAsync();
            var results = new Dictionary<int, int>();
            while (await reader.ReadAsync())
            {
                results[(int)reader["Id"]] = (int)reader["Count"];
            }

            return results;
        }

        public async Task<List<string>> GetSubCategoriesTextAsync()
        {
            const string key = nameof(GetSubCategoriesTextAsync);
            var categories = await _memoryCache.GetOrCreateAsync(key, async entry =>
            {
                var result = await _context.Categories.Where(x => x.ParentCategoryId != null && x.IsActiveForFeed)
                    .Select(x => x.Name)
                    .ToListAsync();
                result = result.OrderBy(x => Guid.NewGuid()).ToList();

                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

                return result;
            });

            return categories;
        }

        public async Task<FeedCategoryModel> GetFeedFromDbAsync(int id, int count)
        {
            var category = await _context.Categories.SingleOrDefaultAsync(x => x.Id == id);
            if (category == null)
                return null;

            var storiesModels = await GetStoriesFromDbAsync(id, count);

            var categories = await _context.Categories.Where(x => x.IsActiveForFeed).ToListAsync();

            var model = new FeedCategoryModel
            {
                Stories = storiesModels,
                CategoryColor = category.ShopBackgroundColor,
                Id = category.Id,
                HeaderImageUrl = category.HeaderPictureUrl,
                MobileHeaderImageUrl = category.MobileHeaderPictureUrl ?? category.HeaderPictureUrl,
                Name = category.Name,
                Description = category.Description,
                ParentCategoryId = category.ParentCategoryId,
                ParentCategoryName = category.ParentCategory?.Name
            };

            var menuModel = categories.Where(x => x.ParentCategoryId == null)
                .OrderBy(x => x.Name)
                .Select(x => new FeedMenuModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    IconUrl = x.MenuIconUrl,
                    SubCategories = categories
                                     .Where(sub => sub.ParentCategoryId == x.Id)
                                     .OrderBy(sub => sub.Name)
                                     .Select(sub => new FeedMenuModel
                                     {
                                         Id = sub.Id,
                                         Name = sub.Name
                                     }).ToList()
                })
                .ToList();
            model.Categories = menuModel;
            return model;
        }

        public async Task<List<StoryCardModel>> GetStoriesFromDbAsync(int categoryId, int count, int offset = 0)
        {
            var stories = await _context.Stories
                .Include(x => x.Paragraphs)
                .Include(x => x.Images)
                .Include(x => x.User)
                .Include(x => x.StoryCategories)
                    .ThenInclude(x => x.Category)
                    .ThenInclude(x => x.ParentCategory)
                .Include(x => x.StoryLimitations)
                    .ThenInclude(x => x.Limitation)
                .Where(x => x.IsPublished && !x.IsNoIndex &&
                            x.StoryCategories.Any(sc => sc.CategoryId == categoryId || sc.Category.ParentCategoryId == categoryId))
                .OrderByDescending(x => x.StoryCategories.Any(sc => sc.IsPrimary && (sc.CategoryId == categoryId || sc.Category.ParentCategoryId == categoryId)))
                .ThenByDescending(x => x.PublishDate)
                .Skip(offset)
                .Take(count)
                .ToListAsync();
            var storiesModels = _mapper.Map<List<StoryCardModel>>(stories);

            return storiesModels;
        }

        public async Task<FeedCategoryModel> GetFeedAsync(int categoryId, string userId, int count, int defaultCount)
        {
            var redisKey = string.Format(RedisKeys.Feed, categoryId);
            FeedCategoryModel model;
            RedisValue redisValue = RedisValue.Null;
            try
            {
                if (count <= defaultCount)
                    redisValue = _redisDatabase.StringGet(redisKey);
            }
            catch (Exception e)
            {
                _logger.LogError(321322, e, "Error when trying to get cache from Redis for {resource}", redisKey);
                model = await GetFeedFromDbAsync(categoryId, count);
                if (userId != null)
                    model.IsFollowed = await _context.CategoryFollowers.AnyAsync(x => x.CategoryId == categoryId && x.DeleteDate == null && x.UserId == userId);

                return model;
            }
            if (!redisValue.IsNull)
            {
                model = JsonConvert.DeserializeObject<FeedCategoryModel>(redisValue.ToString());
            }
            else
            {
                model = await GetFeedFromDbAsync(categoryId, count);
                if (model == null)
                    return null;

                var serializedModel = JsonConvert.SerializeObject(model);
                try
                {
                    _redisDatabase.StringSet(redisKey, serializedModel, TimeSpan.FromHours(1));
                }
                catch (Exception e)
                {
                    _logger.LogError(321323, e, "Error when trying to set cache in Redis for {resource}", redisKey);
                }
            }

            if (userId != null)
                model.IsFollowed = await _context.CategoryFollowers.AnyAsync(x => x.CategoryId == categoryId && x.DeleteDate == null && x.UserId == userId);

            return model;

        }
    }
}