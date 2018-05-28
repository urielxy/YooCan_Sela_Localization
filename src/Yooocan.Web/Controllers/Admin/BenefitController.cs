using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Yooocan.Dal;
using Yooocan.Entities;
using Yooocan.Logic.Benefits;
using Yooocan.Models;
using Yooocan.Models.Benefits;
using Yooocan.Models.Company;

namespace Yooocan.Web.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class BenefitController : BaseController
    {
        private readonly IBenefitLogic _benefitLogic;

        public BenefitController(ApplicationDbContext context, IMapper mapperConfiguration, UserManager<ApplicationUser> userManager, ILogger<BaseController> logger, IBenefitLogic benefitLogic) : base(context, logger, mapperConfiguration, userManager)
        {
            _benefitLogic = benefitLogic;
        }

        public async Task<IActionResult> All()
        {
            var model = await _benefitLogic.GetAllAsync();
            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            var model = new BenefitEditModel
            {
                CompaniesOptions = await Context.Companies.Where(x => x.DeleteDate == null).Select(x => new CompanySelectModel { Id = x.Id, Name = x.Name }).ToListAsync(),
                CategoriesOptions = await GetAllCategories()
            };
            return View(model);
        }

        //public async Task<IActionResult> Promoted()
        //{
        //    var data = await Context.PromotedBenefits
        //        .Include(x => x.Benefit)
        //        .OrderBy(x => x.Order)
        //        .Select(x => new PromotedBenefitModel
        //                     {
        //                         BenefitId = x.BenefitId,
        //                         PromotionType = x.PromotionType,
        //                         Title = x.Benefit.Title
        //                     }).ToListAsync();
        //    return View(data);
        //}

        //[HttpPost]
        //public async Task<IActionResult> Promoted([FromBody]List<PromotedBenefitModel> data)
        //{
        //    if (data.Count == 0)
        //    {
        //        return BadRequest("Empty list");
        //    }

        //    Context.PromotedBenefits.RemoveRange(Context.PromotedBenefits);
        //    var entities = data.Select((x, index) => new PromotedBenefit
        //                                             {
        //                                                 BenefitId = x.BenefitId,
        //                                                 PromotionType = x.PromotionType,
        //                                                 Order = index
        //                                             });
        //    Context.PromotedBenefits.AddRange(entities);
        //    await Context.SaveChangesAsync();

        //    return Ok();
        //}

        private async Task<List<CategoryModel>> GetAllCategories()
        {
            return await Context.Categories.Where(x => x.ParentCategory != null)
                                            .OrderBy(x => x.Name)
                                            .Select(x => new CategoryModel { Id = x.Id, Name = x.Name })
                                            .ToListAsync();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BenefitEditModel model)
        {
            var id = await _benefitLogic.CreateAsync(model);

            return RedirectToAction(nameof(Edit), new { id });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var model = await _benefitLogic.Get(id);
            model.CategoriesOptions = await GetAllCategories();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BenefitEditModel model)
        {
            await _benefitLogic.EditAsync(model);

            return RedirectToAction(nameof(Edit), model.Id);
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _benefitLogic.DeleteAsync(id);

            return RedirectToAction(nameof(All));
        }
    }
}