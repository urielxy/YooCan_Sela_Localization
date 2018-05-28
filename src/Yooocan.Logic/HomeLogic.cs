using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using MoreLinq;
using Yooocan.Dal;
using Yooocan.Entities;
using Yooocan.Enums;
using Yooocan.Models;
using Yooocan.Models.New.Home;
using Yooocan.Models.Products;
using Yooocan.Models.Blog;

namespace Yooocan.Logic
{
    public class HomeLogic : IHomeLogic
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeLogic> _logger;
        private readonly IMapper _mapper;
        private readonly RedisWrapper _redisWrapper;
        private readonly IServiceProvider _serviceProvider;
        private readonly IOldProductLogic _productLogic;
        private readonly ICategoriesLogic _categoriesLogic;

        public HomeLogic(ApplicationDbContext context, ILogger<HomeLogic> logger, IMapper mapper, RedisWrapper redisWrapper, IServiceProvider serviceProvider,
            IOldProductLogic productLogic, ICategoriesLogic categoriesLogic)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _serviceProvider = serviceProvider;
            _productLogic = productLogic;
            _categoriesLogic = categoriesLogic;
            _redisWrapper = redisWrapper;
        }

        public async Task<HomeModel> GetModelAsync()
        {
            return await _redisWrapper.GetModelAsync(RedisKeys.HomeModel, GetHomeModelFromDbAsync, TimeSpan.FromHours(3));
        }

        public async Task<List<FeaturedStoryHeader>> GetLatestStoriesPerCategoryAsync()
        {
            return await _redisWrapper.GetModelAsync(RedisKeys.LatestStoriesPerCategory, async () =>
            {
                //TODO: check if below or similar works on EF Core 2 to replace the raw SQL query
                //var latestStories = Context.Categories.Where(x => x.IsActiveForFeed && x.ParentCategoryId != null)
                //    .Select(c => c.StoryCategories.Where(sc => sc.IsPrimary).Select(sc => sc.Story).OrderByDescending(s => s.PublishDate).Take(1).DefaultIfEmpty())
                //    .SelectMany(x => x)
                //    .ToList();      
                var latestStoriesPerCategory = await _context.Stories.FromSql(
@"SELECT * from (
	select
		s.*,
		SC.CategoryId,
        PC.Name as MainCategoryName,
		rowid = ROW_NUMBER() OVER (PARTITION BY pc.Id ORDER BY s.InsertDate desc)
    FROM StoryCategories SC
	  inner join categories c on sc.categoryid = c.id
	  inner join categories pc on c.parentcategoryid = pc.id
	  INNER JOIN Stories s ON s.Id = SC.StoryId
	WHERE s.IsPublished = 1 and s.isdeleted = 0 and s.IsNoIndex = 0
      and pc.isactiveforfeed = 1
	  and SC.IsPrimary = 1 and sc.isdeleted = 0) as t
where t.rowid = 1")
                    .Include(x => x.Images)
                    .Include(x => x.User)
                    .Include(x => x.StoryCategories)
                    .ThenInclude(x => x.Category)
                    .ThenInclude(x => x.ParentCategory)
                    .ToListAsync();
                return _mapper.Map<List<Story>, List<FeaturedStoryHeader>>(latestStoriesPerCategory).OrderBy(x => x.Category != "DAILY LIVING & MOBILITY")
                                                                            .ThenBy(x => x.Category != "FASHION")
                                                                            .ThenBy(x => x.Category != "SPORTS, FITNESS & DANCE")
                                                                            .ThenBy(x => x.Category == "OTHERS")
                                                                            .ThenBy(x => x.Category)
                                                                            .ToList();
            }, TimeSpan.FromHours(3));
        }

        public async Task<MobileHomeModel> GetMobileModelAsync()
        {
            return await _redisWrapper.GetModelAsync(RedisKeys.HomeModelMobile, async () =>
            {
                var model = new MobileHomeModel
                {
                    LatestStoriesPerCategory = await GetLatestStoriesPerCategoryAsync(),
                    FeaturedStory = (await GetModelAsync()).HeaderStories[0],
                    //FeaturedProduct = await _productLogic.GetProductOfTheDay(),
                    //ShopCategories = (await _categoriesLogic.GetMenuShopAndServiceProvidersCategories()).OrderBy(x => Guid.NewGuid()).ToList(),
                    FeatureBlogPost = await GetFeaturedBlogPost()
                };
                return model;
            }, TimeSpan.FromHours(1));
        }

        public async Task<ContentImFollowingModel> GetContentImFollowingAsync(string userId, int count, DateTime? maxDate = null, int? lastId = null)
        {
            ContentImFollowingModel models;
            ApplicationUser user = null;
            if (!string.IsNullOrEmpty(userId))
            {
                user = await _context.Users.SingleAsync(x => x.Id == userId);
            }

            if (user?.CustomizedFeedDone == true)
            {
                Task<List<Story>> limitationsTask;
                Task<List<Story>> categoriesTask;
                Task<List<Story>> usersTask;

                using (var categoriesContext = _serviceProvider.GetService<ApplicationDbContext>())
                using (var limitationsContext = _serviceProvider.GetService<ApplicationDbContext>())
                using (var userContext = _serviceProvider.GetService<ApplicationDbContext>())
                {
                    usersTask = userContext.
                        Stories.Where(x => x.IsPublished && !x.IsNoIndex && x.User.Followers.Any(f => !f.IsDeleted && f.FollowerUserId == userId))
                        .Where(StoryWasNotLoaded(maxDate, 0))
                        .Include(x => x.Paragraphs)
                        .Include(x => x.StoryCategories)
                        .ThenInclude(x => x.Category)
                        .ThenInclude(x => x.ParentCategory)
                        .Include(x => x.Images)
                        .Include(x => x.User)
                        .OrderByDescending(x => x.PublishDate)
                            .ThenByDescending(x => x.Id)
                        .AsNoTracking()
                        .Take(15)
                        .ToListAsync();

                    var categoriesIdsTask = categoriesContext.CategoryFollowers.Where(x => x.UserId == userId && x.DeleteDate == null)
                        .SelectMany(x => x.Category.SubCategories.Where(c => c.IsActiveForFeed).Select(c => c.Id)).ToListAsync();
                    var limitationsIdsTask = limitationsContext.LimitationFollowers
                        .Where(x => x.UserId == userId && x.DeleteDate == null)
                        .Select(c => c.Id).ToListAsync();

                    await Task.WhenAll(categoriesIdsTask, limitationsIdsTask);

                    var categories = categoriesIdsTask.Result;
                    var limitations = limitationsIdsTask.Result;

                    limitationsTask = limitations.Any()
                        ? limitationsContext.Stories
                            .Include(x => x.User)
                            .Include(x => x.Paragraphs)
                            .Include(x => x.Images)
                            .Include(x => x.StoryCategories)
                            .ThenInclude(x => x.Category)
                            .ThenInclude(x => x.ParentCategory)
                            .Where(x => x.IsPublished && !x.IsNoIndex && x.StoryLimitations.Any(sl => !sl.IsDeleted && limitations.Contains(sl.LimitationId)))
                            .Where(StoryWasNotLoaded(maxDate, 0))
                            .OrderByDescending(x => x.PublishDate)
                                .ThenByDescending(x => x.Id)
                            .AsNoTracking()
                            .Take(15)
                            .ToListAsync()
                        : Task.FromResult(new List<Story>());

                    categoriesTask = categories.Any()
                        ? categoriesContext.Stories
                            .Include(x => x.User)
                            .Include(x => x.Paragraphs)
                            .Include(x => x.Images)
                            .Include(x => x.StoryCategories)
                            .ThenInclude(x => x.Category)
                            .ThenInclude(x => x.ParentCategory)
                            .Where(x => x.IsPublished && !x.IsNoIndex && x.StoryCategories.Any(sc => !sc.IsDeleted && sc.IsPrimary && categories.Contains(sc.CategoryId)))
                            .Where(StoryWasNotLoaded(maxDate, 0))
                            .OrderByDescending(x => x.PublishDate)
                                .ThenByDescending(x => x.Id)
                            .AsNoTracking()
                            .Take(15)
                            .ToListAsync()
                        : Task.FromResult(new List<Story>());

                    await Task.WhenAll(categoriesTask, usersTask, limitationsTask);
                }

                var stories = usersTask.Result.Concat(limitationsTask.Result).Concat(categoriesTask.Result)
                    .DistinctBy(x => x.Id)
                    .OrderByDescending(x => x.PublishDate)
                        .ThenByDescending(x => x.Id)
                    .Take(count);
                var storiesModels = _mapper.Map<List<StoryCardModel>>(stories);
                models = new ContentImFollowingModel {Stories = storiesModels, Products = new List<ProductCardModel>()};
            }
            else
            {
                if(maxDate == null && lastId == null)
                {
                    models = await _redisWrapper.GetModelAsync(RedisKeys.HomeStoryFeed, () => GetHomeFeedFromDb(count), TimeSpan.FromHours(1));
                }
                else
                    models = await GetHomeFeedFromDb(count, maxDate);
            }

            return models;
        }

        private async Task<ContentImFollowingModel> GetHomeFeedFromDb(int count, DateTime? maxDate = null)
        {
            var stories = await _context.Stories
                .Include(x => x.User)
                .Include(x => x.Paragraphs)
                .Include(x => x.Images)
                .Include(x => x.StoryCategories)
                .ThenInclude(x => x.Category)
                .ThenInclude(x => x.ParentCategory)
                .Where(x => x.IsPublished && !x.IsNoIndex)
                .Where(StoryWasNotLoaded(maxDate, 0))
                .OrderByDescending(x => x.PublishDate)
                    .ThenByDescending(x => x.Id)
                .Take(count)
                .AsNoTracking()
                .ToListAsync();

            var storiesModel = _mapper.Map<List<StoryCardModel>>(stories);
            var models = new ContentImFollowingModel
            {
                Stories = storiesModel,
                Products = new List<ProductCardModel>()
            };
            return models;
        }

        private Expression<Func<Story, bool>> StoryWasNotLoaded(DateTime? maxDate, int lastId)
        {
            return story => !maxDate.HasValue || maxDate > story.PublishDate || (maxDate == story.PublishDate && lastId > story.Id);
        }

        const int MaxHeaderStoriesCount = 1;

        private async Task<HomeModel> GetHomeModelFromDbAsync()
        {
            const int maxEditorChoiceStoriesCount = 6;

            var featuredStoriesTable = _context.FeaturedStories.ToList();
            var featuredHeaderIds = featuredStoriesTable.Where(x => x.FeaturedType == FeaturedType.Header).Select(x=> x.StoryId).ToList();
            var model = new HomeModel();
            var headerStories = await _context.Stories
                .Where(x => x.IsPublished && !x.IsNoIndex && featuredHeaderIds.Contains(x.Id))
                .Include(x => x.StoryCategories)
                .ThenInclude(x => x.Category)
                .ThenInclude(x => x.ParentCategory)
                .Include(x => x.Images)
                .Include(x => x.User)
                .OrderByDescending(x => x.PublishDate)
                .Take(MaxHeaderStoriesCount)
                .AsNoTracking()
                .ToListAsync();

            var alreadyFetchedIds = headerStories.Select(x => x.Id).ToList();
            if (headerStories.Count < MaxHeaderStoriesCount)
            {
                headerStories.AddRange(await _context.Stories
                    .Where(x => x.IsPublished && !x.IsNoIndex && !alreadyFetchedIds.Contains(x.Id))
                    .Include(x => x.StoryCategories)
                    .ThenInclude(x => x.Category)
                    .ThenInclude(x => x.ParentCategory)
                    .Include(x => x.Images)
                    .Include(x => x.User)
                    .OrderByDescending(x => x.PublishDate)
                    .Take(MaxHeaderStoriesCount - alreadyFetchedIds.Count)
                    .AsNoTracking()
                    .ToListAsync());
                alreadyFetchedIds = headerStories.Select(x => x.Id).ToList();
            }


            model.HeaderStories = _mapper.Map<List<FeaturedStoryHeader>>(headerStories);

            var featuredIds = featuredStoriesTable.Where(x => x.FeaturedType != FeaturedType.Header).Select(x => x.StoryId).ToList();

            var featuredStories = await _context.Stories
                .Where(x => x.IsPublished && !x.IsNoIndex && !alreadyFetchedIds.Contains(x.Id) && featuredIds.Contains(x.Id))
                .Include(x => x.Paragraphs)
                .Include(x => x.StoryCategories)
                .ThenInclude(x => x.Category)
                .ThenInclude(x => x.ParentCategory)
                .Include(x => x.Paragraphs)
                .Include(x => x.Images)
                .Include(x => x.User)
                .OrderByDescending(x => x.PublishDate)
                .Take(maxEditorChoiceStoriesCount)
                .AsNoTracking()
                .ToListAsync();

            if (featuredStories.Count < maxEditorChoiceStoriesCount)
            {
                alreadyFetchedIds.AddRange(featuredStories.Select(x => x.Id).ToList());
                featuredStories.AddRange(await _context.Stories
                    .Where(x => x.IsPublished && !x.IsNoIndex && !alreadyFetchedIds.Contains(x.Id))
                    .Include(x => x.Paragraphs)
                    .Include(x => x.StoryCategories)
                    .ThenInclude(x => x.Category)
                    .ThenInclude(x => x.ParentCategory)
                    .Include(x => x.Paragraphs)
                    .Include(x => x.Images)
                    .Include(x => x.User)
                    .OrderByDescending(x => x.PublishDate)
                    .Take(maxEditorChoiceStoriesCount - featuredStories.Count)
                    .AsNoTracking()
                    .ToListAsync());
            }

            model.StoryCards = _mapper.Map<List<StoryCardModel>>(featuredStories);
            model.FeatureBlogPost = await GetFeaturedBlogPost();

            return model;
        }

        public async Task<NewUserHomeModel> GetNewUserModelAsync()
        {
            return await _redisWrapper.GetModelAsync(RedisKeys.NewUserHomeModel, GetNewUserHomeModelFromDbAsync, TimeSpan.FromHours(3));
        }

        private async Task<PostModel> GetFeaturedBlogPost()
        {
            var featuredBlogPost = await _context.Posts.Where(x => x.PublishDate != null)
                                                        .Include(x => x.Images)
                                                        .OrderBy(x => x.Order)
                                                        .ThenByDescending(x => x.PublishDate)
                                                        .FirstOrDefaultAsync();
            return _mapper.Map<PostModel>(featuredBlogPost);
        }

        private async Task<NewUserHomeModel> GetNewUserHomeModelFromDbAsync()
        {
            var model = new NewUserHomeModel();

            var headerStory = await _context.Stories
                .Where(x => x.IsPublished && !x.IsNoIndex && _context.FeaturedStories.Any(fs => fs.FeaturedType == FeaturedType.Header && fs.StoryId == x.Id))
                .Include(x => x.StoryCategories)
                .ThenInclude(x => x.Category)
                .ThenInclude(x => x.ParentCategory)
                .Include(x => x.Images)
                .Include(x => x.User)
                .OrderByDescending(x => x.PublishDate)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (headerStory == null)
            {
                headerStory = await _context.Stories
                    .Where(x => x.IsPublished && !x.IsNoIndex)
                    .Include(x => x.StoryCategories)
                    .ThenInclude(x => x.Category)
                    .ThenInclude(x => x.ParentCategory)
                    .Include(x => x.Images)
                    .Include(x => x.User)
                    .OrderByDescending(x => x.PublishDate)
                    .AsNoTracking()
                    .FirstAsync();
            }

            model.HeaderStory = _mapper.Map<FeaturedStoryHeader>(headerStory);
            const int maxFeaturedStories = 11;
            var featuredStories = await _context.Stories
                .Where(x => x.IsPublished && !x.IsNoIndex && model.HeaderStory.Id != x.Id && _context.FeaturedStories.Any(fs => (fs.FeaturedType == FeaturedType.Feed || fs.FeaturedType == FeaturedType.EditorPick) && fs.StoryId == x.Id))
                .Include(x => x.Paragraphs)
                .Include(x => x.StoryCategories)
                .ThenInclude(x => x.Category)
                .ThenInclude(x => x.ParentCategory)
                .Include(x => x.Paragraphs)
                .Include(x => x.Images)
                .Include(x => x.User)
                .OrderByDescending(x => x.PublishDate)
                .Take(maxFeaturedStories)
                .AsNoTracking()
                .ToListAsync();

            model.StoryCards = _mapper.Map<List<StoryCardModel>>(featuredStories);

            if (featuredStories.Count < maxFeaturedStories)
            {
                var alreadyFetchedIds = featuredStories.Select(x => x.Id).ToList();
                alreadyFetchedIds.Add(headerStory.Id);

                model.StoryCards.AddRange(await GetStoriesFromDb(maxFeaturedStories - featuredStories.Count, excludeIds: alreadyFetchedIds));
            }




            //var productsIds = await _context.Products
            //    .Where(x => x.IsPublished)
            //    .Select(x => x.Id)
            //    .OrderBy(x => Guid.NewGuid())
            //    .Take(2)
            //    .ToListAsync();

            //var products = await _context.Products
            //    .Include(x => x.Images)
            //    .Include(x => x.ProductCategories)
            //    .ThenInclude(x => x.Category)
            //    .ThenInclude(x => x.ParentCategory)
            //    .Where(x => productsIds.Contains(x.Id))
            //    .AsNoTracking()
            //    .ToListAsync();

            // Until there will be constant new products every day
            //var products = await _context.Products
            //    .Where(x => x.IsPublished)
            //    .Include(x => x.Images)
            //    .Include(x => x.ProductCategories)
            //    .ThenInclude(x => x.Category)
            //    .ThenInclude(x => x.ParentCategory)
            //    .OrderByDescending(x => x.InsertDate)
            //    .AsNoTracking()
            //    .Take(2)
            //    .ToListAsync();

            //model.ProductCards = _mapper.Map<List<ProductCardModel>>(products);
            //model.ProductCards.ForEach(x => x.IsNew = true);
            var categories = await _context.Categories.Where(x => x.ParentCategoryId == null && x.IsActiveForFeed).ToListAsync();
            categories = categories.OrderBy(x => Guid.NewGuid()).Take(4).ToList();
            model.FeaturedCategories = _mapper.Map<List<FeaturedCategory>>(categories);
            return model;
        }

        public async Task<List<StoryCardModel>> GetStoriesFromDb(int count, DateTime? maxDate = null, int? maxId = null, List<int> excludeIds = null)
        {
            excludeIds = excludeIds ?? new List<int>();

            var stories = await _context.Stories
                .Where(x => x.IsPublished && !x.IsNoIndex && !excludeIds.Contains(x.Id))
                .Where(StoryWasNotLoaded(maxDate, 0))
                .Include(x => x.Paragraphs)
                .Include(x => x.StoryCategories)
                .ThenInclude(x => x.Category)
                .ThenInclude(x => x.ParentCategory)
                .Include(x => x.Paragraphs)
                .Include(x => x.Images)
                .Include(x => x.User)
                .OrderByDescending(x => x.PublishDate)
                .ThenByDescending(x => x.Id)
                .Take(count)
                .AsNoTracking()
                .ToListAsync();
            return _mapper.Map<List<StoryCardModel>>(stories);
        }
    }
}