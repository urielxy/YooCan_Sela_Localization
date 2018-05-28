using System;
using System.Collections.Generic;
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
using Yooocan.Entities.Companies;
using Yooocan.Enums;
using Yooocan.Logic;
using Yooocan.Logic.Messaging;
using Yooocan.Models.Categories;
using Yooocan.Models.Company;
using Yooocan.Web.ActionFilters;

namespace Yooocan.Web.Controllers
{
    public class CompanyController : BaseController
    {
        private IBlobUploader Uploader { get; }
        private SignInManager<ApplicationUser> SignInManager { get; }
        private IEmailSender EmailSender { get; }

        private const string ContainerName = "vendors";

        public CompanyController(ApplicationDbContext context, IMapper mapperConfiguration, UserManager<ApplicationUser> userManager, ILogger<BaseController> logger,
                                 IBlobUploader uploader, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender) : base(context, logger, mapperConfiguration, userManager)
        {
            Uploader = uploader;
            SignInManager = signInManager;
            EmailSender = emailSender;
        }

        public IActionResult Register()
        {
            var model = new CompanyRegisterModel { MainCategories = GetMainCategories() };

            return View(model);
        }

        [HttpPost]
        [ServiceFilter(typeof(CsrfHeadersValidationFilter))]
        public async Task<IActionResult> Register(CompanyRegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                model.MainCategories = GetMainCategories();
                return View(model);
            }

            var company = Mapper.Map<Company>(model);

            var logoUrl = await Uploader.UploadDataUriImage(model.LogoDataUri, ContainerName);
            company.Images.Add(new CompanyImage { Url = logoUrl, Type = AltoImageType.Logo });

            Context.Companies.Add(company);

            //if (!User.IsInRole("Admin") && User.FindFirstValue(ClaimTypes.Email) != model.Email)
            //{
            //    var result = await CreateUser(model.Email, model.Password);
            //    if (!result.Succeeded)
            //    {
            //        model.MainCategories = GetMainCategories();
            //        ModelState.AddModelError(nameof(model.Email), result.Errors.First().Description);
            //        return View(model);
            //    }
            //    await UserManager.AddClaimAsync(result, new Claim("vendor", pendingClaim.ClaimValue));
            //}

            await Context.SaveChangesAsync();
            try
            {
                await EmailSender.SendEmailAsync(null, "moshe@yoocantech.com", "New vendor registered!", Url.Action(nameof(Edit), "Company", new { company.Id }, "https"), "company-registered", null);
            }
            catch(Exception e)
            {
                Logger.LogError(e, "failed to send notification email on company registration");
            }

            return RedirectToAction("RegistrationConfirmation");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var model = new CompanyEditModel { MainCategories = GetMainCategories() };
            var company = await Context.Companies.Where(x => x.Id == id)
                                                .Include(x => x.ContactPersons)
                                                .Include(x => x.Categories)
                                                .Include(x => x.Images)
                                                .SingleAsync();
            Mapper.Map(company, model);
            if (!model.ContactPersons.Any())
            {
                model.ContactPersons.Add(new ContactPersonModel());
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CompanyEditModel model)
        {
            model.MainCategories = GetMainCategories();

            if (!ModelState.IsValid)
            {                
                return View(model);
            }

            var company = await Context.Companies.Where(x => x.Id == id)
                                                .Include(x => x.ContactPersons)
                                                .Include(x => x.Categories)
                                                .Include(x => x.Images)
                                                .SingleAsync();
            Mapper.Map(model, company);

            if (!string.IsNullOrWhiteSpace(model.LogoDataUri))
            {
                var logoUrl = await Uploader.UploadDataUriImage(model.LogoDataUri, ContainerName);
                company.Images.Single(c => c.Type == AltoImageType.Logo && c.DeleteDate == null).DeleteDate = DateTime.UtcNow;
                company.Images.Add(new CompanyImage {Url = logoUrl, CdnUrl = logoUrl, Type = AltoImageType.Logo});
                model.LogoUri = logoUrl;
            }

            await Context.SaveChangesAsync();

            return View(model);
        }

        public IActionResult RegistrationConfirmation()
        {
            return View();
        }

        private async Task<IdentityResult> CreateUser(string email, string password)
        {
            var user = new ApplicationUser { UserName = email, Email = email };
            var result = await UserManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                var claim = new Claim(ClaimTypes.Email, email);
                await UserManager.AddClaimAsync(user, claim);
                await SignInManager.SignInAsync(user, true);
            }
            else
            {
                Logger.LogError($"Failed creating user for company with email: {email}, error: {result.Errors.First().Description}");
            }
            return result;
        }

        private List<AltoCategoryModel> GetMainCategories()
        {
            return Context.AltoCategories.Where(x => x.ParentCategory == null && !x.IsDeleted)
                                                    .Select(x => new AltoCategoryModel { Id = x.Id, Name = x.Name })
                                                    .ToList();
        }
    }
}
