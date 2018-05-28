using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Alto.Domain.Companies;
using Alto.Web.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Alto.Web.ActionFilters
{
    public class PromoRegistrationFilter : IAsyncActionFilter
    {
        private readonly RegistrationPromoSessionManager _registrationPromoSessionManager;
        private readonly AccountHelper _accountHelper;
        private readonly bool _allowInstantRegistration;

        public PromoRegistrationFilter(RegistrationPromoSessionManager registrationPromoSessionManager,
            AccountHelper accountHelper, bool allowInstantRegistration = false)
        {
            _registrationPromoSessionManager = registrationPromoSessionManager;
            _accountHelper = accountHelper;
            _allowInstantRegistration = allowInstantRegistration;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            const string promoCodeParamName = "promo";
            const string emailParamName = "email";

            RegistrationPromo registrationPromo = null;
            var promoCode = context.HttpContext.Request.Query[promoCodeParamName];
            if (!string.IsNullOrWhiteSpace(promoCode))
            {
                registrationPromo = await _registrationPromoSessionManager.TrySetRegistrationPromoCookie(promoCode);
                if (registrationPromo != null)
                {
                    context.Result = new RedirectResult(UrlHelper.GetUrlWithRemovedQueryParams(context.HttpContext.Request, promoCodeParamName));
                }
            }

            if (_allowInstantRegistration)
            {
                var email = context.HttpContext.Request.Query[emailParamName];
                if (!string.IsNullOrWhiteSpace(email) && !context.HttpContext.User.Identity.IsAuthenticated)
                {
                    var result = await _accountHelper.TryCreateAccount(email, promoId: registrationPromo?.ReferrerUserId, promoType: registrationPromo?.Type);
                    if (result.Succeeded)
                    {
                        context.Result = new RedirectResult(UrlHelper.GetUrlWithRemovedQueryParams(context.HttpContext.Request, emailParamName, promoCodeParamName));
                    }
                }
            }

            if(context.Result == null)
                await next();
        }
    }
}