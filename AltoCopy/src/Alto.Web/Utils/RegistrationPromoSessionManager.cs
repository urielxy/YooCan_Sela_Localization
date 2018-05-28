using System;
using System.Text;
using System.Threading.Tasks;
using Alto.Dal;
using Alto.Domain.Companies;
using Alto.Enums.Account;
using Alto.Models.Account;
using Alto.Models.Account.Claims;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Alto.Web.Utils
{
    public class RegistrationPromoSessionManager
    {
        private readonly HttpContext _httpContext;
        private readonly AltoDbContext _context;
        private readonly ILogger<RegistrationPromoSessionManager> _logger;
        private readonly MembershipManager _membershipManager;
        private readonly IDataProtector _dataProtector;

        private const string PromoSessionCookieName = "Promo";

        public RegistrationPromoSessionManager(IHttpContextAccessor httpContextAccessor, IDataProtectionProvider dataProtectorProvider, AltoDbContext context,
            ILogger<RegistrationPromoSessionManager> logger, MembershipManager membershipManager)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _context = context;
            _logger = logger;
            _membershipManager = membershipManager;
            _dataProtector = dataProtectorProvider.CreateProtector("Alto.Web.Controllers.AccountController"/*for backwards compatibility*/);
        }

        public async Task<RegistrationPromo> TrySetRegistrationPromoCookie(string promoCode)
        {
            var promo = await _context.RegistrationPromos.SingleOrDefaultAsync(x => x.PromoCode == promoCode);
            if (promo == null)
            {
                _logger.LogWarning("User entered url of a non-existing promo code: {PromoCode}", promoCode);
                return null;
            }

            if (promo.ExpirationDate < DateTime.UtcNow || promo.ReferralsRemaining <= 0 || promo.IsDisabled)
            {
                _logger.LogWarning(
                    $"User entered url of a problematic promo code: {{PromoCode}} expiration: {promo.ExpirationDate}, " +
                    $"referral remaining: {promo.ReferralsRemaining}, disabled: {promo.IsDisabled} promo", promoCode);
                return null;
            }

            var token = CreateRegistrationPromoSessionToken(promo);
            _httpContext.Response.Cookies.Append(PromoSessionCookieName, token,
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddMonths(24) });

            return promo;
        }

        private string CreateRegistrationPromoSessionToken(RegistrationPromo promo)
        {
            var session = new RegistrationPromoSession
            {
                PromoCode = promo.PromoCode,
                RegistrationPromoId = promo.Id,
                StartDate = DateTime.UtcNow,
                Guid = Guid.NewGuid().ToString(),
                PromoType = promo.Type,
                PromoAmount = promo.Amount
            };

            var json = JsonConvert.SerializeObject(session);
            var encrypted = _dataProtector.Protect(Encoding.Unicode.GetBytes(json));
            var token = Convert.ToBase64String(encrypted);
            return token;
        }

        public RegistrationPromoSession GetRegistrationPromoSession()
        {
            return GetRegistrationPromoSession(_membershipManager.GetMembershipState());
        }

        public RegistrationPromoSession GetRegistrationPromoSession(MembershipState membershipState)
        {
            var token = _httpContext.Request.Cookies[PromoSessionCookieName];
            if (token == null || membershipState == MembershipState.Expired)
                return GetDefaultPromoSession();

            RegistrationPromoSession session;
            try
            {
                var encrypted = Convert.FromBase64String(token);
                var decrypted = _dataProtector.Unprotect(encrypted);
                var json = Encoding.Unicode.GetString(decrypted);
                session = JsonConvert.DeserializeObject<RegistrationPromoSession>(json);
            }
            catch (Exception e)
            {
                _logger.LogError(5235124, e, "error decrypting promo token");
                DeleteCookie();
                return GetDefaultPromoSession();
            }

            //worse promos than the default one
            if (session.PromoType == RegistrationPromoType.DiscountPercentage ||
                session.PromoType == RegistrationPromoType.ExtendedTrial)
            {
                return GetDefaultPromoSession();
            }

            return session;
        }

        public void DeleteCookie()
        {
            _httpContext.Response.Cookies.Delete(PromoSessionCookieName);
        }

        private RegistrationPromoSession GetDefaultPromoSession()
        {
            var defaultPromoSession = new RegistrationPromoSession
            {
                Guid = Guid.NewGuid().ToString(),
                PromoType = RegistrationPromoType.FreeTrial,
                PromoAmount = 1,
                StartDate = DateTime.Now,
                PromoCode = "OXJDVAZ3K30NBA6OME0P",
                RegistrationPromoId = 2
            };

            return defaultPromoSession;
        }
    }
}