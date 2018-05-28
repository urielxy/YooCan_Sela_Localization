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
using Yooocan.Entities.Benefits;
using Yooocan.Enums;
using Yooocan.Models.Benefits;
using Yooocan.Models.Cards;

namespace Yooocan.Logic.Benefits
{
    public class BenefitLogic : IBenefitLogic
    {
        private readonly ApplicationDbContext _context;
        private readonly IBlobUploader _blobUploader;
        private readonly IMapper _mapper;
        private readonly IDatabase _redisDatabase;
        private readonly ILogger<BenefitLogic> _logger;

        public BenefitLogic(ApplicationDbContext context, IMapper mapperConfiguration, IBlobUploader blobUploader, IDatabase redisDatabase,
            ILogger<BenefitLogic> logger)
        {
            _context = context;
            _blobUploader = blobUploader;
            _redisDatabase = redisDatabase;
            _logger = logger;
            _mapper = mapperConfiguration;
        }

        public async Task<int> CreateAsync(BenefitEditModel model)
        {
            var entity = _mapper.Map<Benefit>(model);

            if (!string.IsNullOrEmpty(model.ImageDataUri))
            {
                //TODO: Process the image for different thumbnail sizes
                var imageUrl = await _blobUploader.UploadDataUriImage(model.ImageDataUri, "images");

                //TODO: Change to CDN URL
                entity.Images.Add(new BenefitImage { CdnUrl = imageUrl, Type = AltoImageType.Main });
            }

            _context.Benefits.Add(entity);
            await _context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<BenefitEditModel> Get(int benefitId)
        {
            var benefit = await _context.Benefits.Include(x => x.Company)
                                                 .Include(x => x.Images)
                                                 .Include(x => x.Categories)
                                                 .SingleAsync(x => x.Id == benefitId);
            var model = _mapper.Map<BenefitEditModel>(benefit);
            return model;
        }

        public async Task EditAsync(BenefitEditModel model)
        {
            var entity = await _context.Benefits.Include(x => x.Images)
                                                .Include(x => x.Categories)
                                                .SingleAsync(x => x.Id == model.Id);

            _mapper.Map(model, entity);
            if (!string.IsNullOrEmpty(model.ImageDataUri))
            {
                //TODO: Process the image for different thumbnail sizes
                var imageUrl = await _blobUploader.UploadDataUriImage(model.ImageDataUri, "images");

                entity.Images.ForEach(i => i.DeleteDate = DateTime.UtcNow);
                //TODO: Add CDN URL
                entity.Images.Add(new BenefitImage { Url = imageUrl, Type = AltoImageType.Main });
            }

            await _context.SaveChangesAsync();
            await _redisDatabase.KeyDeleteAsync(string.Format(RedisKeys.BenefitModel, model.Id));
        }

        public async Task<List<BenefitCardModel>> GetAllAsync()
        {
            var benefits = await _context.Benefits.Where(x => x.DeleteDate == null)
                                                    .Include(x => x.Images)
                                                    .Include(x => x.Company)
                                                    .Include(x => x.Categories)
                                                    .AsNoTracking()
                                                    .ToListAsync();

            var models = _mapper.Map<List<BenefitCardModel>>(benefits);

            return models;
        }

        public async Task DeleteAsync(int id)
        {
            var benefit = await _context.Benefits.SingleAsync(x => x.Id == id);
            benefit.DeleteDate = DateTime.UtcNow;
            benefit.IsPublished = false;
            await _context.SaveChangesAsync();

            await _redisDatabase.KeyDeleteAsync(string.Format(RedisKeys.BenefitModel, id));
        }

        public async Task<BenefitModel> GetModelAsync(int id)
        {
            var cacheKey = string.Format(RedisKeys.BenefitModel, id);

            BenefitModel model;
            RedisValue redisValue;
            try
            {
                redisValue = await _redisDatabase.StringGetAsync(cacheKey);
            }
            catch (Exception e)
            {
                _logger.LogError(321322, e, "Error when trying to get cache from Redis for {resource}", cacheKey);
                model = await GetModelFromDbAsync(id);
                return model;
            }

            if (redisValue.HasValue)
                return JsonConvert.DeserializeObject<BenefitModel>(redisValue);

            model = await GetModelFromDbAsync(id);
            if (model == null)
                return null;

            redisValue = JsonConvert.SerializeObject(model, Formatting.None, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });

            try
            {
                _redisDatabase.StringSet(cacheKey, redisValue, TimeSpan.FromHours(24), flags: CommandFlags.FireAndForget);
            }
            catch (Exception e)
            {
                _logger.LogError(112233, e, "Writing to Redis of {benefit} failed", id);
            }

            return model;
        }

        private async Task<BenefitModel> GetModelFromDbAsync(int id)
        {
            var benefit = await _context.Benefits
                .Include(x => x.Company)
                .ThenInclude(x => x.Images)
                .Include(x => x.Images)
                .Include(x => x.Categories)
                    .ThenInclude(x => x.Category)
                        .ThenInclude(x => x.ParentCategory)
                .Where(x => x.Id == id && x.DeleteDate == null)
                .SingleOrDefaultAsync();

            if (benefit == null)
            {
                return null;
            }

            var model = _mapper.Map<BenefitModel>(benefit);
            model.RelatedBenefits = await GetRelatedBenefits(id);

            return model;
        }

        private async Task<BenefitsStripModel> GetRelatedBenefits(int id)
        {
            var benefits = await _context.Benefits
                .Where(x => x.Id != id && x.DeleteDate == null)
                .Include(x => x.Company)
                .ThenInclude(company => company.Images)
                .Include(x => x.Images)
                .OrderBy(x => x.Id)
                .Take(10)
                .ToListAsync();

            var models = _mapper.Map<List<BenefitCardModel>>(benefits);
            var strip = new BenefitsStripModel
            {
                Title = "POPULAR BENEFITS",
                Benefits = models
            };

            return strip;
        }
    }
}