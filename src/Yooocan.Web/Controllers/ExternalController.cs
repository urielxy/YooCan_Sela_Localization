using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Yooocan.Dal;
using Yooocan.Entities;
using Yooocan.Entities.Referrals;
using Yooocan.Web.Utils;

namespace Yooocan.Web.Controllers
{
    public class ExternalController : BaseController
    {
        public ExternalController(ApplicationDbContext context, ILogger<Controller> logger, IMapper mapper, UserManager<ApplicationUser> userManager)
            : base(context, logger, mapper, userManager)
        {
        }

        public async Task<IActionResult> Product(int id)
        {
            var product = await Context.Products.Include(x => x.Company).SingleOrDefaultAsync(x => x.Id == id);
            if (product == null)
                return NotFound();

            string url = product.Url;
            if (product.AmazonId != null)
            {
                url = $"https://www.amazon.com/exec/obidos/ASIN/{product.AmazonId}/yoocan-20";
            }
            else if (product.Company != null && !string.IsNullOrEmpty(product.Company.ReferrerFormat))
            {
                url = string.Format(product.Company.ReferrerFormat, product.Url);
            }

            var referral = InitReferral<ProductReferral>(url);
            referral.ProductId = id;
            Context.ProductReferrals.Add(referral);

            return await TrySaveReferralAndRedirect(url);
        }

        [Authorize]
        public async Task<IActionResult> Benefit(int id)
        {
            var url = await Context.Benefits.Where(x => x.Id == id)
                                            .Select(x => x.Url)
                                            .SingleOrDefaultAsync();
            if (url == null)
                return NotFound();

            var referral = InitReferral<BenefitReferral>(url);
            referral.BenefitId = id;
            Context.BenefitReferrals.Add(referral);

            return await TrySaveReferralAndRedirect(url);
        }

        public async Task<IActionResult> ServiceProvider(int id)
        {
            var url = await Context.ServiceProviders.Where(x => x.Id == id)
                                            .Select(x => x.WebsiteUrl)
                                            .SingleOrDefaultAsync();
            if (url == null)
                return NotFound();

            var referral = InitReferral<ServiceProviderReferral>(url);
            referral.ServiceProviderId = id;
            Context.ServiceProviderReferrals.Add(referral);

            return await TrySaveReferralAndRedirect(url);
        }

        private ReferralClientData GetClientData(string url)
        {
            var clientData = new ReferralClientData
            {
                Ip = NetworkHelper.GetIpAddress(Request),
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                Referrer = HttpContext.Request.Headers["referer"],
                UserAgent = HttpContext.Request.Headers["user-agent"],
                Url = url
            };

            return clientData;
        }

        private T InitReferral<T>(string url) where T : new()
        {
            var referral = new T();
            var clientData = GetClientData(url);
            Mapper.Map(clientData, referral);
            return referral;
        }

        private async Task<IActionResult> TrySaveReferralAndRedirect(string url)
        {
            try
            {
                await Context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Logger.LogError(12125, e, "Saving referral data failed, redirecting anyway");
            }
            return Redirect(url);
        }
    }
}