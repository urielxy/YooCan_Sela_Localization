using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Alto.Dal;
using Alto.Domain;
using Alto.Domain.Referrals;
using Alto.Models.Account.Claims;
using Alto.Web.Utils;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Alto.Web.Controllers
{
    public class ExternalController : BaseController
    {
        private readonly MembershipManager _membershipManager;

        public ExternalController(AltoDbContext context, MapperConfiguration mapperConfiguration, 
            UserManager<AltoUser> userManager, ILogger<BaseController> logger,
            MembershipManager membershipManager) : base(context, mapperConfiguration, userManager, logger)
        {
            _membershipManager = membershipManager;
        }

        public async Task<IActionResult> Product(int id)
        {
            var product = await Context.Products.Include(x => x.Company).SingleOrDefaultAsync(x => x.Id == id);
            if (product == null)
                return NotFound();

            string url = product.Url;
            if (!string.IsNullOrEmpty(product.Company.ReferrerFormat))
            {
                url = string.Format(product.Company.ReferrerFormat, product.Url);
            }

            var referral = InitReferral<ProductReferral>(url);
            referral.ProductId = id;
            Context.ProductReferrals.Add(referral);            

            return await TrySaveReferralAndRedirect(url);
        }

        public async Task<IActionResult> Benefit(int id)
        {
            var membershipState = _membershipManager.GetMembershipState();
            if (membershipState == MembershipState.Unregistered || 
                membershipState == MembershipState.YoocanUnregistered)
            {
                return Unauthorized();
            }

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

        private ReferralClientData GetClientData(string url)
        {
            var clientData = new ReferralClientData
            {
                Ip = NetworkHelper.GetIpAddress(Request),
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                Referrer = Request.Headers.ContainsKey("referer") ? Request.Headers["referer"].ToString() : null,
                UserAgent = Request.Headers.ContainsKey("user-agent") ? Request.Headers["user-agent"].ToString() : null,
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
    }
}