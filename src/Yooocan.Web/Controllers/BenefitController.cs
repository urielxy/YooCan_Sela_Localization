using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Yooocan.Dal;
using Yooocan.Entities;
using Yooocan.Logic.Benefits;
using Yooocan.Logic.Categories;
using Yooocan.Logic.Extensions;
using Yooocan.Models.Categories;

namespace Yooocan.Web.Controllers
{
    public class BenefitController : BaseController
    {
        private readonly IBenefitLogic _benefitLogic;
        private readonly IAltoCategoryLogic _categoryLogic;

        private readonly Dictionary<int, int> _altoToYoocanShopCategories = new Dictionary<int, int>
        {
            //adaptive living:
            {89, 11},
            {93, 5},
            {91, 5},
            {90, 60},
            {96, 150},
            {92, 5},
            {837, 131},
            //everyday shopping:
            {78, 66},
            {86, 157},
            {79, 66},
            {848, 142},
            //home & car services:
            {845, 148},
            {846, 148},
            //sports & fitness:
            {112, 133},

            //move and think:
            {821, 153},
            {822, 153},
            {825, 153},
            {827, 153},
            {824, 153},
            {828, 153},
            {826, 153},
            {823, 153},
            //calm and focus:
            {807, 151},
            {808, 151},
            {809, 151},
            {810, 151},
            //organizers:
            {813, 154},
            {814, 154},
            {815, 154},
            {816, 154},
            {817, 154},
            //sensory environments & swings:
            {802, 156},
            {803, 156},
            {804, 156},
            {805, 156},
            {806, 156},
            {811, 156},
            {812, 156},
            //school work
            {818, 155},
            {819, 155},
            {820, 155}
        };

        public BenefitController(ApplicationDbContext context, IMapper mapperConfiguration, UserManager<ApplicationUser> userManager, ILogger<BaseController> logger, IBenefitLogic benefitLogic,
            IAltoCategoryLogic categoryLogic) : base(context, logger, mapperConfiguration, userManager)
        {
            _benefitLogic = benefitLogic;
            _categoryLogic = categoryLogic;
        }

        [Route("Benefit/{id:int}/{company?}", Name = "Benefit")]
        public async Task<IActionResult> Index(int id, string company)
        {
            var model = await _benefitLogic.GetModelAsync(id);
            if (model == null)
                return NotFound();

            if (model.CompanyName.ToCanonical() != company)
                return RedirectToRoutePermanent("Benefit", new {id, company = model.CompanyName.ToCanonical()});

            return View(model);
        }

        public async Task<IActionResult> Home()
        {
            var benefitMainCategoryIds = new[] {104, 77, 120, 111};
            var categoryModels = new List<AltoCategoryFeedModel>();
            foreach (var mainCategoryId in benefitMainCategoryIds)
            {
                categoryModels.Add(await _categoryLogic.GetParentFeedModelAsync(mainCategoryId));
            }

            return View(categoryModels);
        }

        [Route("Benefit/Category/{id:int}/{categoryName?}", Name = "BenefitCategory")]
        public async Task<IActionResult> Category(int id, string categoryName)
        {
            if(_altoToYoocanShopCategories.ContainsKey(id))
            {
                return RedirectToAction("Category", "Shop", new { Id = _altoToYoocanShopCategories[id], CategoryName = categoryName?.ToCanonical() });
            }
            var category = await Context.AltoCategories.Where(x => x.Id == id)
                                                    .Include(x => x.SubCategories)
                                                    .SingleAsync();
            if (category == null)
                return NotFound();
            if (category.Name.ToCanonical() != categoryName)
                return RedirectToRoutePermanent("BenefitCategory", new { id, categoryName = category.Name.ToCanonical() });

            var model = category.SubCategories.Any() || category.ParentCategoryId == null
                ? await _categoryLogic.GetParentFeedModelAsync(id)
                : await _categoryLogic.GetFeedModelAsync(id);
            ViewBag.SelectedCategory = model.ParentCategoryId ?? model.Id;

            return View(model);
        }
    }
}
