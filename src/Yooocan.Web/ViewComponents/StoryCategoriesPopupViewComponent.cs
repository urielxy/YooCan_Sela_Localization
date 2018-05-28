using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Yooocan.Dal;
using Yooocan.Logic;

namespace Yooocan.Web.ViewComponents
{
    public class StoryCategoriesPopupViewComponent : ViewComponentBase
    {
        private readonly ICategoriesLogic _categoriesLogic;

        public StoryCategoriesPopupViewComponent(ApplicationDbContext context, IMapper mapper, ICategoriesLogic categoriesLogic) : base(context, mapper)
        {
            _categoriesLogic = categoriesLogic;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await _categoriesLogic.GetMenuFeedCategories();

            ViewBag.RouteName = "Feed";
            return View("/Views/Shared/Components/PopupCategories.cshtml", model);
        }
    }
}
