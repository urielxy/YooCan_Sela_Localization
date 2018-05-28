using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Alto.Domain;
using Alto.Enums.Account;
using Alto.Logic.Messaging;
using Alto.Models.Messaging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;

namespace Alto.Web.Utils
{
    public class AccountHelper
    {
        private readonly UserManager<AltoUser> _userManager;
        private readonly SignInManager<AltoUser> _signInManager;
        private readonly IUrlHelper _urlHelper;
        private readonly IEmailLogic _emailLogic;
        private readonly ILogger<AccountHelper> _logger;
        private readonly IGoogleAnalyticsLogic _googleAnalyticsLogic;

        public AccountHelper(UserManager<AltoUser> userManager, SignInManager<AltoUser> signInManager, 
            IActionContextAccessor actionContextAccessor, IUrlHelperFactory urlHelperFactory,
            IEmailLogic emailLogic, ILogger<AccountHelper> logger, IGoogleAnalyticsLogic googleAnalyticsLogic)
        {
            _signInManager = signInManager;
            _urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
            _emailLogic = emailLogic;
            _logger = logger;
            _googleAnalyticsLogic = googleAnalyticsLogic;
            _userManager = userManager;
        }

        public async Task<IdentityResult> TryCreateAccount(string email, string password = null, int? promoId = null, RegistrationPromoType? promoType = null)
        {
            var user = new AltoUser
            {
                UserName = email,
                Email = email,
                ReferrerPromoId = promoId
            };
            var result = await _userManager.CreateAsync(user, password ?? Guid.NewGuid().ToString());
            if (!result.Succeeded)
            {
                return result;
            }

            var claim = new Claim(ClaimTypes.Email, email.Trim());
            await _userManager.AddClaimAsync(user, claim);
            
            await _signInManager.SignInAsync(user, true);
            _logger.LogInformation(3, "User created a new account with password.");

            if (!string.IsNullOrEmpty(password))
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = _urlHelper.Action("ConfirmEmail", "Account", new { userId = user.Id, code }, "https");
                await _emailLogic.SendConfirmEmailAsync(email, user.Id, callbackUrl);
            }
            else
            {
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = _urlHelper.Action("ResetPassword", "Account", new { userId = user.Id, code, email }, "https");
                await _emailLogic.SendResetPasswordEmailAsync(new EmailUserData {Email = email, UserId = user.Id }, callbackUrl, false);
                _googleAnalyticsLogic.TrackPageview(user.Id, "/Account/RegistrationCompleted/landingPage");
            }
            
            await _emailLogic.SendPostAccountCreationEmailAsync(email, user.Id, _urlHelper.Action("ContinueRegistration", "Account", null, "https"), 
                    hasFreeTrial: promoType == RegistrationPromoType.FreeTrial);            

            return result;
        }

    }
}
