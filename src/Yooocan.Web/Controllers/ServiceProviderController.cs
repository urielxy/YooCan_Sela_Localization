using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Yooocan.Dal;
using Yooocan.Entities;
using Yooocan.Logic;
using Yooocan.Logic.Extensions;
using Yooocan.Logic.Messaging;
using Yooocan.Models.ServiceProviders;
using Microsoft.EntityFrameworkCore;
using Yooocan.Models;
using Yooocan.Web.Utils;

namespace Yooocan.Web.Controllers
{
    public class ServiceProviderController : BaseController
    {
        private readonly IServiceProviderLogic _serviceProviderLogic;
        private readonly ICategoriesLogic _categoriesLogic;
        private readonly ILimitationLogic _limitationLogic;

        public ServiceProviderController(ApplicationDbContext context, ILogger<ServiceProviderController> logger, IMapper mapper,
            UserManager<ApplicationUser> userManager, IEmailSender emailSender, IBlobUploader blobUploader, IServiceProviderLogic serviceProviderLogic,
            ICategoriesLogic categoriesLogic, ILimitationLogic limitationLogic) : base(context, logger, mapper, userManager)
        {
            _serviceProviderLogic = serviceProviderLogic;
            _categoriesLogic = categoriesLogic;
            _limitationLogic = limitationLogic;
        }

        [Route("ServiceProvider/{id:int}/{name?}", Name = "ServiceProvider")]
        [Route("{id:int}/Member/{name?}", Name = "ServiceProviderMember")]
        public async Task<ActionResult> Index(int id, string name)
        {
            var model = await _serviceProviderLogic.GetModelAsync(id, GetCurrentUserId());
            if (model == null)
                return NotFound();

            if (name != model.Name.ToCanonical())
                return RedirectToRoutePermanent("ServiceProvider", new { id, Name = model.Name.ToCanonical() });

            return View(model);
        }

        [Route("ServiceProvider/Category/{id:int}/{categoryName?}", Name = "CategoryServiceProvider")]
        public async Task<ActionResult> Category(int id, string categoryName)
        {
            var category = await Context.Categories.Include(x => x.ParentCategory)
                                                    .Include(x => x.RedirectCategory)
                                                    .SingleOrDefaultAsync(x => x.Id == id);
            if (category == null)
                return NotFound();

            if (category.RedirectCategoryId != null)
                return RedirectToRoute("CategoryServiceProvider", new { id = category.RedirectCategoryId, categoryName = category.RedirectCategory.Name.ToCanonical() });

            if (category.ParentCategoryId != null)
            {
                if (!category.ParentCategory.IsActiveForShop)
                    return NotFound();

                return RedirectToRoute("CategoryServiceProvider", new
                {
                    id = category.ParentCategoryId,
                    categoryName = category.ParentCategory.Name.ToCanonical()
                });
            }

            if (category.Name.ToCanonical() != categoryName)
            {
                return RedirectToRoutePermanent("CategoryServiceProvider", new
                {
                    category.Id,
                    categoryName = category.Name.ToCanonical()
                });
            }

            var model = new ServiceProvidersCategoryModel
            {
                Id = category.Id,
                Name = category.Name,
                HeaderImageUrl = category.HeaderPictureUrl,
                MobileHeaderImageUrl = category.MobileHeaderPictureUrl ?? category.HeaderPictureUrl,
                ServiceProviders = await GetCategoryPage(id, 0)
            };

            return View(model);
        }

        public async Task<ActionResult> MoreFromCategory(int id, int page)
        {
            return PartialView("_CategoryCards", await GetCategoryPage(id, page));
        }

        private async Task<List<RelatedServiceProviderModel>> GetCategoryPage(int categoryId, int pageIndex)
        {
            var serviceProvidersPerPage = 20;
            var data = await Context.ServiceProviders
                .Include(x => x.Images)
                .Where(x => x.IsPublished && !x.IsDeleted &&
                            x.ServiceProviderCategories.Any(spc => spc.Category.ParentCategoryId == categoryId || spc.CategoryId == categoryId))
                .OrderBy(x => x.Id)
                .Skip(pageIndex * serviceProvidersPerPage)
                .Take(serviceProvidersPerPage)                
                .ToListAsync();

            return Mapper.Map<List<RelatedServiceProviderModel>>(data);
        }

        [Authorize]
        public async Task<ActionResult> Follow(int id)
        {
            var userId = GetCurrentUserId();
            await _serviceProviderLogic.FollowAsync(id, userId);
            return NoContent();
        }

        [Authorize]
        public async Task<ActionResult> Unfollow(int id)
        {
            var userId = GetCurrentUserId();
            await _serviceProviderLogic.UnfollowyAsync(id, userId);
            return NoContent();
        }

        [Authorize]
        public ActionResult Create()
        {
            return OldIframeContainer();
        }


        [Authorize]
        public async Task<ActionResult> CreateOld()
        {
            var model = new CreateServiceProviderModel
            {
                CategoriesOptions = await _categoriesLogic.GetCategoriesForProductAsync(),
                LimitationsOptions = await _limitationLogic.GetLimitationsAsync()
            };

            return OldView(model);
        }

        #region Edit

        [Authorize(Policy = "MyServiceProvider")]
        public ActionResult Edit(int id)
        {
            return OldIframeContainer();
        }

        [Authorize(Policy = "MyServiceProvider")]
        public async Task<ActionResult> EditOld(int id)
        {
            var serviceProvider = await Context.ServiceProviders
                .Include(x => x.Images)
                .Include(x => x.Videos)
                .Include(x => x.ServiceProviderCategories)
                    .ThenInclude(x => x.Category)
                .Include(x => x.ServiceProviderLimitations)
                .Include(x => x.Activities)
                .SingleAsync(x => x.Id == id);

            if (serviceProvider == null)
                return NotFound();

            serviceProvider.Images = serviceProvider.Images.Where(x => !x.IsDeleted).ToList();
            serviceProvider.Videos = serviceProvider.Videos.Where(x => !x.IsDeleted).ToList();
            serviceProvider.Activities = serviceProvider.Activities
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Order)
                .ToList();

            var model = Mapper.Map<CreateServiceProviderModel>(serviceProvider);
            model.CategoriesOptions = await _categoriesLogic.GetCategoriesForProductAsync();
            model.LimitationsOptions = await _limitationLogic.GetLimitationsAsync();

            return OldView(model);
        }

        [HttpPost]
        [Authorize(Policy = "MyServiceProvider")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CreateServiceProviderModel model)
        {
            if (ModelState.IsValid)
            {
                var serviceProvider = await _serviceProviderLogic.EditAsync(model);
                return RedirectToRoute("ServiceProvider", new
                {
                    id = serviceProvider.Id,
                    name = serviceProvider.Name.ToCanonical()
                });
            }

            LogModelStateErrors();

            model.CategoriesOptions = await _categoriesLogic.GetCategoriesForProductAsync();
            model.LimitationsOptions = await _limitationLogic.GetLimitationsAsync();

            return OldView(model);
        }

        #endregion

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateServiceProviderModel model)
        {
            if (ModelState.IsValid)
            {
                model.UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var serviceProvider = await _serviceProviderLogic.CreateAsync(model);
                return RedirectToRoute("ServiceProvider", new
                {
                    id = serviceProvider.Id,
                    name = serviceProvider.Name.ToCanonical()
                });
            }

            LogModelStateErrors();

            model.CategoriesOptions = await _categoriesLogic.GetCategoriesForProductAsync();
            model.LimitationsOptions = await _limitationLogic.GetLimitationsAsync();

            return OldView(model);
        }

        [HttpGet]
        [Authorize]
        public ActionResult Preview(CreateServiceProviderModel model)
        {
            var previewModel = Mapper.Map<ServiceProviderIndexModel>(model);
            return View(nameof(Index), previewModel);
        }

        public IActionResult MyServices()
        {
            throw new System.NotImplementedException();
        }

        public async Task<IActionResult> Contact(ContactServiceProviderModel model)
        {
            if (ModelState.IsValid)
            {
                model.ClientIp = NetworkHelper.GetIpAddress(Request);
                await _serviceProviderLogic.ContactServiceProviderAsync(model);
                return Ok();
            }

            return BadRequest(ModelState);
        }
    }
}