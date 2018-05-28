using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Yooocan.Dal;
using Yooocan.Entities.ServiceProviders;
using Yooocan.Logic.Messaging;
using Yooocan.Models.ServiceProviders;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Yooocan.Entities;
using Yooocan.Logic.Recaptchas;
using Yooocan.Logic.Extensions;
using Yooocan.Models;

namespace Yooocan.Logic
{
    public class ServiceProviderLogic : IServiceProviderLogic
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ServiceProviderLogic> _logger;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        private readonly IDatabase _redisDatabase;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IRecaptchaApi _recaptcha;

        public ServiceProviderLogic(ApplicationDbContext context, ILogger<ServiceProviderLogic> logger, 
            IMapper mapper, IEmailSender emailSender, IDatabase redisDatabase, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            IRecaptchaApi recaptcha)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _emailSender = emailSender;
            _redisDatabase = redisDatabase;
            _userManager = userManager;
            _signInManager = signInManager;
            _recaptcha = recaptcha;
        }

        public async Task<ServiceProvider> CreateAsync(CreateServiceProviderModel model)
        {
            var serviceProvider = _mapper.Map<ServiceProvider>(model);
            serviceProvider.Name = serviceProvider.Name.Trim();
            serviceProvider.Activities = serviceProvider.Activities.Where(x => !string.IsNullOrWhiteSpace(x.Name)).ToList();
            _context.ServiceProviders.Add(serviceProvider);


            var imagesToMarkAsUsed = new List<string>(model.Images)
                                     {
                                         model.HeaderImageUrl,
                                         model.LogoUrl
                                     };
            if (imagesToMarkAsUsed.Any())
            {
                var usedImages = await _context.FileUploads.Where(x => imagesToMarkAsUsed.Contains(x.Url)).ToListAsync();
                foreach (var usedImage in usedImages)
                {
                    usedImage.IsUsed = true;
                }
            }

            await _context.SaveChangesAsync();
            var user = await _userManager.Users.SingleAsync(x => x.Id == model.UserId);
            await _userManager.AddClaimAsync(user, new Claim("serviceProvider", serviceProvider.Id.ToString()));
            await _signInManager.RefreshSignInAsync(user);

            await _emailSender.SendEmailAsync(null, "yoav@yoocantech.com;moshe@yoocantech.com;jessica@yoocantech.com", $"New service provider registered {serviceProvider.Name}",
                    $"https://yoocanfind.com/ServiceProvider/{serviceProvider.Id}/{serviceProvider.Name.ToCanonical()}", "New Service Provider", null);

            if (!string.IsNullOrWhiteSpace(model.SuggestedCategory))
                await _emailSender.SendEmailAsync(null, "jessica@yoocantech.com;yoav@yoocantech.com", $"New category \"{model.SuggestedCategory}\" was suggested, for service provider {serviceProvider.Name}",
                    $"https://yoocanfind.com/ServiceProvider/{serviceProvider.Id}/{serviceProvider.Name.ToCanonical()}", "SuggestedCategory", null);

            return serviceProvider;
        }

        public async Task<ServiceProviderIndexModel> GetModelAsync(int id, string userId)
        {
            var redisKey = string.Format(RedisKeys.ServiceProviderModel, id);
            ServiceProviderIndexModel model;
            RedisValue redisValue;
            try
            {
                redisValue = _redisDatabase.StringGet(redisKey);
            }
            catch (Exception e)
            {
                _logger.LogError(324322, e, "Error when trying to get cache from Redis for {resource}", redisKey);
                model = await GetModelFromDbAsync(id);

                if (userId != null)
                    model.IsFollowed = await _context.ServiceProviderFollowers.AnyAsync(x => x.UserId == userId && x.ServiceProviderId == id);

                return model;
            }
            if (!redisValue.IsNull)
            {
                model = JsonConvert.DeserializeObject<ServiceProviderIndexModel>(redisValue.ToString());
            }
            else
            {
                model = await GetModelFromDbAsync(id);
                if (model == null)
                    return null;

                var serializedModel = JsonConvert.SerializeObject(model);
                try
                {
                    _redisDatabase.StringSet(redisKey, serializedModel, TimeSpan.FromHours(5));
                }
                catch (Exception e)
                {
                    _logger.LogError(391323, e, "Error when trying to set cache in Redis for {resource}", redisKey);
                }
            }

            if (userId != null)
                model.IsFollowed = await _context.ServiceProviderFollowers.AnyAsync(x => x.UserId == userId && x.ServiceProviderId == id);

            return model;
        }

        private async Task<ServiceProviderIndexModel> GetModelFromDbAsync(int id)
        {
            var serviceProvider = await _context.ServiceProviders
                .Include(x => x.Images)
                .Include(x => x.ServiceProviderCategories)
                    .ThenInclude(x => x.Category)
                .Include(x => x.Videos)
                .Include(x => x.Activities)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (serviceProvider == null)
                return null;

            serviceProvider.Activities = serviceProvider.Activities.Where(x => !x.IsDeleted).OrderBy(x => x.Order).ToList();
            var model = _mapper.Map<ServiceProviderIndexModel>(serviceProvider);

            var stories = (await _context.StoryServiceProviders
                    .Include(x => x.Story)
                    .ThenInclude(x => x.StoryCategories)
                    .ThenInclude(x => x.Category)
                    .ThenInclude(x => x.ParentCategory)
                    .Include(x => x.Story)
                    .ThenInclude(x => x.Images)
                    .Include(x => x.Story)
                    .ThenInclude(x => x.User)
                    .Where(x => x.ServiceProviderId == id)
                    .ToListAsync())
                .Select(x => x.Story);
            model.RelatedStories = _mapper.Map<List<StoryCardModel>>(stories);
            return model;
        }

        public async Task<ServiceProvider> EditAsync(CreateServiceProviderModel model)
        {
            // Get rid of old data on the story.
            var dbServiceProvider = _context.ServiceProviders
                .Include(x => x.Images)
                .Include(x => x.Videos)
                .Include(x => x.Activities)
                .Include(x => x.ServiceProviderCategories)
                .Include(x => x.ServiceProviderLimitations)
                .Single(x => x.Id == model.Id);

            foreach (var serviceProviderImage in dbServiceProvider.Images.Where(x => !model.Images.Contains(x.Url) && !model.Images.Contains(x.CdnUrl) 
                                                                                     && model.HeaderImageUrl != x.CdnUrl && model.HeaderImageUrl != x.Url))
            {
                serviceProviderImage.IsDeleted = true;
            }

            foreach (var activity in dbServiceProvider.Activities)
            {
                activity.IsDeleted = true;
            }

            foreach (var video in dbServiceProvider.Videos)
            {
                video.IsDeleted = true;
            }

            _context.RemoveRange(dbServiceProvider.ServiceProviderCategories);
            _context.RemoveRange(dbServiceProvider.ServiceProviderLimitations);
            _context.SaveChanges();

            var serviceProvider = _mapper.Map<ServiceProvider>(model);
            if (serviceProvider.Images.Any())
            {
                var usedImages = await _context.FileUploads.Where(x => model.Images.Contains(x.Url) && !x.IsUsed).ToListAsync();
                foreach (var usedImage in usedImages)
                {
                    usedImage.IsUsed = true;
                }
            }

            var newImages = serviceProvider.Images.Where(x => !dbServiceProvider.Images.Select(dbImage => dbImage.CdnUrl ?? dbImage.Url).Contains(x.CdnUrl ?? x.Url));

            dbServiceProvider.Images.AddRange(newImages);

            for (int i = 0; i < model.Images.Count; i++)
            {
                var image = dbServiceProvider.Images.SingleOrDefault(x => x.Url == model.Images[i]);
                if (image == null)
                    continue;
                image.Order = i;
                image.IsPrimaryImage = i == 0;
            }

            dbServiceProvider.Videos.AddRange(serviceProvider.Videos);
            dbServiceProvider.Activities.AddRange(serviceProvider.Activities);

            dbServiceProvider.LastUpdateDate = DateTime.UtcNow;
            dbServiceProvider.Name = serviceProvider.Name.Trim();
            dbServiceProvider.Address = serviceProvider.Address;
            dbServiceProvider.AboutTheCompany = serviceProvider.AboutTheCompany;
            dbServiceProvider.AdditionalInformation = serviceProvider.AdditionalInformation;
            dbServiceProvider.City = serviceProvider.City;
            dbServiceProvider.ContactPresonName = serviceProvider.ContactPresonName;
            dbServiceProvider.Email = serviceProvider.Email;
            dbServiceProvider.Country = serviceProvider.Country;
            dbServiceProvider.Facebook = serviceProvider.Facebook;
            dbServiceProvider.HeaderImageUrl = serviceProvider.HeaderImageUrl;
            dbServiceProvider.Instagram = serviceProvider.Instagram;
            dbServiceProvider.IsChapter = serviceProvider.IsChapter;
            dbServiceProvider.Latitude = serviceProvider.Latitude;
            dbServiceProvider.Longitude = serviceProvider.Longitude;
            dbServiceProvider.LogoUrl = serviceProvider.LogoUrl;
            dbServiceProvider.MobileNumber = serviceProvider.MobileNumber;
            dbServiceProvider.WebsiteUrl = serviceProvider.WebsiteUrl;
            dbServiceProvider.TollFreePhoneNumber = serviceProvider.TollFreePhoneNumber;
            dbServiceProvider.State = serviceProvider.State;
            dbServiceProvider.PostalCode = serviceProvider.PostalCode;
            dbServiceProvider.PhoneNumber = serviceProvider.PhoneNumber;
            dbServiceProvider.StreetName = serviceProvider.StreetName;
            dbServiceProvider.StreetNumber = serviceProvider.StreetNumber;

            dbServiceProvider.ServiceProviderCategories = serviceProvider.ServiceProviderCategories;
            dbServiceProvider.ServiceProviderLimitations = serviceProvider.ServiceProviderLimitations;

            await _context.SaveChangesAsync();
            var key = string.Format(RedisKeys.ServiceProviderModel, dbServiceProvider.Id);
            _redisDatabase.KeyDelete(key);
            return serviceProvider;
        }

        public async Task<ServiceProviderContactRequest> ContactServiceProviderAsync(ContactServiceProviderModel model)
        {
            if (!await _recaptcha.ValidateAsync(model.ClientIp, model.RecaptchToken))
            {
                _logger.LogInformation(7843, "Recaptcha incorrect");
            }
            var entity = _mapper.Map<ServiceProviderContactRequest>(model);
            _context.ServiceProviderContactRequests.Add(entity);
            var serviceProvider = await _context.ServiceProviders.SingleAsync(x => x.Id == model.ServiceProviderId);
            await _context.SaveChangesAsync();
            var content = $@"Hi {serviceProvider.Name},<br><br>{model.Name} contacted you regarding your service on yoocanfind.com:<br><br>
""{model.Message}""<br><br>
Email: {model.Email}<br>
Phone: {model.Phone}<br>
Address: {model.Address}";
            await _emailSender.SendEmailAsync(new SendEmailModel
                                              {
                                                  BypassListManagement = true,
                                                  Category = "Contact ServiceProvider",
                                                  Content = content,
                                                  //From = model.Email,
                                                  //FromName = model.Name,
                                                  Subject = "Contact request from yooocan",
                                                  To = serviceProvider.Email,
                                                  NotificationId = $"Contact ServiceProvider_{entity.Id}"
                                              }, new List<SendEmailPersonalizationModel>
                                                 {
                                                     new SendEmailPersonalizationModel
                                                     {
                                                         Name = serviceProvider.Name,
                                                         Email = serviceProvider.Email
                                                     },
                                                     new SendEmailPersonalizationModel
                                                     {
                                                         Name = "Yoav Gaon",
                                                         Email = "yoav@yoocantech.com"
                                                     },
                                                     new SendEmailPersonalizationModel
                                                     {
                                                         Name = "Jessica Levin",
                                                         Email = "jessica@yoocantech.com"
                                                     }
                                                 });

            return entity;
        }

        public async Task FollowAsync(int serviceProviderId, string userId)
        {
            if (await _context.ServiceProviderFollowers.AnyAsync(x => x.UserId == userId &&
                                                                      x.ServiceProviderId == serviceProviderId &&
                                                                      x.DeleteDate == null))
            {
                _logger.LogWarning("ServiceProvider {id} is already followed by {userId}", serviceProviderId, userId);
                return;
            }

            var serviceProviderFollower = new ServiceProviderFollower
                                   {
                                       UserId = userId,
                                       ServiceProviderId = serviceProviderId,
                                   };
            _context.Add(serviceProviderFollower);

            await _context.SaveChangesAsync();
        }

        public async Task UnfollowyAsync(int serviceProviderId, string userId)
        {
            var serviceProviderFollowers = await _context.ServiceProviderFollowers.Where(x => x.UserId == userId &&
                                                                                              x.ServiceProviderId == serviceProviderId &&
                                                                                              x.DeleteDate == null).ToListAsync();
            if (serviceProviderFollowers.Count == 0)
            {
                _logger.LogWarning("ServiceProvider {id} is not followed by {userId}", serviceProviderId, userId);
                return;
            }

            foreach (var serviceProviderFollower in serviceProviderFollowers)
            {
                serviceProviderFollower.DeleteDate = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }
    }
}