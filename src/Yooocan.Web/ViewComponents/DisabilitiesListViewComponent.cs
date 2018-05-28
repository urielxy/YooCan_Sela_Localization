using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Yooocan.Dal;
using Yooocan.Logic;

namespace Yooocan.Web.ViewComponents
{
    public class DisabilitiesListViewComponent : ViewComponentBase
    {
        private readonly ILimitationLogic _limitationLogic;

        public DisabilitiesListViewComponent(ApplicationDbContext context, IMapper mapper, ILimitationLogic limitationLogic) : base(context, mapper)
        {
            _limitationLogic = limitationLogic;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await _limitationLogic.GetLimitationsAsync();
            
            return View(model);
        }
    }
}
