using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Yooocan.Dal;
using Yooocan.Models;
using Yooocan.Models.Products;
using Yooocan.Models.Shop;
using System.Data.Common;

namespace Yooocan.Logic
{
    public class ShopLogic : IShopLogic
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ShopLogic> _logger;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;
        private readonly RedisWrapper _redisWrapper;

        public ShopLogic(ApplicationDbContext context, ILogger<ShopLogic> logger,
            IMapper mapper, IServiceProvider serviceProvider, RedisWrapper redisWrapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _serviceProvider = serviceProvider;
            _redisWrapper = redisWrapper;
        }

        public async Task<CategoryShopModel> GetCategoryShopAsync(int categoryId, int count)
        {
            var cacheKey = $"shopCategory:{categoryId}";            
            var model = await _redisWrapper.GetModelAsync(cacheKey, async () =>
            {
                return await GetCategoryShopFromDbAsync(categoryId, count);
            }, TimeSpan.FromMinutes(30));

            return model;
        }

        public async Task<CategoryShopModel> GetCategoryShopFromDbAsync(int categoryId, int count, DateTime? maxDate = null, int? lastId = null)
        {
            //some imported products have sort of a default date value
            if (maxDate?.Year < 1000)
            {
                maxDate = null;
            }

            var categories = await _context.Categories.Where(x => x.Id == categoryId || x.ParentCategoryId == categoryId)
                                                        .Include(x => x.ParentCategory)
                                                        .Include(x => x.RedirectCategory)
                                                            .ThenInclude(x => x.ParentCategory)
                                                        .ToListAsync();
            if (!categories.Any())
                return null;

            var category = categories.Single(x => x.Id == categoryId);
            if (category.RedirectCategoryId != null)
                return new CategoryShopModel
                {
                    CategoryId = (int)(category.RedirectCategory.ParentCategoryId ?? category.RedirectCategoryId),
                    CategoryName = category.RedirectCategory.ParentCategory?.Name ?? category.RedirectCategory?.Name
                };

            if (category.ParentCategoryId != null)
            {
                return new CategoryShopModel
                {
                    CategoryId = category.ParentCategoryId.Value,
                    CategoryName = category.ParentCategory.Name
                };
            }
            var categoriesIds = categories.Select(x => x.Id).ToList();
            var products = await _context.Products
                .Where(x => x.IsPublished && !x.IsDeleted && x.ProductCategories.Any(pc => categoriesIds.Contains(pc.CategoryId)) 
                            && ((lastId != null && x.Id < lastId) || lastId == null) && ((maxDate != null && x.InsertDate < maxDate) || maxDate == null))
                .OrderByDescending(x => x.InsertDate)
                .ThenByDescending(x => x.Id)
                .Take(count)
                .Include(x => x.Images)
                .Include(x => x.Company)
                .Include(x => x.ProductCategories)
                .ThenInclude(x => x.Category)
                .ThenInclude(x => x.ParentCategory)
                .ToListAsync();

            var results = new CategoryShopModel
            {
                Products = _mapper.Map<List<ProductCardModel>>(products),
                ShopBackgroundColor = category.ShopBackgroundColor,
                HeaderPictureUrl = category.HeaderPictureUrl,
                CategoryId = categoryId,
                CategoryName = category.Name
            };

            return results;
        }

        public async Task<ShopHomeModel> GetShopHomeAsync()
        {
            return await _redisWrapper.GetModelAsync(RedisKeys.ShopHomeModel, async () =>
            {
                List<CategoryShopModel> categories = new List<CategoryShopModel>();
                var conn = _context.Database.GetDbConnection();
                try
                {
                    await conn.OpenAsync();
                    using (var command = conn.CreateCommand())
                    {
                        string query = @"SELECT * from (
	select				
		parentc.Id as MainCategoryId,	
		parentc.Name as MainCategoryName,		      
		parentc.ShopBackgroundColor,
        [pi].CdnUrl as ProductImageUrl,
		rowid = ROW_NUMBER() OVER (PARTITION BY parentc.Id ORDER BY NEWID())
    FROM ProductCategories PC
	  inner join categories c on PC.categoryid = c.id
	  inner join categories parentc on c.parentcategoryid = parentc.id
	  INNER JOIN Products p ON p.Id = PC.ProductId
	  inner join ProductImages [pi] on [pi].productid = p.id
	WHERE p.IsPublished = 1 and p.isdeleted = 0
      and parentc.isactiveforshop = 1 and parentc.parentcategoryid is null
	  and [pi].type = 1 /*primary image*/
	) as t
where t.rowid = 1
ORDER BY t.MainCategoryId";

                        command.CommandText = query;
                        DbDataReader reader = await command.ExecuteReaderAsync();

                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                var category = new CategoryShopModel
                                {
                                    CategoryId = reader.GetInt32(0),
                                    CategoryName = reader.GetString(1),
                                    ShopBackgroundColor = reader.GetString(2),
                                    HeaderPictureUrl = reader.GetString(3)
                                };
                                categories.Add(category);
                            }
                        }
                        reader.Dispose();
                    }
                }
                finally
                {
                    conn.Close();
                }

                return new ShopHomeModel
                {
                    Categories = categories.OrderBy(x => x.CategoryName).ToList()
                };
            }, TimeSpan.FromHours(3));
        }
    }
}