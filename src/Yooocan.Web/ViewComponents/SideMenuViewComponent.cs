using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Yooocan.Dal;
using Yooocan.Logic;
using Yooocan.Logic.Categories;
using Yooocan.Models;

namespace Yooocan.Web.ViewComponents
{
    public class SideMenuViewComponent : ViewComponentBase
    {
        private readonly ICategoriesLogic _categoriesLogic;
        private readonly IAltoCategoryLogic _altoCategoriesLogic;

        public SideMenuViewComponent(ApplicationDbContext context, IMapper mapper, ICategoriesLogic categoriesLogic, IAltoCategoryLogic altoCategoriesLogic) : base(context, mapper)
        {
            _categoriesLogic = categoriesLogic;
            _altoCategoriesLogic = altoCategoriesLogic;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new SideMenuModel
            {
                StoriesCategories = await _categoriesLogic.GetMenuFeedCategories(),
                ServiceProvidersCategories = await _categoriesLogic.GetMenuShopAndServiceProvidersCategories(),
                BenefitsCategories = (await _altoCategoriesLogic.GetMenuCategories()).Select(x => new CategoryModel { Id = x.Id, Name = x.Name.ToUpper() }).ToList()
            };

            ViewBag.RouteName = "Feed";
            return View(model);
        }
    }
}
