using System.Linq;
using System.Threading.Tasks;
using Alto.Dal;
using Alto.Domain;
using Alto.Domain.Users;
using Alto.Logic.Extensions;
using Alto.Logic.Upload;
using Alto.Models.Categories;
using Alto.Web.Utils;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Alto.Web.Controllers
{
    public class CategoryController : BaseController
    {
        private readonly ICategoryLogic _categoryLogic;

        public CategoryController(AltoDbContext context, MapperConfiguration mapperConfiguration, UserManager<AltoUser> userManager, ILogger<BaseController> logger, ICategoryLogic categoryLogic) : base(context, mapperConfiguration, userManager, logger)
        {
            _categoryLogic = categoryLogic;
        }

        [Route("Category/{id:int}/{categoryName?}", Name = "Category")]
        public async Task<IActionResult> Category(int id, string categoryName, bool showProducts = true)
        {
            var category = await Context.Categories.Where(x => x.Id == id)
                                                    .Include(x => x.SubCategories)
                                                    .SingleAsync();
            if (category == null)
                return NotFound();
            if (category.Name.ToCanonical() != categoryName)
                return RedirectToRoutePermanent("Category", new { id, categoryName = category.Name.ToCanonical() });

            var model = category.SubCategories.Any() || category.ParentCategoryId == null
                ? await _categoryLogic.GetParentFeedModelAsync(id, showProducts)
                : await _categoryLogic.GetFeedModelAsync(id, showProducts);
            ViewBag.SelectedCategory = model.ParentCategoryId ?? model.Id;
            ViewBag.ShowProducts = showProducts;

            return View("Category", model);
        }

        public IActionResult Travel()
        {
            return View();
        }

        public IActionResult Insurance()
        {
            return View();
        }

        public IActionResult InsuranceRegisteration()
        {

            if (ViewBag.IsAjax = Request.IsAjaxRequest())
                return PartialView();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> InsuranceRegisteration(InsuranceRegisterationModel model)
        {
            var userId = GetCurrentUserId();
            var ip = NetworkHelper.GetIpAddress(Request);
            var entities = model.FutureServices.Select(x => new UserFutureService
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                FutureService = x,
                Ip = ip,
                UserId = userId,
            });
            Context.UserFutureServices.AddRange(entities);
            await Context.SaveChangesAsync();
            return RedirectToLocal("/");
        }
    }
}