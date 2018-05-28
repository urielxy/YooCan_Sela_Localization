using System.Linq;
using System.Threading.Tasks;
using Alto.Dal;
using Alto.Domain;
using Alto.Domain.Companies;
using Alto.Models.Categories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Alto.Web.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : BaseController
    {
        public CategoryController(AltoDbContext context, MapperConfiguration mapperConfiguration, UserManager<AltoUser> userManager, ILogger<BaseController> logger) : base(context, mapperConfiguration, userManager, logger)
        {
        }

        public async Task<IActionResult> All()
        {
            var categories = await Context.Categories
                .Where(x=> !x.IsDeleted)
                .OrderBy(x=> x.ParentCategoryId ?? -1)
                .ThenBy(x=> x.Name)
                .Select(x => new CategoryModel
                             {
                                 Id = x.Id,
                                 Name = x.Name,
                                 IsActive = x.IsActive,
                                 ParentCategoryId = x.ParentCategoryId,
                                 ParentCategoryName = x.ParentCategory != null
                                     ? x.ParentCategory.Name
                                     : null
                             }).ToListAsync();
            return View(categories);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await Context.Categories.FindAsync(id);
            if (category == null)
                return NotFound();

            var categories = await Context.Categories
                .Where(x => x.ParentCategoryId == null && !x.IsDeleted)
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem
                             {
                                 Value = x.Id + "",
                                 Text = x.Name
                             })
                .ToListAsync();
            categories.Insert(0, new SelectListItem {Value = "", Text = "No Parent", Selected = true});
            ViewBag.Categories = categories;
            var model = new CategoryModel
                        {
                            Id = category.Id,
                            Name = category.Name,
                            IsActive = category.IsActive,
                            ParentCategoryId = category.ParentCategoryId,
                            ParentCategoryName = category.ParentCategory?.Name
                        };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryModel model)
        {
            var category = await Context.Categories.FindAsync(model.Id);
            if (category == null)
                return NotFound();

            category.Name = model.Name;
            category.IsActive = model.IsActive;
            category.ParentCategoryId = model.ParentCategoryId;
            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(All));
        }

        public async Task<IActionResult> Create()
        {
            var categories = await Context.Categories
                .Where(x => x.ParentCategoryId == null && !x.IsDeleted)
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem
                             {
                                 Value = x.Id + "",
                                 Text = x.Name
                             })
                .ToListAsync();
            categories.Insert(0, new SelectListItem {Value = "", Text = "No Parent", Selected = true});
            ViewBag.Categories = categories;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryModel model)
        {
            var category = new Category
                           {
                               Name = model.Name.Trim(),
                               ParentCategoryId = model.ParentCategoryId,
                               IsActive = model.IsActive
                           };
            Context.Categories.Add(category);
            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(All));
        }
    }
}