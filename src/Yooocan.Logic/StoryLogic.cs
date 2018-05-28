using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using Yooocan.Dal;
using Yooocan.Entities;
using Yooocan.Logic.Messaging;
using Yooocan.Models;
using Yooocan.Models.New.Stories;
using Yooocan.Models.Products;
using Yooocan.Models.UploadStoryModels;

namespace Yooocan.Logic
{
    public class StoryLogic : IStoryLogic
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<StoryLogic> _logger;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        private readonly IDatabase _redisDatabase;
        private readonly IEmailLogic _emailLogic;
        private readonly HtmlSanitizer _htmlSanitizer;
        private readonly IServiceProvider _serviceProvider;

        public StoryLogic(ApplicationDbContext context, ILogger<StoryLogic> logger, IMapper mapper, IEmailSender emailSender, IDatabase redisDatabase, IEmailLogic emailLogic,
            HtmlSanitizer htmlSanitizer, IServiceProvider serviceProvider)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _emailSender = emailSender;
            _redisDatabase = redisDatabase;
            _emailLogic = emailLogic;
            _htmlSanitizer = htmlSanitizer;
            _serviceProvider = serviceProvider;
        }

        public async Task<StoryModel> GetStoryModelForPage(int storyId)
        {
            var key = GetStoryModelKey(storyId);
            try
            {
                var cacheArray = _redisDatabase.HashGet(key, new RedisValue[] { RedisKeys.Main, RedisKeys.Comments, RedisKeys.Likes, RedisKeys.Views });

                var mainCache = cacheArray[0];
                var commentsCache = cacheArray[1];
                var likesCache = cacheArray[2];
                var viewsCache = cacheArray[3];

                string newMainCache = null;
                string newCommentsCache = null;
                string newLikesCache = null;
                string newViewsCache = null;

                var fieldsToSet = new List<HashEntry>();

                if (mainCache.IsNull)
                {
                    var mainStory = await GetMainStoryAsync(storyId);
                    if (mainStory == null)
                        return null;
                    newMainCache = JsonConvert.SerializeObject(mainStory);
                    fieldsToSet.Add(new HashEntry(RedisKeys.Main, newMainCache));
                }

                if (commentsCache.IsNull)
                {
                    var comments = await GetStoryCommentsAsync(storyId);
                    newCommentsCache = JsonConvert.SerializeObject(comments);
                    fieldsToSet.Add(new HashEntry(RedisKeys.Comments, newCommentsCache));
                }

                if (likesCache.IsNull || viewsCache.IsNull)
                {
                    var data = await _context.Stories
                        .Where(x => x.Id == storyId)
                        .Select(x => new
                        {
                            x.ViewsCount,
                            x.LikesCount
                        })
                        .SingleAsync();

                    newLikesCache = data.LikesCount.ToString();
                    newViewsCache = data.ViewsCount.ToString();

                    if (likesCache.IsNull)
                    {
                        fieldsToSet.Add(new HashEntry(RedisKeys.Likes, newLikesCache));
                    }
                    if (viewsCache.IsNull)
                    {
                        fieldsToSet.Add(new HashEntry(RedisKeys.Views, newViewsCache));
                    }
                }
                
                _redisDatabase.HashSet(key, fieldsToSet.ToArray());
                if (mainCache.IsNull)
                    _redisDatabase.KeyExpire(key, TimeSpan.FromHours(12));

                //todo: not to deserialize when getting from DB, we already have it in that way.
                var storyModel = JsonConvert.DeserializeObject<StoryModel>(newMainCache ?? mainCache);
                var commentsModel = JsonConvert.DeserializeObject<List<StoryCommentModel>>(newCommentsCache ?? commentsCache);
                storyModel.Comments = commentsModel;
                storyModel.ViewsCount = int.Parse(viewsCache.HasValue ? viewsCache.ToString() : (newViewsCache ?? "0"));
                storyModel.LikesCount = int.Parse(likesCache.HasValue ? likesCache.ToString() : (newLikesCache ?? "0"));
                return storyModel;
            }
            catch (Exception exception)
            {
                _logger.LogError(12399, exception, "Error getting story {id} from Redis", storyId);
                var main = await GetMainStoryAsync(storyId);
                var comments = await GetStoryCommentsAsync(storyId);
                main.Comments = comments;
                return main;
            }
        }

        private async Task<StoryModel> GetMainStoryAsync(int storyId)
        {
            var story = await _context.Stories
                .Include(x => x.StoryCategories)
                .ThenInclude(x => x.Category)
                .ThenInclude(x => x.ParentCategory)
                .Include(x => x.StoryLimitations)
                    .ThenInclude(x => x.Limitation)
                .Include(x => x.User)
                .Include(x => x.Paragraphs)
                .Include(x => x.Images)
                .SingleOrDefaultAsync(x => x.Id == storyId && !x.IsDeleted);
            if (story == null)
                return null;

            story.Images = story.Images.Where(x => !x.IsDeleted).ToList();
            story.Paragraphs = story.Paragraphs.Where(x => !x.IsDeleted).ToList();

            var results = _mapper.Map<StoryModel>(story);

            var primaryCategory = story.StoryCategories
                .Where(x => !x.IsDeleted && x.IsPrimary)
                .Select(x => x.CategoryId)
                .FirstOrDefault();
            var secondaryCategories = story.StoryCategories
                .Where(x => !x.IsDeleted && !x.IsPrimary)
                .Select(x => x.CategoryId)
                .ToList();
            var relatedStories = await GetRelatedStoriesAsync(storyId, primaryCategory, secondaryCategories);
            var relatedStoriesModel = _mapper.Map<List<StoryCardModel>>(relatedStories);
            for (var i = 0; i < relatedStoriesModel.Count; i++)
            {
                relatedStoriesModel[i].IsDarkTheme = i % 2 == 0;
            }

            results.RelatedStories = relatedStoriesModel;
            var relatedProducts = await GetRelatedProductsAsync(storyId, primaryCategory, secondaryCategories);
            results.RelatedProducts = _mapper.Map<List<ProductCardModel>>(relatedProducts);
            results.RelatedServiceProviders = await GetRelatedServiceProvidersAsync(storyId);
            return results;
        }

        private async Task<List<StoryCommentModel>> GetStoryCommentsAsync(int storyId)
        {
            var comments = await _context.StoryComments
                .Include(x => x.User)
                .AsNoTracking()
                .Where(x => x.StoryId == storyId && !x.IsDeleted)
                .OrderByDescending(x => x.InsertDate)
                .ToListAsync();

            var commentsModel = _mapper.Map<List<StoryCommentModel>>(comments);
            return commentsModel;
        }


        public async Task<List<ProductCardModel>> GetRelatedProductsAsync(int storyId, int primaryCategoryId, List<int> secenodaryCategoriesIds,
            int maxRelatedProducts = 10)
        {
            var productsIds = await _context.StoryProducts
                .Where(x => x.StoryId == storyId)
                .OrderBy(x => x.Order)
                .Select(x => x.ProductId)
                .Take(maxRelatedProducts)
                .ToListAsync();

            List<Product> products;
            if (productsIds.Any())
            {
                products = await _context.Products
                    .Where(x => x.IsPublished && productsIds.Contains(x.Id))
                    .Include(x => x.Images)
                    .Include(x => x.Company)
                    .Include(x => x.ProductCategories)
                    .ThenInclude(x => x.Category)
                    .ThenInclude(x => x.ParentCategory)
                    .ToListAsync();

                //Order by [Order]
                products = (from id in productsIds
                            join product in products
                            on id equals product.Id
                            select product).ToList();
            }
            else
            {
                products = new List<Product>();
            }

            if (products.Count < maxRelatedProducts)
            {
                var newProducts = (await _context.Products
                        .Where(x => x.IsPublished &&
                                    !productsIds.Contains(x.Id) &&
                                    x.ProductCategories.Any(ps => ps.Category.Id == primaryCategoryId))
                        .Include(x => x.Images)
                        .Include(x => x.Company)
                        .Include(x => x.ProductCategories)
                        .ThenInclude(x => x.Category)
                        .ThenInclude(x => x.ParentCategory)
                        .OrderByDescending(x => x.Id)
                        .Take(100)
                        .ToListAsync())
                    .OrderBy(x => Guid.NewGuid())
                    .Take(maxRelatedProducts - products.Count)
                    .ToList();
                products.AddRange(newProducts);
                if (products.Count < maxRelatedProducts)
                {
                    productsIds.AddRange(newProducts.Select(x => x.Id));

                    var secondaryCategoryProducts = (await _context.Products
                            .Where(x => x.IsPublished &&
                                        !productsIds.Contains(x.Id) &&
                                        x.ProductCategories.Any(cs => secenodaryCategoriesIds.Contains(cs.Category.Id)))
                            .Include(x => x.Images)
                            .Include(x => x.Company)
                            .Include(x => x.ProductCategories)
                            .ThenInclude(x => x.Category)
                            .ThenInclude(x => x.ParentCategory)
                            .OrderByDescending(x => x.Id)
                            .Take(100)
                            .ToListAsync()
                        ).OrderBy(x => Guid.NewGuid())
                        .Take(maxRelatedProducts - productsIds.Count)
                        .ToList();
                    products.AddRange(secondaryCategoryProducts);
                }
            }

            var models = _mapper.Map<List<ProductCardModel>>(products);
            return models;
        }

        public async Task<List<RelatedServiceProviderModel>> GetRelatedServiceProvidersAsync(int storyId)
        {
            const int maxServiceProviders = 3;
            var serviceProviders = (await _context.StoryServiceProviders
                    .Where(x => x.StoryId == storyId)
                    .Include(x => x.ServiceProvider)
                    .ThenInclude(x => x.Images)
                    .OrderBy(x => x.Order)
                    .Take(maxServiceProviders)
                    .ToListAsync())
                .Select(x => x.ServiceProvider)
                .ToList();
            //var alreadyPickedServiceProviders = serviceProviders.Select(x => x.Id).ToList();
            //if (serviceProviders.Count < maxServiceProviders)
            //{

            //    var relatedCategories = await _context.StoryCategories.Where(x => x.StoryId == storyId).ToArrayAsync();
            //    var primaryCategoryId = relatedCategories.FirstOrDefault(x => x.IsPrimary)?.CategoryId;
            //    var relatedCategoriesIds = await _context.StoryCategories.Where(x => x.StoryId == storyId && !x.IsPrimary).Select(x => x.CategoryId).ToArrayAsync();

            //    var mainCategoryServiceProviders = await _context.ServiceProviders
            //        .Where(x => x.IsPublished &&
            //        x.ServiceProviderCategories.Any(cs => primaryCategoryId == cs.Category.Id) &&
            //                    !alreadyPickedServiceProviders.Contains(x.Id))
            //        .Include(x => x.Images)
            //        .Take(100)
            //        .ToListAsync();
            //    mainCategoryServiceProviders = mainCategoryServiceProviders.OrderBy(x => Guid.NewGuid()).Take(maxServiceProviders - serviceProviders.Count).ToList();
            //    serviceProviders.AddRange(mainCategoryServiceProviders);
            //    alreadyPickedServiceProviders.AddRange(mainCategoryServiceProviders.Select(x => x.Id));

            //    if (serviceProviders.Count < maxServiceProviders)
            //    {
            //        var otherCategoriesServiceProviders = await _context.ServiceProviders
            //            .Where(x => x.IsPublished &&
            //                        x.ServiceProviderCategories.Any(cs => relatedCategoriesIds.Contains(cs.Category.Id)) &&
            //                        !alreadyPickedServiceProviders.Contains(x.Id))
            //            .Include(x => x.Images)
            //            .Take(100)
            //            .ToListAsync();
            //        otherCategoriesServiceProviders =
            //            otherCategoriesServiceProviders.OrderBy(x => Guid.NewGuid()).Take(maxServiceProviders - serviceProviders.Count).ToList();
            //        serviceProviders.AddRange(otherCategoriesServiceProviders);
            //        alreadyPickedServiceProviders.AddRange(otherCategoriesServiceProviders.Select(x => x.Id));
            //    }

            //    if (serviceProviders.Count < maxServiceProviders)
            //    {
            //        var randomServiceProviders = await _context.ServiceProviders
            //            .Where(x => x.IsPublished && !alreadyPickedServiceProviders.Contains(x.Id))
            //            .Include(x => x.Images)
            //            .Take(100)
            //            .ToListAsync();
            //        randomServiceProviders = randomServiceProviders.OrderBy(x => Guid.NewGuid()).Take(maxServiceProviders - serviceProviders.Count).ToList();
            //        serviceProviders.AddRange(randomServiceProviders);
            //    }
            //}

            var models = _mapper.Map<List<RelatedServiceProviderModel>>(serviceProviders);

            return models;
        }

        public async Task<List<Story>> GetRelatedStoriesAsync(int storyId, int primaryCategoryId, List<int> secenodaryCategoriesIds)
        {
            const int maxRelatedStories = 3;

            var stories = await _context.Stories
                .Include(x => x.Paragraphs)
                .Include(x => x.User)
                .Include(x => x.Images)
                .Include(x => x.StoryCategories)
                .ThenInclude(x => x.Category)
                .ThenInclude(x => x.ParentCategory)
                .Where(x => x.Id != storyId && x.IsPublished && !x.IsNoIndex &&
                            x.StoryCategories.Where(sc => sc.IsPrimary && !sc.IsDeleted)
                                .Select(sc => sc.Category)
                                .Any(sc => sc.Id == primaryCategoryId))
                .OrderByDescending(x => x.Id)
                .Take(maxRelatedStories)
                .AsNoTracking()
                .ToListAsync();

            if (stories.Count == maxRelatedStories)
            {
                return stories;
            }

            if (secenodaryCategoriesIds.Any())
            {
                var alreadyFetchedStoriesIds = stories.Select(x => x.Id).ToList();
                var storiesFromOtherCategories = await _context.Stories
                    .Include(x => x.Paragraphs)
                    .Include(x => x.User)
                    .Include(x => x.Images)
                    .Include(x => x.StoryCategories)
                    .ThenInclude(x => x.Category)
                    .ThenInclude(x => x.ParentCategory)
                    .Where(x => x.Id != storyId &&
                                x.IsPublished &&
                                !x.IsNoIndex &&
                                !alreadyFetchedStoriesIds.Contains(x.Id) &&
                                x.StoryCategories
                                    .Select(cs => cs.Category)
                                    .Any(cs => secenodaryCategoriesIds.Contains(cs.Id)))
                    .OrderByDescending(x => x.Id)
                    .Take(maxRelatedStories - stories.Count)
                    .AsNoTracking()
                    .ToListAsync();

                stories.AddRange(storiesFromOtherCategories);
            }
            if (stories.Count < maxRelatedStories)
            {
                var alreadyFetchedStoriesIds = stories.Select(x => x.Id).ToList();

                var randomStories = await _context.Stories
                    .Include(x => x.Paragraphs)
                    .Include(x => x.User)
                    .Include(x => x.Images)
                    .Include(x => x.StoryCategories)
                    .ThenInclude(x => x.Category)
                    .ThenInclude(x => x.ParentCategory)
                    .Where(x => x.Id != storyId &&
                                !x.IsNoIndex &&
                                x.IsPublished &&
                                !alreadyFetchedStoriesIds.Contains(x.Id))
                    .OrderByDescending(x => x.Id)
                    .Take(maxRelatedStories - stories.Count)
                    .AsNoTracking()
                    .ToListAsync();

                stories.AddRange(randomStories);
            }

            return stories;
        }

        public async Task<Story> UploadStoryAsync(UploadStoryModel model, string authorId)
        {
            foreach (var paragraph in model.Paragraphs)
            {
                paragraph.Text = _htmlSanitizer.SanitizeStory(paragraph.Text);
            }

            var story = _mapper.Map<Story>(model);
            //todo: add this to the mapping.
            story.Title = story.Title.Trim();
            story.LastUpdateDate = DateTime.UtcNow;
            story.UserId = authorId;

            _context.Stories.Add(story);

            if (story.Images.Any())
            {
                var usedImages = await _context.FileUploads.Where(x => model.Images.Contains(x.Url) && !x.IsUsed).ToListAsync();
                foreach (var usedImage in usedImages)
                {
                    usedImage.IsUsed = true;
                }
            }

            if (!string.IsNullOrEmpty(model.UserInstagramUserName) || !string.IsNullOrEmpty(model.UserFacebookPageUrl))
            {
                var user = _context.Users.Single(x => x.Id == authorId);
                user.InstagramUserName = model.UserInstagramUserName;
                user.FacebookPageUrl = model.UserFacebookPageUrl;
            }

            await _context.SaveChangesAsync();
            var emailSendTasks = new List<Task>();

            if (!string.IsNullOrWhiteSpace(story.UsedProducts))
            {
                emailSendTasks.Add(_emailSender.SendEmailAsync(null, "moshe@yoocantech.com;jessica@yoocantech.com", "New story on yooocan with suggested products!",
                    $"https://yoocanfind.com/Story/{story.Id}", "NewStory-suggested products", null));
            }

            if (!story.IsPublished)
            {
                emailSendTasks.Add(_emailSender.SendEmailAsync(null, "jessica@yoocantech.com", "New story on yooocan awaits your approval!",
                    $"https://yoocanfind.com/Story/{story.Id}", "NewStory", null));
            }

            if (!string.IsNullOrWhiteSpace(model.SuggestedCategory))
                emailSendTasks.Add(_emailSender.SendEmailAsync(null, "jessica@yoocantech.com;yoav@yoocantech.com", $"New category \"{model.SuggestedCategory}\" was suggested, for story {story.Title}",
                    $"https://yoocanfind.com/Story/{story.Id}", "SuggestedCategory", null));

            await Task.WhenAll(emailSendTasks);

            return story;
        }

        public async Task<Story> EditStoryAsync(UploadStoryModel model)
        {
            foreach (var paragraph in model.Paragraphs)
            {
                paragraph.Text = _htmlSanitizer.SanitizeStory(paragraph.Text);
            }

            // Get rid of old data on the story.
            var dbStory = _context.Stories
                .Include(x => x.User)
                .Include(x => x.Images)
                .Include(x => x.StoryCategories)
                .Include(x => x.StoryLimitations)
                .Include(x => x.Paragraphs)
                .Single(x => x.Id == model.Id);

            foreach (var storyImage in dbStory.Images.Where(x => !model.Images.Contains(x.Url) && !model.Images.Contains(x.CdnUrl) && 
                                                                 model.HeaderImageUrl != x.CdnUrl && model.HeaderImageUrl != x.Url))
            {
                storyImage.IsDeleted = true;
            }

            var unchoosableCategoriesIds = await _context.Categories.Where(x => !x.IsChoosableForStory).Select(x => x.Id).ToListAsync();
            _context.RemoveRange(dbStory.StoryCategories.Where(x => !unchoosableCategoriesIds.Contains(x.CategoryId)));
            _context.RemoveRange(dbStory.StoryLimitations);
            // Hard delete already deleted paragraphs, we are saving only 1 revision back
            _context.RemoveRange(dbStory.Paragraphs.Where(x => x.IsDeleted));

            // Soft delete Previous version.
            foreach (var paragraph in dbStory.Paragraphs.Where(x => !x.IsDeleted))
            {
                paragraph.IsDeleted = true;
            }

            _context.SaveChanges();

            var story = _mapper.Map<Story>(model);

            // Add new paragraphs
            dbStory.Paragraphs.AddRange(story.Paragraphs);

            story.UsedProducts = story.UsedProducts?.Trim();
            if (story.Images.Any())
            {
                var usedImages = await _context.FileUploads.Where(x => model.Images.Contains(x.Url)).ToListAsync();
                foreach (var usedImage in usedImages)
                {
                    usedImage.IsUsed = true;
                }
            }

            var newImages = story.Images.Where(x => !dbStory.Images.Select(dbImage => dbImage.CdnUrl ?? dbImage.Url).Contains(x.CdnUrl ?? x.Url));
            dbStory.Images.AddRange(newImages);
            // Take the new order and type(primary) from the client
            foreach (var storyImage in story.Images)
            {
                var dbImage = dbStory.Images.SingleOrDefault(x => x.Url == storyImage.Url && !x.IsDeleted);
                if (dbImage == null)
                    continue;

                dbImage.Order = storyImage.Order;
                dbImage.Type = storyImage.Type;
            }

            dbStory.LastUpdateDate = DateTime.UtcNow;
            dbStory.YouTubeId = story.YouTubeId;
            dbStory.Title = story.Title.Trim();
            dbStory.IsProductsReviewed = story.IsProductsReviewed;

            if (!string.IsNullOrWhiteSpace(story.UsedProducts) && story.UsedProducts != dbStory.UsedProducts)
            {
                await _emailSender.SendEmailAsync(null, "moshe@yoocantech.com", "Edited story on yooocan with suggested products!",
                    $"https://yoocanfind.com/Story/{story.Id}", "EditStory-suggested products", null);
            }            
            dbStory.ActivityLocation = story.ActivityLocation;
            dbStory.GooglePlaceId = story.GooglePlaceId;
            dbStory.Latitude = story.Latitude;
            dbStory.Longitude = story.Longitude;
            dbStory.Country = story.Country;
            dbStory.City = story.City;
            dbStory.PostalCode = story.PostalCode;
            dbStory.State = story.State;
            dbStory.StreetName = story.StreetName;
            dbStory.StreetNumber = story.StreetNumber;

            dbStory.Paragraphs.AddRange(story.Paragraphs);
            dbStory.StoryCategories.AddRange(story.StoryCategories);
            dbStory.StoryLimitations = story.StoryLimitations;

            if (dbStory.User.InstagramUserName != model.UserInstagramUserName)
            {
                dbStory.User.InstagramUserName = model.UserInstagramUserName;
            }

            if (dbStory.User.FacebookPageUrl != model.UserFacebookPageUrl)
            {
                dbStory.User.FacebookPageUrl = model.UserFacebookPageUrl;
            }

            await _context.SaveChangesAsync();

            InvalidateCache(dbStory.Id);

            if (!string.IsNullOrWhiteSpace(model.SuggestedCategory))
                await _emailSender.SendEmailAsync(null, "jessica@yoocantech.com;yoav@yoocantech.com", $"New category \"{model.SuggestedCategory}\" was suggested, for story {story.Title}",
                    $"https://yoocanfind.com/Story/{story.Id}", "SuggestedCategory-edit", null);

            return story;
        }

        public async Task ApproveStoryAsync(int storyId, string storyUrl)
        {
            var story = _context.Stories
                .Include(x => x.User)
                .Include(x => x.StoryCategories)
                    .ThenInclude(x => x.Category)
                        .ThenInclude(x => x.ParentCategory)
                .Include(x => x.StoryLimitations)
                .Single(x => x.Id == storyId);

            if (story.PublishDate == null)
            {
                var authorNotificationId = $"YourStoryWasPublished_{storyId}";
                var wasAuthorNotified = _context.NotificationLogs
                    .Any(x => x.UserId == story.UserId && x.NotificationId == authorNotificationId);

                if (!wasAuthorNotified && story.User.EmailConfirmed)
                {
                    var userData = new EmailUserData
                    {
                        Email = story.User.Email,
                        UserId = story.User.Id,
                        FirstName = story.User.FirstName,
                        LastName = story.User.LastName,
                        ProfileImageUrl = story.User.PictureUrl
                    };

                    await _emailLogic.SendYourStoryWasPublishedEmailAsync(userData, storyUrl, story.Title, storyId);
                }

                var followerNotificationId = $"Your followed published a story {storyId}";
                var followersEmails = await _context.Users.FromSql(@"SELECT DISTINCT U.*
FROM Followers F 
INNER JOIN Users U 
    ON F.FollowerUserId = U.Id
WHERE F.FollowedUserId = {0}
AND U.EmailConfirmed = 1
AND NOT EXISTS (SELECT 1 
                FROM   NotificationLogs NL 
                WHERE  NL.NotificationId = {1} 
                AND    NL.UserId = U.Id)", story.User.Id, followerNotificationId).ToListAsync();

                var emailUserData = followersEmails.Select(x => new EmailUserData
                {
                    Email = x.Email,
                    UserId = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    ProfileImageUrl = x.PictureUrl
                }).ToList();
                var storyData = new FollowingPublishedStoryUserData
                {
                    AuthorFirstName = story.User.FirstName,
                    AuthorLastName = story.User.LastName,
                    StoryId = storyId,
                    StoryTitle = story.Title,
                    StoryUrl = storyUrl,
                    AuhtorProfileImageUrl = story.User.PictureUrl
                };

                await _emailLogic.SendFollowingPublishedStoryEmailAsync(emailUserData, storyData);
                await _redisDatabase.KeyDeleteAsync(RedisKeys.HomeStoryFeed, CommandFlags.FireAndForget);

                //await NotifyCategoryAndLimitationFollowersAsync(story, storyUrl);
                
                story.PublishDate = DateTime.UtcNow;
            }
            story.IsPublished = true;
            story.LastUpdateDate = DateTime.UtcNow;
            _context.SaveChanges();
        }

        private async Task NotifyCategoryAndLimitationFollowersAsync(Story story, string storyUrl)
        {
            var limitationsIdsString = $"{string.Join(",", story.StoryLimitations.Select(x => x.LimitationId).ToList())}";

            var categoriesIds = story.StoryCategories.Select(x => x.CategoryId).ToList();
            var parentCategoriesIds = story.StoryCategories.Where(x => x.Category.ParentCategoryId != null)
                                                                                .Select(x => x.Category.ParentCategoryId.Value)
                                                                                .Distinct()
                                                                                .ToList();
            var parentCategoriesIdsString = $"{string.Join(",", parentCategoriesIds)}";
            const string categoryFollowNotificationIdPrefix = "FollowedCategoryStoryPublished";

            var categoryFollowingUsersToNotify = await _context.Users.FromSql($@"SELECT DISTINCT U.*
FROM Users U 
LEFT OUTER JOIN LimitationFollowers LF
    ON LF.UserId = U.Id
LEFT OUTER JOIN CategoryFollowers CF
    ON CF.UserId = U.Id
INNER JOIN Categories C
    ON CF.CategoryId = C.Id
WHERE (
    (LF.LimitationId in ({limitationsIdsString}) AND LF.DeleteDate IS NULL)
    OR 
    (CF.CategoryId in ({parentCategoriesIdsString}) AND CF.DeleteDate IS NULL)
)
AND U.EmailConfirmed = 1
AND U.Id <> {{0}}
AND NOT EXISTS (SELECT 1 
                FROM   NotificationLogs NL 
                WHERE  NL.NotificationId like '{categoryFollowNotificationIdPrefix}%' 
                AND    NL.InsertDate >= DATEADD(day, -1, GETUTCDATE())
                AND    NL.UserId = U.Id)", story.User.Id).ToListAsync();

            var emailUserData = categoryFollowingUsersToNotify.Select(x => new EmailUserData
            {
                Email = x.Email,
                UserId = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                ProfileImageUrl = x.PictureUrl
            }).ToList();
            var storyData = new FollowingPublishedStoryUserData
            {
                AuthorFirstName = story.User.FirstName,
                AuthorLastName = story.User.LastName,
                StoryId = story.Id,
                StoryTitle = story.Title,
                StoryUrl = storyUrl,
                AuhtorProfileImageUrl = story.User.PictureUrl
            };
        }

        public async Task ToggleDeleteStoryAsync(int storyId)
        {
            var story = await _context.Stories.SingleAsync(x => x.Id == storyId);
            story.IsDeleted = !story.IsDeleted;
            story.IsPublished = false;
            story.LastUpdateDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            InvalidateCache(storyId);
        }

        public async Task<List<Story>> GetUserStoriesAsync(string userId)
        {
            return await _context.Stories.Where(x => x.UserId == userId && !x.IsDeleted)
                .Include(x => x.User)
                .Include(x => x.Images)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task SetRelatedProductsAsync(int storyId, IEnumerable<SetStoryProductModel> products)
        {
            var story = await _context.Stories
                .Include(x => x.StoryProducts)
                .SingleAsync(x => x.Id == storyId);
            _context.RemoveRange(story.StoryProducts);

            RemoveStoryProductsFromCache(story.StoryProducts);

            await _context.SaveChangesAsync();
            story.StoryProducts = products.Distinct().Select((p, order) => new StoryProduct
            {
                ProductId = p.Id,
                IsUsedInStory = p.IsUsedInStory,
                StoryId = storyId,
                Order = order
            }).ToList();
            InvalidateCache(storyId);            

            RemoveStoryProductsFromCache(story.StoryProducts);

            await _context.SaveChangesAsync();
        }

        public async Task<PreviewStoryModel> PreviewStoryAsync(UploadStoryModel model, string userId)
        {
            var user = await _context.Users.SingleAsync(x => x.Id == userId);

            var viewModel = _mapper.Map<PreviewStoryModel>(model);
            viewModel.AuthorName = $"{user.FirstName} {user.LastName}";
            viewModel.AuthorAboutMe = user.AboutMe;
            viewModel.AuthorProfileUrl = user.PictureUrl;

            return viewModel;
        }

        public async Task LikeAsync(int storyId, string userId)
        {
            var alreadyLiked = await _context.StoryLikes.AnyAsync(x => x.StoryId == storyId && x.UserId == userId && !x.IsDeleted);
            if (alreadyLiked)
            {
                _logger.LogWarning("Trying to like already liked {story} {user}", storyId, userId);
                return;
            }

            _context.StoryLikes.Add(new StoryLike
            {
                UserId = userId,
                StoryId = storyId
            });
            _redisDatabase.HashIncrement(GetStoryModelKey(storyId), RedisKeys.Likes, flags: CommandFlags.FireAndForget);
            await _context.Database.ExecuteSqlCommandAsync($"UPDATE Stories SET LikesCount = LikesCount+1 WHERE Id ={storyId}");
            await _context.SaveChangesAsync();
        }

        public async Task UnLikeAsync(int storyId, string userId)
        {
            var storyLikes = await _context.StoryLikes.Where(x => x.StoryId == storyId && !x.IsDeleted).ToListAsync();

            foreach (var storyLike in storyLikes)
            {
                storyLike.IsDeleted = true;
                storyLike.DeleteDate = DateTime.UtcNow;
            }

            if (storyLikes.Any())
            {
                _redisDatabase.HashDecrement(GetStoryModelKey(storyId), RedisKeys.Likes, flags: CommandFlags.FireAndForget);
                await _context.Database.ExecuteSqlCommandAsync($"UPDATE Stories SET LikesCount = LikesCount-1 WHERE Id ={storyId}");
                await _context.SaveChangesAsync();
            }
        }

        private void RemoveStoryProductsFromCache(IEnumerable<StoryProduct> productIds)
        {
            foreach (var productId in productIds.Select(x => x.ProductId))
            {
                var key = string.Format(RedisKeys.ProductModel, productId);
                _redisDatabase.KeyDelete(key, CommandFlags.FireAndForget);
            }
        }

        private string GetStoryModelKey(int storyId)
        {
            var key = string.Format(RedisKeys.StoryModel, storyId);
            return key;
        }

        private void InvalidateCache(int storyId)
        {
            var key = GetStoryModelKey(storyId);
            _redisDatabase.HashDelete(key, RedisKeys.Main, CommandFlags.FireAndForget);
        }
    }
}