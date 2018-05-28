using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Yooocan.Dal;
using Yooocan.Logic;

namespace Yooocan.Web.ViewComponents
{
    public class ShopCategoriesPopupViewComponent : ViewComponentBase
    {
        private readonly ICategoriesLogic _categoriesLogic;

        public ShopCategoriesPopupViewComponent(ApplicationDbContext context, IMapper mapper, ICategoriesLogic categoriesLogic) : base(context, mapper)
        {
            _categoriesLogic = categoriesLogic;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await _categoriesLogic.GetMenuShopAndServiceProvidersCategories();

            ViewBag.RouteName = "CategoryShop";
            return View("/Views/Shared/Components/PopupCategories.cshtml", model);
        }
    }
}
