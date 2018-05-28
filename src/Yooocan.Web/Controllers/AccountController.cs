using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Yooocan.Entities;
using Yooocan.Dal;
using Yooocan.Logic;
using Yooocan.Logic.Messaging;
using Yooocan.Models;
using Yooocan.Web.Models.AccountViewModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using AutoMapper;
using Yooocan.Logic.Extensions;
using Yooocan.Web.Utils;
using Microsoft.AspNetCore.Authentication;
using Yooocan.Web.ActionFilters;

namespace Yooocan.Web.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly IEmailLogic _emailLogic;
        private readonly IGoogleAnalyticsLogic _googleAnalyticsLogic;
        private readonly IBlobUploader _blobUploader;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ISmsSender smsSender,
            ApplicationDbContext context,
            IEmailLogic emailLogic,
            IGoogleAnalyticsLogic googleAnalyticsLogic,
            ILogger<AccountController> logger,
            IBlobUploader blobUploader,
            IMapper mapper) : base(context, logger, mapper, userManager)
        {
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _emailLogic = emailLogic;
            _googleAnalyticsLogic = googleAnalyticsLogic;
            _blobUploader = blobUploader;
        }

        //
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            await HttpContext.SignOutAsync();
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            model.Email= model.Email.Trim();
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, true, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    Logger.LogInformation(1, "User logged in.");
                    if (Context.PendingClaims.Any(x => x.Email == model.Email && !x.WasAssigned))
                    {
                        var user = UserManager.Users.Single(x => x.Email == model.Email);
                        var code = await UserManager.GenerateEmailConfirmationTokenAsync(user);
                        var callbackUrl = Url.Action("ConfirmEmail", "Account", new {userId = user.Id, code}, protocol: HttpContext.Request.Scheme);
                        await _emailLogic.SendConfirmEmailAsync(user.Email, user.Id, callbackUrl);
                        await _emailSender.SendEmailAsync(null, "yoav@yoocantech.com", "Pending email confirmation",
                            $"{model.Email} tried to login, he needs to verify his email account first!", "Pending claim Yoav", null);
                    }

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

        //
        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
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
                var user = new ApplicationUser {UserName = model.Email, Email = model.Email};
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var claim = new Claim(ClaimTypes.Email, model.Email.Trim());
                    await UserManager.AddClaimAsync(user, claim);
                    var code = await UserManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new {userId = user.Id, code = code}, protocol: HttpContext.Request.Scheme);

                    await _emailLogic.SendConfirmEmailAsync(model.Email, user.Id, callbackUrl);

                    await _signInManager.SignInAsync(user, isPersistent: true);
                    Logger.LogInformation(3, "User created a new account with password.");
                    _googleAnalyticsLogic.TrackEvent(user.Id, "Signup", "Signup", "Email");
                    if (Request.IsAjaxRequest())
                        return Ok();

                    return RedirectToLocal(returnUrl);
                }

                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            if (Request.IsAjaxRequest())
                return ReturnAjaxErrors();

            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ServiceFilter(typeof(CsrfHeadersValidationFilter))]
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
        [ServiceFilter(typeof(CsrfHeadersValidationFilter))]
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
            const string closePopup = "if(window.opener) {window.close();}";
            var successResponsePopup = returnUrl == null
                ? $"<html><head><script>(window.opener || window).location.reload();{closePopup}</script></head></html>"
                : $"<html><head><script>(window.opener || window).location='{returnUrl}{(returnUrl.Contains("?") ? "&" : "?")}cb=' + new Date().getTime();{closePopup}</script></head></html>";

            var errorLink = Url.Action(nameof(Login), "Account", null, HttpContext.Request.Scheme);
            if (remoteError != null)
            {
                if (isPopup)
                    return Content($"<html><head><script>(window.opener || window).location={errorLink};{closePopup}</script></head><body></body></html>", "text/html");

                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View(nameof(Login));
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null && isPopup)
                return Content($"<html><head><script>(window.opener || window).location={errorLink};{closePopup}</script></head><body></body></html>", "text/html");

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
            var gender = info.Principal.FindFirst(ClaimTypes.Gender)?.Value;
            var firstName = info.Principal.FindFirst(ClaimTypes.GivenName)?.Value;
            var lastName = info.Principal.FindFirst(ClaimTypes.Surname)?.Value;
            var email = info.Principal.FindFirst(ClaimTypes.Email)?.Value;

            // If we didn't get email permissions
            if (email == null && isPopup)
                return Content($"<html><head><script>(window.opener || window).location={errorLink};{closePopup}</script></head><body></body></html>", "text/html");
            if (email == null)
                return RedirectToAction(nameof(Login));

            var normalizedEmail = email.Normalize();

            var user = await UserManager.Users.Include(x => x.Claims).SingleOrDefaultAsync(x => x.NormalizedEmail == normalizedEmail);
            if (user != null)
            {
                Logger.LogInformation("{name} {email} exist in the DB", info.Principal.Identity.Name, info.Principal.FindFirst(ClaimTypes.Email));
            }
            else
            {
                Logger.LogInformation("{name} {email} doesn't exist in the DB", info.Principal.Identity.Name,
                    info.Principal.FindFirst(ClaimTypes.Email));

                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    PictureUrl = picture,
                    FirstName = firstName,
                    LastName = lastName, 
                    Gender = gender == "male" ? Gender.Male : gender == "female" ? (Gender?)Gender.Female : null,
                    EmailConfirmed = true
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
                _googleAnalyticsLogic.TrackEvent(user.Id, "Signup", "Signup", info.LoginProvider);
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
                                picture = await _blobUploader.UploadStreamAsync(stream, "user-images", Guid.NewGuid().ToString("N"), quality: 100);
                            }
                        }

                        claimsToAdd.Remove(pictureClaim);
                        claimsToAdd.Add(new Claim("picture", picture));
                        user.PictureUrl = picture;
                        await UserManager.UpdateAsync(user);
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError(432412, e, "Something went wrong with getting the avatar of {email}", user.Email);
                }
                await UserManager.AddClaimsAsync(user, claimsToAdd);
                await AddPendingClaimsAsync(user);
                await _signInManager.SignInAsync(user, isPersistent: true);
                Logger.LogInformation(6, "User created an account using {Name} provider.", info.LoginProvider);

                if (isPopup)
                {
                    return Content(successResponsePopup, "text/html");
                }
                return RedirectToLocal(returnUrl);
            }

            Logger.LogInformation("Can't login external login --not implemented {email} {provider}", info.Principal.Identity.Name, info.LoginProvider);
            return Content($"<html><head><script>(window.opener || window).location={errorLink};{closePopup}</script></head><body></body></html>", "text/html");
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
                var gender = info.Principal.FindFirst(ClaimTypes.Gender)?.Value;
                var firstName = info.Principal.FindFirst(ClaimTypes.GivenName)?.Value;
                var lastName = info.Principal.FindFirst(ClaimTypes.Surname)?.Value;
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    PictureUrl = picture,
                    FirstName = firstName,
                    LastName = lastName,
                    Gender = gender == "male" ? Gender.Male : gender == "female" ? (Gender?)Gender.Female : null,
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
        public async Task<IActionResult> ConfirmEmail([Required]string userId, [Required] string code)
        {
            if (userId == null || code == null)
                ModelState.AddModelError(string.Empty, $"{nameof(userId)} or {nameof(code)} were empty");

            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(userId);
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
                        _googleAnalyticsLogic.TrackEvent(userId, "User", "Confirm email", null);

                        await AddPendingClaimsAsync(user);

                        if (User.Identity.IsAuthenticated && User.FindFirst(ClaimTypes.NameIdentifier).Value == userId)
                            await _signInManager.RefreshSignInAsync(user);

                        return RedirectToAction("Edit", "User");
                    }

                    AddErrors(result);
                }
            }

            LogModelStateErrors();

            return View("Error");
        }

        private async Task AddPendingClaimsAsync(ApplicationUser user)
        {
            var pendingClaims = Context.PendingClaims.Where(x => x.Email == user.Email && !x.WasAssigned)
                                                      .ToList();
            foreach (var pendingClaim in pendingClaims)
            {
                await UserManager.AddClaimAsync(user, new Claim(pendingClaim.ClaimType, pendingClaim.ClaimValue));
                pendingClaim.LastUpdateDate = DateTime.UtcNow;
                pendingClaim.WasAssigned = true;
            }

            await Context.SaveChangesAsync();
        }

        private new void LogModelStateErrors()
        {
            var state = ModelState.Where(x => x.Value.Errors.Any())
                    .Select(x => new KeyValuePair<string, object>(string.IsNullOrWhiteSpace(x.Key) ? "General" : x.Key,
                    string.Join(",", x.Value.Errors.Select(e => e.ErrorMessage))));
            Logger.Log(LogLevel.Warning, 123, state, null, null);
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
                    Logger.LogWarning("{email} doesn't exist for reset password ", model.Email);
                    ModelState.AddModelError(nameof(model.Email), $"The email {model.Email} does not exist in the system.");

                    if (Request.IsAjaxRequest())
                        return ReturnAjaxErrors();
                    return View();
                }

                var code = await UserManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                var userData = new EmailUserData
                {
                    Email = user.Email,
                    UserId = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    ProfileImageUrl = user.PictureUrl
                };

                var isSuccess = await _emailLogic.SendResetPasswordEmailAsync(userData, callbackUrl);
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
        public IActionResult ResetPassword(string code = null)
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