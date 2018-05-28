using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;
using Yooocan.Dal;
using Yooocan.Entities;
using Yooocan.Entities.ServiceProviders;
using Yooocan.Enums;
using Yooocan.Models;
using Humanizer;

namespace Yooocan.Logic
{
    public class AdminLogic : IAdminLogic
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _memoryCache;
        private readonly IDatabase _redisDatabase;
        private readonly IEmailLogic _emailLogic;

        public AdminLogic(ApplicationDbContext context, IMemoryCache memoryCache, IDatabase redisDatabase, IEmailLogic emailLogic)
        {
            _context = context;
            _memoryCache = memoryCache;
            _redisDatabase = redisDatabase;
            _emailLogic = emailLogic;
        }
        public DashboardModel GetDashboard()
        {
            var results = _memoryCache.GetOrCreate(nameof(GetDashboard), entry =>
            {
                var stories = _context.Stories.Count(x => x.IsPublished);
                var tipsArticles = _context.Stories.Count(x => x.IsPublished && x.Title.Contains("tips"));
                var productRecommendations = _context.Stories.Where(x => x.IsProductsReviewed && x.IsPublished).Count();
                var products = _context.Products.Count(x => x.IsPublished);
                var vendors = _context.Companies.Count(x => x.Products.Any());
                var users = _context.Users.Select(x => x.NormalizedEmail).Distinct().Count();
                var storyImages = _context.StoryImages.Count(x => x.Story.IsPublished && !x.IsDeleted);
                var storyVideos = _context.Stories.Count(x => x.IsPublished && x.YouTubeId != null);
                var productsVideos = _context.Products.Count(x => x.IsPublished && x.YouTubeId != null);
                var productImages = _context.ProductImages.Count(x => x.Product.IsPublished && !x.IsDeleted);
                var pendingStories = _context.Stories.Count(x => !x.IsPublished && !x.IsDeleted);
                var serviceProviders = _context.ServiceProviders.Count(x => x.IsPublished && !x.IsDeleted);
                var pendingServiceProviders = _context.ServiceProviders.Count(x => !x.IsPublished && !x.IsDeleted);
                var model = new DashboardModel
                            {
                                Stories = stories,
                                ProductRecommendations = productRecommendations,
                                TipsArticles = tipsArticles,
                                Products = products,
                                StoryImages = storyImages,
                                ProductImages = productImages,
                                Vendors = vendors,
                                Users = users,
                                ProductVideos = productsVideos,
                                StoryVideos = storyVideos,
                                PendingStories = pendingStories,
                                ServiceProviders = serviceProviders,
                                PendingServiceProviders = pendingServiceProviders
                            };

                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(15));
                entry.SetValue(model);
                entry.SetPriority(CacheItemPriority.High);
                return model;
            });

            return results;
        }

        public async Task<List<SetStoryProductsDashboardModel>> GetSetStoryProductsDataAsync()
        {
            var stories = await _context.Stories.Where(x => !x.IsDeleted && x.IsProductsReviewed)
                .OrderByDescending(x => x.Id)
                .Take(50)
                .Select(x => new SetStoryProductsDashboardModel
                             {
                                 Id = x.Id,
                                 Title = x.Title,
                             })
                .ToListAsync();

            return stories;

        }

        public async Task<Story> SetStoryServiceProvidersAsync(int storyId, string serviceProviderIds)
        {
            var story = await _context.Stories.Include(x => x.StoryServiceProviders).SingleOrDefaultAsync(x => x.Id == storyId && x.IsPublished && !x.IsDeleted);
            _context.StoryServiceProviders.RemoveRange(story.StoryServiceProviders);
            await _context.SaveChangesAsync();
            if (!string.IsNullOrWhiteSpace(serviceProviderIds))
            {
                story.StoryServiceProviders = serviceProviderIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => new StoryServiceProvider { ServiceProviderId = int.Parse(x.Trim()) })
                    .ToList();
            }
            story.LastUpdateDate = DateTime.UtcNow;

            var key = string.Format(RedisKeys.StoryModel, storyId);
            _redisDatabase.KeyDelete(key, CommandFlags.FireAndForget);
            await _context.SaveChangesAsync();
            return story;
        }

        public async Task SendStoryOfTheDayAsync(int storyId, string text, string storyUrl, string recipient = null)
        {
            var story = await _context.Stories
                .Include(x=> x.Paragraphs)
                .Include(x=> x.Images)
                .SingleAsync(x => x.Id == storyId);
            var emailUserData = await _context.Users.Where(x => x.EmailConfirmed && (x.Email == recipient || recipient == null))
                .Select(x => new EmailUserData
                             {
                                 Email = x.Email,
                                 UserId = x.Id,
                                 FirstName = x.FirstName,
                                 LastName = x.LastName,
                                 ProfileImageUrl = x.PictureUrl
                             }).ToListAsync();

            var primaryImage = story.Images
                .Where(x => !x.IsDeleted && (x.Type == ImageType.Primary || x.Type == ImageType.Header))
                .OrderByDescending(x => x.Type == ImageType.Header)
                .Select(x => x.CdnUrl ?? x.Url)
                .First();

            text = !string.IsNullOrWhiteSpace(text) ? text : story.Paragraphs.Where(x => !x.IsDeleted).OrderBy(x => x.Order).First().Text.Truncate(200);
            var storyData = new StoryOfTheDayData
                            {
                                StoryId = storyId,
                                StoryTitle = story.Title,
                                StoryUrl = storyUrl,
                                PrimaryImage = primaryImage,
                                StoryText = text
                            };

            await _emailLogic.SendStoryOfTheDayAsync(emailUserData, storyData);

            await _context.SaveChangesAsync();
        }
    }
}