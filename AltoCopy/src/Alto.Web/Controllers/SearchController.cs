using System.Threading.Tasks;
using Alto.Dal;
using Alto.Domain;
using Alto.Logic.Search;
using Alto.Models.Search;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Alto.Web.Controllers
{
    public class SearchController : BaseController
    {
        private readonly SearchLogic _searchLogic;

        public SearchController(AltoDbContext context, 
            MapperConfiguration mapperConfiguration, 
            UserManager<AltoUser> userManager, 
            ILogger<BaseController> logger,
            SearchLogic searchLogic) : base(context, mapperConfiguration, userManager, logger)
        {
            _searchLogic = searchLogic;
        }

        public async Task<IActionResult> Index(string query)
        {
            var results = await _searchLogic.SearchAsync(query);
            var model = Mapper.Map<SearchResultModel>(results);

            ViewBag.Query = query;
            ViewBag.IsSearchBarInPage = true;

            return View(model);
        }
    }
}
