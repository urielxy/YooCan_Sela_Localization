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
using Yooocan.Entities.Companies;
using Yooocan.Models.Company;

namespace Yooocan.Web.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class CompanyController : BaseController
    {
        public CompanyController(ApplicationDbContext context, IMapper mapperConfiguration,
            UserManager<ApplicationUser> userManager, ILogger<BaseController> logger)
            : base(context, logger, mapperConfiguration, userManager)
        {
        }

        public async Task<IActionResult> Index()
        {
            var companies = await Context.Companies.Where(x => x.DeleteDate == null).Include(x => x.ContactPersons).ToListAsync();
            var model = Mapper.Map<List<CompanyIndexModel>>(companies);
            return View(model);
        }

        public async Task<IActionResult> EditCommercialTerms(int id)
        {
            var company = await Context.Companies.Where(x => x.Id == id)
                .Include(x => x.Coupons)
                .Include(x => x.ShippingRules)
                .SingleAsync();
            var model = Mapper.Map<CompanyEditTermsModel>(company);

            //not fun and function
            if (id != 421)
            {
                model.CouponCode = company.Coupons.SingleOrDefault()?.Code;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCommercialTerms(int id, CompanyEditTermsModel model)
        {
            if (ModelState.IsValid)
            {
                var company = await Context.Companies.Where(x => x.Id == id)
                    .Include(x => x.Coupons)
                    .Include(x => x.ShippingRules)
                    .SingleAsync();
                model.Name = company.Name;
                model.OnBoardingDate = company.OnBoardingDate;
                model.OnBoardingContactPersonEmail = company.OnBoardingContactPersonEmail;
                Mapper.Map(model, company);

                //not fun and function
                if (id != 421)
                {
                    if (!string.IsNullOrEmpty(model.CouponCode))
                    {
                        company.Coupons.Clear();
                        company.Coupons.Add(new CompanyCoupon { Code = model.CouponCode });
                    }
                }

                await Context.SaveChangesAsync();
            }

            return View(model);
        }
    }
}
