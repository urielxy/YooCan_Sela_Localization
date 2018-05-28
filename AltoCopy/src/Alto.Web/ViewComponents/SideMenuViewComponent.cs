using System.Threading.Tasks;
using Alto.Dal;
using Alto.Logic.Upload;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Alto.Web.ViewComponents
{
    public class SideMenuViewComponent : BaseViewComponent
    {
        private readonly ICategoryLogic _categoryLogic;

        public SideMenuViewComponent(AltoDbContext context, MapperConfiguration mapperConfiguration, ICategoryLogic categoryLogic) : base(context, mapperConfiguration)
        {
            _categoryLogic = categoryLogic;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await _categoryLogic.GetMenuCategories();
            
            return View("/Views/Shared/Components/SideMenu.cshtml", model);
        }
    }
}