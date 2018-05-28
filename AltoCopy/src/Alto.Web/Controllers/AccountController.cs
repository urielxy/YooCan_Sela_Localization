using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Alto.Dal;
using Alto.Domain;
using Alto.Domain.Users;
using Alto.Enums;
using Alto.Enums.Account;
using Alto.Logic.Extensions;
using Alto.Logic.Messaging;
using Alto.Logic.PayPal;
using Alto.Logic.Upload;
using Alto.Models.Account;
using Alto.Models.Account.Claims;
using Alto.Models.Messaging;
using Alto.Models.PayPal;
using Alto.Web.Models.AccountViewModels;
using Alto.Web.Utils;
using Alto.Web.Utils.Yoocan;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PayPal.Api;

namespace Alto.Web.Controllers
{
    //TODO: uncomment profile pic code
    [Authorize]
    public class AccountController : BaseController
    {
        private readonly SignInManager<AltoUser> _signInManager;
        private readonly string _externalCookieScheme;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly IEmailLogic _emailLogic;
        private readonly IGoogleAnalyticsLogic _googleAnalyticsLogic;
        private readonly IBlobUploader _blobUploader;
        private readonly PayPalLogic _payPalLogic;
        private readonly RegistrationPromoSessionManager _registrationPromoSessionManager;
        private readonly AccountHelper _accountHelper;
        private readonly MembershipManager _membershipManager;
        private readonly YoocanUsersManager _yoocanUsersManager;

        public AccountController(
            UserManager<AltoUser> userManager,
            SignInManager<AltoUser> signInManager,
            IOptions<IdentityCookieOptions> identityCookieOptions,
            IEmailSender emailSender,
            ISmsSender smsSender,
            AltoDbContext context,
            IEmailLogic emailLogic,
            IGoogleAnalyticsLogic googleAnalyticsLogic,
            ILogger<AccountController> logger,
            IBlobUploader blobUploader,
            MapperConfiguration mapper,
            PayPalLogic payPalLogic,
            RegistrationPromoSessionManager registrationPromoSessionManager,
            AccountHelper accountHelper,
            MembershipManager membershipManager,
            YoocanUsersManager yoocanUsersManager) : base(context, mapper, userManager, logger)
        {
            _signInManager = signInManager;
            _externalCookieScheme = identityCookieOptions.Value.ExternalCookieAuthenticationScheme;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _emailLogic = emailLogic;
            _googleAnalyticsLogic = googleAnalyticsLogic;
            _blobUploader = blobUploader;
            _payPalLogic = payPalLogic;
            _registrationPromoSessionManager = registrationPromoSessionManager;
            _accountHelper = accountHelper;
            _membershipManager = membershipManager;
            _yoocanUsersManager = yoocanUsersManager;
        }

        //
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewBag.HideRegisterButtons = true;

            await HttpContext.Authentication.SignOutAsync(_externalCookieScheme);
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            model.Email = model.Email.Trim();
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    Logger.LogInformation(1, "User logged in.");
                    if (Request.IsAjaxRequest())
                        return Ok();

                    return RedirectToLocal(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    Logger.LogWarning(2, "User account locked out.");
                    return View("Lockout");
                }

                ModelState.AddModelError(nameof(model.Email), "Incorrect login attempt.");
            }

            // If we got this far, something failed, redisplay form
            if (Request.IsAjaxRequest())
            {
                return ReturnAjaxErrors();
            }
            return View(model);
        }

        public IActionResult ContinueRegistration()
        {
            var membershipState = _membershipManager.GetMembershipState();
            int targetRegistrationState;
            if (membershipState == MembershipState.Registered)
                targetRegistrationState = 1;
            else if (membershipState == MembershipState.FilledDetails || membershipState == MembershipState.Expired)
                targetRegistrationState = 3;
            else if (membershipState == MembershipState.Payed)
                targetRegistrationState = 4;
            else
                return BadRequest();

            return RedirectToAction("Register", new {registrationState = targetRegistrationState});
        }

        //
        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewBag.HideRegisterButtons = true;

            var model = await CreateRegisterModel();
            return View(model);
        }

        private async Task<RegisterModel> CreateRegisterModel()
        {
            var model = new RegisterModel
            {
                MembershipState = _membershipManager.GetMembershipState(),
                UserDetailsModel = await GetUserDetailsModel(),
                PaymentModel = GetPaymentModel()
            };
            return model;
        }

        private async Task<UserDetailsModel> GetUserDetailsModel()
        {
            var model = new UserDetailsModel();
            var limitations = await Context.Limitations.OrderBy(x => x.Name).ToListAsync();
            model.Limitations = Mapper.Map<List<LimitationModel>>(limitations);

            return model;
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            
            if (ModelState.IsValid)
            {
                var referrerPromo = _registrationPromoSessionManager.GetRegistrationPromoSession(MembershipState.Unregistered);
                
                var result = await _accountHelper.TryCreateAccount(model.Email, model.Password, referrerPromo?.RegistrationPromoId, referrerPromo?.PromoType);
                if (result.Succeeded)
                {
                    if (Request.IsAjaxRequest())
                        return Ok();

                    return RedirectToAction("Index", "Home", new { returnUrl });
                }

                AddErrors(result);
            }
            
            if (Request.IsAjaxRequest())
                return ReturnAjaxErrors();

            return BadRequest();
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        //security: removed anti-forgery check to allow logout after login 
        //without getting a new token from the server, low risk action
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            Logger.LogInformation(4, "User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [AllowAnonymous]
        public ActionResult ExternalLoginWindow(string provider, string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View("ExternalLoginWindow", provider);
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLogin(string provider, string returnUrl = null, bool isPopup = false)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info != null)
            {
                return RedirectToAction("ExternalLoginCallback", "Account", new { isPopup });
            }

            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { returnUrl, isPopup });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        //
        // GET: /Account/ExternalLoginCallback
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(bool isPopup, string returnUrl = null, string remoteError = null)
        {
            var successScript = returnUrl == null
                ? "window.opener.location.reload();"
                : $"window.opener.location='{returnUrl}';";

            var successResponsePopup = $"<html><head><script>{successScript}window.close();</script></head><body></body></html>";

            var isNewUser = false;
            var errorLink = Url.Action(nameof(Login), "Account", null, HttpContext.Request.Scheme);
            var errorResponse = $"<html><head><script>window.opener.location={errorLink};window.close();</script></head><body></body></html>";

            if (remoteError != null)
            {
                if (isPopup)
                    return Content(errorResponse, "text/html");

                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View(nameof(Login));
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null && isPopup)
                return Content(errorResponse, "text/html");

            if (info == null)
                return RedirectToAction(nameof(Login));

            // Sign in the user with this external login provider if the user already has a login.

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true);
            Logger.LogInformation("{name} - {email} tried to login with {provider}, {success}", info.Principal.Identity.Name, info.Principal.FindFirst(ClaimTypes.Email),
                info.LoginProvider, result.Succeeded);

            if (result.Succeeded)
            {
                Logger.LogInformation(5, "User logged in with {Name} provider.", info.LoginProvider);
                if (isPopup)
                    return Content(successResponsePopup, "text/html");

                return RedirectToLocal(returnUrl);
            }

            var picture = info.Principal.FindFirst("picture")?.Value;
            var firstName = info.Principal.FindFirst(ClaimTypes.GivenName)?.Value;
            var lastName = info.Principal.FindFirst(ClaimTypes.Surname)?.Value;
            var email = info.Principal.FindFirst(ClaimTypes.Email)?.Value;

            //if user's account is created after he clicked "login", currently not moving him to fill user details
            var completeRegistrationPopup = $@"<html><head><script>if(window.opener.setRegistrationState){{window.opener.setRegistrationState(1, true, {{FirstName: ""{firstName}"",LastName: ""{lastName}""}});}}else{{{successScript}}}try{{window.opener.ga('send','pageview','{$"/Account/RegistrationCompleted/{info.LoginProvider}"}');window.opener.fbq('track','Lead');}}catch(e){{console.error(e);}}window.close();</script></head></html>";

            // If we didn't get email permissions
            if (email == null && isPopup)
                return Content(errorResponse, "text/html");
            if (email == null)
                return RedirectToAction(nameof(Login));

            var normalizedEmail = email.Normalize();
            var promo = _registrationPromoSessionManager.GetRegistrationPromoSession(MembershipState.Unregistered);

            var user = await UserManager.Users.Include(x => x.Claims).SingleOrDefaultAsync(x => x.NormalizedEmail == normalizedEmail);
            if (user != null)
            {
                Logger.LogInformation("{name} {email} exist in the DB", info.Principal.Identity.Name, info.Principal.FindFirst(ClaimTypes.Email));
            }
            else
            {
                isNewUser = true;
                Logger.LogInformation("{name} {email} doesn't exist in the DB", info.Principal.Identity.Name,
                    info.Principal.FindFirst(ClaimTypes.Email));

                user = new AltoUser
                {
                    UserName = email,
                    Email = email,
                    //PictureUrl = picture,
                    FirstName = firstName,
                    LastName = lastName,
                    EmailConfirmed = true,
                    ReferrerPromoId = promo?.RegistrationPromoId
                };
                var createUserResult = await UserManager.CreateAsync(user);
                if (!createUserResult.Succeeded)
                {
                    throw new Exception(string.Join(", ", createUserResult.Errors.Select(x => $"Code:{x.Code}. Description:{x.Description}")));
                }
            }

            var addLoginResult = await UserManager.AddLoginAsync(user, info);
            Logger.LogInformation("add external login to {email} {provider}, {success}", info.Principal.Identity.Name, info.LoginProvider, result.Succeeded);
            if (addLoginResult.Succeeded)
            {
                var alreadyExistingClaimsTypes = user.Claims.Select(x => x.ClaimType).ToList();

                var claimsToAdd = info.Principal.Claims.Where(x => !alreadyExistingClaimsTypes.Contains(x.Type)).ToList();
                try
                {
                    // Take the avatar of the 3rd party and use ours storage (because FB url expires)
                    var pictureClaim = claimsToAdd.FirstOrDefault(x => x.Type == "picture");
                    if (pictureClaim != null)
                    {
                        using (var client = new HttpClient())
                        {
                            using (var stream = await client.GetStreamAsync(picture))
                            {
                                picture = await _blobUploader.UploadStreamAsync(stream, "profiles", Guid.NewGuid().ToString("N"), quality: 100);
                            }
                        }

                        claimsToAdd.Remove(pictureClaim);
                        claimsToAdd.Add(new Claim("picture", picture));
                        user.Images.Add(new UserImage
                        {
                            Url = picture,
                            CdnUrl = picture,
                            Type = ImageType.Profile
                        });
                        await UserManager.UpdateAsync(user);
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError(432412, e, "Something went wrong with getting the avatar of {email}", user.Email);
                }
                await UserManager.AddClaimsAsync(user, claimsToAdd);
                await _signInManager.SignInAsync(user, isPersistent: true);
                Logger.LogInformation(6, "User created an account using {Name} provider.", info.LoginProvider);
                await _emailLogic.SendPostAccountCreationEmailAsync(user.Email, user.Id, Url.Action("ContinueRegistration", "Account", null, HttpContext.Request.Scheme), 
                                                                    hasFreeTrial: promo?.PromoType == RegistrationPromoType.FreeTrial);

                if (isPopup)
                {
                    return Content(isNewUser ? completeRegistrationPopup : successResponsePopup, "text/html");
                }
                return RedirectToLocal(returnUrl);
            }

            Logger.LogInformation("Can't login external login --not implemented {email} {provider}", info.Principal.Identity.Name, info.LoginProvider);
            return Content(errorResponse, "text/html");
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var picture = info.Principal.FindFirst("picture")?.Value;
                var firstName = info.Principal.FindFirst(ClaimTypes.GivenName)?.Value;
                var lastName = info.Principal.FindFirst(ClaimTypes.Surname)?.Value;
                var promo = _registrationPromoSessionManager.GetRegistrationPromoSession();
                var user = new AltoUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    //PictureUrl = picture,
                    FirstName = firstName,
                    LastName = lastName,
                    ReferrerPromoId = promo?.RegistrationPromoId,
                    EmailConfirmed = true
                };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: true);
                        Logger.LogInformation(6, "User created an account using {Name} provider.", info.LoginProvider);
                        await _emailLogic.SendPostAccountCreationEmailAsync(user.Email, user.Id, Url.Action("ContinueRegistration", "Account", null, 
                            HttpContext.Request.Scheme), user.FirstName, promo?.PromoType == RegistrationPromoType.FreeTrial);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        // GET: /Account/ConfirmEmail
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([Required]int userId, [Required] string code)
        {
            if (code == null)
                ModelState.AddModelError(string.Empty, $"{nameof(userId)} or {nameof(code)} were empty");

            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, $"Unknown user {userId}");
                }
                if (ModelState.IsValid)
                {
                    var result = await UserManager.ConfirmEmailAsync(user, code);
                    if (result.Succeeded)
                    {
                        _googleAnalyticsLogic.TrackPageview(userId, "/Account/ConfirmEmail");

                        if (User.Identity.IsAuthenticated && User.FindFirst(ClaimTypes.NameIdentifier).Value == userId.ToString())
                            await _signInManager.RefreshSignInAsync(user);

                        return RedirectToAction("Index", "Home");
                    }

                    AddErrors(result);
                }
            }

            LogModelStateErrors();

            return View("Error");
        }

        //
        // GET: /Account/ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            Logger.LogInformation("{email} asked for password reset", model.Email);
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email.Trim());
                if (user == null /*|| !(await UserManager.IsEmailConfirmedAsync(user))*/)
                {
                    Logger.LogWarning("{email} doesn't for reset password ", model.Email);
                    ModelState.AddModelError(nameof(model.Email), $"The email {model.Email} does not exist in the system.");

                    if (Request.IsAjaxRequest())
                        return ReturnAjaxErrors();
                    return View();
                }

                var code = await UserManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code, email = user.Email }, HttpContext.Request.Scheme);
                var userData = new EmailUserData
                {
                    Email = user.Email,
                    UserId = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                };

                var isSuccess = await _emailLogic.SendResetPasswordEmailAsync(userData, callbackUrl, true);
                if (isSuccess)
                {
                    return RedirectToAction("ForgotPasswordConfirmation");
                }

                ModelState.AddModelError(string.Empty, "Something went wrong, please contact support");
            }

            if (Request.IsAjaxRequest())
            {
                ReturnAjaxErrors();
            }

            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null, string email = null)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByEmailAsync(model.Email.Trim());
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(ResetPasswordConfirmation), "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                Logger.LogInformation("{email} reseted his password", model.Email);
                if (!user.EmailConfirmed)
                {
                    user.EmailConfirmed = true;
                    await UserManager.UpdateAsync(user);
                    Logger.LogInformation("{email} chose a password", model.Email);
                }
                return RedirectToAction(nameof(ResetPasswordConfirmation), "Account");
            }            
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/SendCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl = null, bool rememberMe = false)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(user);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            // Generate the token and send it
            var code = await UserManager.GenerateTwoFactorTokenAsync(user, model.SelectedProvider);
            if (string.IsNullOrWhiteSpace(code))
            {
                return View("Error");
            }

            var message = "Your security code is: " + code;
            if (model.SelectedProvider == "Email")
            {
                await _emailSender.SendEmailAsync(user.Id, user.Email, "Security Code", message, "Security code", null);
            }
            else if (model.SelectedProvider == "Phone")
            {
                await _smsSender.SendSmsAsync(await UserManager.GetPhoneNumberAsync(user), message);
            }

            return RedirectToAction(nameof(VerifyCode), new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/VerifyCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyCode(string provider, bool rememberMe, string returnUrl = null)
        {
            // Require that the user has already logged in via username/password or external login
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes.
            // If a user enters incorrect codes for a specified amount of time then the user account
            // will be locked out for a specified amount of time.
            var result = await _signInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser);
            if (result.Succeeded)
            {
                return RedirectToLocal(model.ReturnUrl);
            }
            if (result.IsLockedOut)
            {
                Logger.LogWarning(7, "User account locked out.");
                return View("Lockout");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid code.");
                return View(model);
            }
        }

        //public IActionResult ChooseSubscription()
        //{
        //    return View();
        //}
        //[HttpPost]
        //[HttpGet]
        //public IActionResult CreateTrialBillingAgreement()
        //{
        //    return Ok(new CreatePaymentResult
        //    {
        //        PaymentID = "EC-44L42847Y71252641"
        //    });
        //}

        [HttpPost]
        public async Task<IActionResult> UserDetails(UserDetailsModel model, string returnUrl)
        {
            if (!Request.IsAjaxRequest())
            {
                Logger.LogWarning("possible csrf attempt");
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return ReturnAjaxErrors();
            }
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await UserManager.FindByIdAsync(userId.ToString());

            Mapper.Map(model, user);
            await UpdateUserClaims(user);
            await UserManager.UpdateAsync(user);
            await Context.SaveChangesAsync();

            var state = _membershipManager.GetMembershipState();
            if (state == MembershipState.Payed)
            {
                return Ok(MembershipState.Payed);
            }

            if (await FinalizeRegistrationIfFreeTrialPromoUser())
            {
                return Ok(MembershipState.Payed);
            }

            if (state == MembershipState.Registered)
            {
                await UpdateMembershipState(MembershipState.FilledDetails);
            }

            return Ok(MembershipState.FilledDetails);
        }

        #region Yoocan - Alto Authentication

        //not security critical as there's no billing and no private info sharing!
        [AllowAnonymous]
        public IActionResult InitYoocanUser(string token, string returnUrl)
        {                     
            if (!string.IsNullOrEmpty(token))
            {
                _yoocanUsersManager.TrySetYoocanUserCookie(token);
            }

            returnUrl+=$"{(returnUrl.Contains("?") ? "&" : "?")}utm_source=yoocanfind.com&utm_medium=referral";
            return RedirectToLocal(returnUrl);            
        }

        #endregion

        private async Task UpdateUserClaims(AltoUser user)
        {
            var claims = await UserManager.GetClaimsAsync(user);
            var givenNameClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName);
            var surnameClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname);
            var claimsToAdd = new List<Claim>();
            var needRefresh = false;

            if (givenNameClaim == null)
            {
                needRefresh = true;
                claimsToAdd.Add(new Claim(ClaimTypes.GivenName, user.FirstName));
            }
            else if (givenNameClaim.Value != user.FirstName)
            {
                needRefresh = true;
                await UserManager.ReplaceClaimAsync(user, givenNameClaim, new Claim(ClaimTypes.GivenName, user.FirstName ?? ""));
            }

            if (surnameClaim == null)
            {
                needRefresh = true;
                claimsToAdd.Add(new Claim(ClaimTypes.Surname, user.LastName));
            }
            else if (surnameClaim.Value != user.LastName)
            {
                needRefresh = true;
                await UserManager.ReplaceClaimAsync(user, surnameClaim, new Claim(ClaimTypes.Surname, user.LastName ?? ""));
            }

            if (claimsToAdd.Any())
                await UserManager.AddClaimsAsync(user, claimsToAdd);

            if (needRefresh)
                await _signInManager.RefreshSignInAsync(user);
        }

        #region Membership State

        private async Task UpdateMembershipState(MembershipState membershipState, DateTime? date = null)
        {
            var user = await UserManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var oldClaims = User.Claims.Where(x => x.Type == UserMembership.MembershipStateClaimType || x.Type == UserMembership.MembershipExpiryDateClaimType).ToList();
            await UserManager.RemoveClaimsAsync(user, oldClaims);
            await UserManager.AddClaimAsync(user, new Claim(UserMembership.MembershipStateClaimType, membershipState.ToString()));
            if (date != null)
                await UserManager.AddClaimAsync(user, new Claim(UserMembership.MembershipExpiryDateClaimType, date.Value.ToString("u", CultureInfo.InvariantCulture)));
            await Context.SaveChangesAsync();
            await _signInManager.RefreshSignInAsync(user);
        }

        #endregion

        #region Payment

        private const decimal FullYearPrice = 59.90M;
        private const decimal PerMonthYearPrice = 4.99M;

        private PaymentModel GetPaymentModel()
        {
            var model = new PaymentModel();

            model.FullYearPrice = FullYearPrice;
            model.PerMonthPrice = PerMonthYearPrice;

            var membershipState = _membershipManager.GetMembershipState();
            var promoSession = _registrationPromoSessionManager.GetRegistrationPromoSession();
            if (promoSession?.PromoType == RegistrationPromoType.DiscountPercentage && promoSession.PromoAmount > 0)
            {
                model.DiscountAmount = (int)(promoSession.PromoAmount * 100);
                model.FullYearPrice = Math.Round(FullYearPrice * (1 - promoSession.PromoAmount), 1);
                model.PerMonthPrice = GetPerMonthPrice(promoSession.PromoAmount);
            }

            if (promoSession?.PromoType == RegistrationPromoType.ExtendedTrial)
            {
                model.ExtendedTrialDuration = (int)promoSession.PromoAmount;
            }

            model.MonthTrialPrice = 4.95M;
            model.IsTrialAvailable = membershipState != MembershipState.Expired && model.FullYearPrice > model.MonthTrialPrice;

            return model;
        }

        private decimal GetPerMonthPrice(decimal discount)
        {
            return Math.Round(PerMonthYearPrice * (1 - discount), 2);
        }

        private const string AnnualMembershipItemName = "Annual Alto membership";
        private const string MonthTrialMembershipItemName = "One month trial Alto membership";

        [HttpPost]
        public IActionResult CreateMembershipPayment(bool isTrial = false)
        {
            if (!Request.IsAjaxRequest())
            {
                Logger.LogWarning("possible csrf attempt");
                return BadRequest();
            }

            var paypalContext = _payPalLogic.GetContext();
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var membershipState = _membershipManager.GetMembershipState();
            if ((membershipState != MembershipState.FilledDetails && membershipState != MembershipState.Expired) ||
                (isTrial && membershipState != MembershipState.FilledDetails))
            {
                Logger.LogError($"User {userId} tried paying for registration even though he has unexpected state {membershipState}");
                return BadRequest();
            }

            //TODO: add proper urls for redirection
            var rootUrl = Url.Action("Index", "Home");

            var paymentModel = GetPaymentModel();
            var totalPriceString = (isTrial ? paymentModel.MonthTrialPrice : paymentModel.FullYearPrice).ToString("0.00");
            var name = isTrial ?
                MonthTrialMembershipItemName +
                                 (paymentModel.ExtendedTrialDuration > 0 ? $" with {paymentModel.ExtendedTrialDuration} months free" : "")
                                 :
                AnnualMembershipItemName;

            var payment = new Payment
            {
                intent = "sale",
                payer = new Payer { payment_method = "paypal" },
                transactions = new List<Transaction> {
                    new Transaction
                                {
                                    description = name,
                                    custom = _registrationPromoSessionManager.GetRegistrationPromoSession()?.RegistrationPromoId.ToString(),
                                    invoice_number = Guid.NewGuid().ToString(),
                                    amount = new Amount
                                    {
                                        currency = "USD",
                                        total = totalPriceString,
                                        details = new Details
                                                    {
                                                        subtotal = totalPriceString
                                                    }
                                    },
                                    item_list = new ItemList
                                                {
                                                    items = new List<Item>
                                                        {
                                                            new Item
                                                            {
                                                                name = name,
                                                                currency = "USD",
                                                                price = totalPriceString,
                                                                quantity = "1"
                                                            }
                                                        }
                                                }
                                }},
                redirect_urls = new RedirectUrls
                {
                    cancel_url = rootUrl,
                    return_url = rootUrl
                },
                experience_profile_id = _payPalLogic.Options.PayPalNoShippingExperienceProfileId
            };

            var createdPayment = payment.Create(paypalContext);
            return Ok(new CreatePaymentResult
            {
                PaymentID = createdPayment.id
            });
        }

        [HttpPost]
        public async Task<IActionResult> ExecuteMembershipPayment(string paymentID, string payerID)
        {
            if (!Request.IsAjaxRequest())
            {
                Logger.LogWarning("possible csrf attempt");
                return BadRequest();
            }

            var paymentExecution = new PaymentExecution { payer_id = payerID };
            var payment = new Payment { id = paymentID };
            var paypalContext = _payPalLogic.GetContext();
            var approvedPayment = Payment.Get(paypalContext, paymentID);

            var membershipDuration = 12;
            var approvedTransaction = approvedPayment.transactions[0];
            var name = approvedTransaction.item_list.items[0].name;
            if (name.StartsWith(MonthTrialMembershipItemName))
            {
                membershipDuration = 1;
                var promoSession = _registrationPromoSessionManager.GetRegistrationPromoSession();
                if (promoSession?.PromoType == RegistrationPromoType.ExtendedTrial)
                {
                    membershipDuration += (int)promoSession.PromoAmount;
                }
            }
            else if (name != AnnualMembershipItemName)
            {
                Logger.LogError("unexpected item name {name} for membership payment", name);
                return BadRequest();
            }
            var promoId = approvedTransaction.custom;
            if (!string.IsNullOrEmpty(promoId) && !await ValidatePromoAndUpdateCounters(int.Parse(promoId)))
            {
                Logger.LogError("Registration PromoId {promoId} was being used despite being invalid", promoId);
                return BadRequest();
            }

            var executedPayment = payment.Execute(paypalContext, paymentExecution);

            //at this point money was taken from buyer's account so have to show him confirmation page
            try
            {
                if (executedPayment.state.ToLower() != "approved")
                {
                    Logger.LogError($"unexpected paypal trasaction state after payment execution : {executedPayment.state} for payment id: {paymentID}, not approving user");
                    return BadRequest();
                }
                var membershipExpiration = DateTime.UtcNow.AddMonths(membershipDuration);
                await UpdateMembershipState(MembershipState.Payed, membershipExpiration);
                await SendMembershipConfirmationEmail(membershipDuration, membershipExpiration,
                    User.FindFirstValue(ClaimTypes.GivenName),
                    User.FindFirstValue(ClaimTypes.Surname));
                _registrationPromoSessionManager.DeleteCookie();
            }
            catch (Exception e)
            {
                Logger.LogError(12162, e, $@"Exception occurred after registration payment executed for user: {User.FindFirstValue(ClaimTypes.NameIdentifier)}, 
view paypal transactions to confirm user. PayPal Payment ID: {paymentID}");
            }

            return Ok(new CreatePaymentResult { PaymentID = paymentID });
        }

        private async Task SendMembershipConfirmationEmail(int membershipDuration, DateTime membershipExpiration, string firstName, string lastName)
        {
            await _emailLogic.SendMembershipConfirmationEmailAsync(new MemberConfirmationData
            {
                UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                Email = User.FindFirstValue(ClaimTypes.Email),
                FirstName = firstName,
                LastName = lastName,
                MembershipDuration = membershipDuration == 12 ? "yearly" : $"{membershipDuration} month trial",
                MembershipExpiration = membershipExpiration
            });
        }

        #endregion

        #region Registration Promo

        [AllowAnonymous]
        [Route("Promo", Name = "Promo")]
        public async Task<IActionResult> Promo(string code)
        {
            var promo = await _registrationPromoSessionManager.TrySetRegistrationPromoCookie(code);
            if (promo == null || promo.Amount <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = Mapper.Map<PromoModel>(promo);

            if (promo.Type == RegistrationPromoType.DiscountPercentage)
                model.PerMonthPrice = GetPerMonthPrice(promo.Amount);

            return View(model);
        }

        private async Task<bool> FinalizeRegistrationIfFreeTrialPromoUser()
        {
            var registrationPromoSession = _registrationPromoSessionManager.GetRegistrationPromoSession();
            if (registrationPromoSession == null ||
                registrationPromoSession.PromoType != RegistrationPromoType.FreeTrial)
            {
                return false;
            }

            if (!await ValidatePromoAndUpdateCounters(registrationPromoSession.RegistrationPromoId))
            {
                return false;
            }

            var membershipDuration = (int)registrationPromoSession.PromoAmount;
            var membershipExpiration = DateTime.UtcNow.AddMonths(membershipDuration);

            await UpdateMembershipState(MembershipState.Payed, membershipExpiration);

            var user = await GetCurrentUserAsync();
            if (user.ReferrerPromoId != registrationPromoSession.RegistrationPromoId)
            {
                user.ReferrerPromoId = registrationPromoSession.RegistrationPromoId;
                await UserManager.UpdateAsync(user);
            }

            await Context.SaveChangesAsync();

            await SendMembershipConfirmationEmail(membershipDuration, membershipExpiration,
                user.FirstName, user.LastName);

            _registrationPromoSessionManager.DeleteCookie();

            return true;
        }

        private async Task<bool> ValidatePromoAndUpdateCounters(int promoId)
        {
            var dbRegistrationPromo = await Context.RegistrationPromos.SingleAsync(x => x.Id == promoId);
            if (dbRegistrationPromo.IsDisabled)
                return false;

            if (dbRegistrationPromo.ReferralsRemaining != null)
            {
                if (dbRegistrationPromo.ReferralsRemaining <= 0)
                {
                    return false;
                }
                dbRegistrationPromo.ReferralsRemaining--;
            }
            //TODO: this is not thread-safe but contention is unlikely currently and the counter is not critical.
            dbRegistrationPromo.ReferralsRegistered++;
            return true;
        }

        #endregion

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        #endregion
    }
}