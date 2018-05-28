using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alto.Dal;
using Alto.Models.Categories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Alto.Web.ViewComponents
{
    public class SubMenuViewComponent : BaseViewComponent
    {
        private readonly IMemoryCache _memoryCache;

        public SubMenuViewComponent(AltoDbContext context, MapperConfiguration mapperConfiguration, IMemoryCache memoryCache) : base(context, mapperConfiguration)
        {
            _memoryCache = memoryCache;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await _memoryCache.GetOrCreateAsync("SubMenu", async entry =>
            {
                var parentCategories = (await Context.Categories
                        .Include(x => x.SubCategories)
                        .Include(x => x.Images)
                        .Where(x => !x.IsDeleted && x.IsHeader)
                        .OrderBy(x => x.Name)
                        .ToListAsync())
                    .Where(x => x.ParentCategoryId == 83 /*fun and function*/)
                    .ToList();

                var results = Mapper.Map<List<CategoryMenuModel>>(parentCategories);
                entry.SetValue(results);
                entry.SetAbsoluteExpiration(TimeSpan.FromDays(1));

                return results;
            });

            return View("/Views/Shared/Components/SubMenu.cshtml", model);
        }
    }
}