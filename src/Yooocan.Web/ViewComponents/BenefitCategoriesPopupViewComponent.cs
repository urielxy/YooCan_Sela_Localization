using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Yooocan.Dal;
using Yooocan.Logic.Categories;
using Yooocan.Models;

namespace Yooocan.Web.ViewComponents
{
    public class BenefitCategoriesPopupViewComponent : ViewComponentBase
    {
        private readonly IAltoCategoryLogic _categoriesLogic;

        public BenefitCategoriesPopupViewComponent(ApplicationDbContext context, IMapper mapper, IAltoCategoryLogic categoriesLogic) : base(context, mapper)
        {
            _categoriesLogic = categoriesLogic;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _categoriesLogic.GetMenuCategories();
            var model = categories.Select(x => new CategoryModel { Id = x.Id, Name = x.Name.ToUpper() }).ToList();
            ViewBag.RouteName = "BenefitCategory";
            return View("/Views/Shared/Components/PopupCategories.cshtml", model);
        }
    }
}
