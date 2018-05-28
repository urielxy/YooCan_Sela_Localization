using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Alto.Dal;
using Alto.Domain;
using Alto.Domain.Benefits;
using Alto.Domain.Companies;
using Alto.Domain.Products;
using Alto.Enums;
using Alto.Enums.Company;
using Alto.Logic.Categories;
using Alto.Logic.Extensions;
using Alto.Logic.Messaging;
using Alto.Logic.Upload;
using Alto.Models.Categories;
using Alto.Models.Home;
using Alto.Models.Products;
using Alto.Web.ActionFilters;
using Alto.Web.Utils;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Alto.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IHomeLogic _homeLogic;
        private readonly IProductLogic _productLogic;
        private readonly ICategoryLogic _categoryLogic;

        public HomeController(AltoDbContext context,
            MapperConfiguration mapperConfiguration,
            UserManager<AltoUser> userManager,
            ILogger<BaseController> logger,
            IHomeLogic homeLogic,
            IProductLogic productLogic,
            ICategoryLogic categoryLogic) : base(context, mapperConfiguration, userManager, logger)
        {
            _homeLogic = homeLogic;
            _productLogic = productLogic;
            _categoryLogic = categoryLogic;
        }

        [TypeFilter(typeof(PromoRegistrationFilter), Arguments = new object[] { true })]
        public async Task<IActionResult> Index()
        {
            HomeModel model;
            if (IsFirstHomePageImpression())
            {
                model = await _homeLogic.GetModelAsync();
            }
            else
            {
                model = await _homeLogic.GetRandomModelAsync();
            }

            ViewBag.IsMenuInPage = true;
            ViewBag.IsSearchBarInPage = true;
            return View(model);
        }

        private bool IsFirstHomePageImpression()
        {
            const string cookieName = "hpi";
            if (!Request.Cookies[cookieName].IsNullOrEmpty())
                return false;

            Response.Cookies.Append(cookieName, DateTimeOffset.UtcNow.Ticks.ToString(), new CookieOptions { Expires = DateTimeOffset.Now.AddYears(2) });
            return true;
        }

        [Route("WhyAlto")]
        public IActionResult WhyAlto()
        {
            return View();
        }

        [Route("Import")]
        [NonAction]
        public IActionResult Import()
        {
            var vendors = Context.ImportVendors
                .Where(x => !x.IsDeleted)
                .Include(x => x.Products)
                .ThenInclude(x => x.Images)
                .AsNoTracking()
                .ToList();

            var companies = vendors
                .Select(v => new Company
                {
                    About = v.About,
                    Address = v.LocationText,
                    BusinessType = BusinessType.ResellerDealer,
                    ContactPersons = v.ContactPersonName == null || v.ContactPersonEmail == null
                                     ? null
                                     : new List<CompanyContactPerson>
                                       {
                                           new CompanyContactPerson
                                           {
                                               Email = v.ContactPersonEmail,
                                               MobileNumber = v.ContactPersonPhoneNumber,
                                               Position = v.ContactPersonPosition,
                                               Skype = v.ContactPersonSkype,
                                               Name = v.ContactPersonName
                                           }
                                       },
                    Images = new List<CompanyImage>
                                          {
                                              new CompanyImage
                                              {
                                                  CdnUrl = v.LogoUrl,
                                                  Type = ImageType.Logo,
                                                  Url = v.LogoUrl
                                              }
                                          },
                    Name = v.Name,
                    FaxNumber = v.FaxNumber,
                    WebsiteUrl = v.WebsiteUrl,
                    TollFreeNumber = v.TollFreeNumber,
                    Products = v.Products
                                     .Where(x => !x.IsDeleted)
                                     .Select(p => new Product
                                     {
                                         Name = p.Name,
                                         Brand = p.Brand,
                                         Colors = p.Colors,
                                         Depth = p.Depth,
                                         Description = p.About + "<br>" + p.Specifications,
                                         YouTubeId = p.YouTubeId,
                                         Width = p.Width,
                                         Height = p.Height,
                                         Price = p.Price,
                                         Upc = p.Upc,
                                         Url = p.Url,
                                         WarrantyUrl = p.WarrentyUrl,
                                         Weight = p.Weight,
                                         Images = p.Images
                                                          .Where(x => !x.IsDeleted)
                                                          .Select(pi => new ProductImage
                                                          {
                                                              Url = pi.Url,
                                                              CdnUrl = pi.CdnUrl,
                                                              Order = pi.Order,
                                                              Type = ConvertImageType(pi.Type)
                                                          }).ToList()
                                     }).ToList()
                });

            Context.Companies.AddRange(companies);
            Context.SaveChanges();
            return NoContent();
        }

        private ImageType ConvertImageType(Domain.Imports.ImageType other)
        {
            switch (other)
            {
                case Domain.Imports.ImageType.Brand:
                    return ImageType.Brand;
                case Domain.Imports.ImageType.Normal:
                    return ImageType.Normal;
                case Domain.Imports.ImageType.Primary:
                    return ImageType.Main;
                default:
                    return ImageType.Normal;
            }
        }

        [Route("Menu")]
        public async Task<IActionResult> Menu(bool showProducts = true)
        {
            ViewBag.IsMenu = true;
            var model = await _categoryLogic.GetMenuCategories(showProducts);
            return View("MenuMobile", model);
        }

        public IActionResult Landing()
        {
            return View();
        }

        [Route("About", Name = "About")]
        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        [Route("TermsOfUse", Name = "TermsOfUse")]
        public IActionResult TermsOfUse()
        {
            return View();
        }

        [Route("PrivacyPolicy", Name = "PrivacyPolicy")]
        public IActionResult PrivacyPolicy()
        {
            return View();
        }

        public ActionResult ExcludeFromTracking()
        {
            if (!Request.Cookies.ContainsKey("removeTracking"))
                Response.Cookies.Append("removeTracking", "true", new CookieOptions { Expires = DateTimeOffset.MaxValue });

            return Content("<html><body><h1>You will not be tracked from now on...</h1></body></html>", "text/html");
        }

        [NonAction]
        public async Task<IActionResult> ChangeStorage()
        {
            await Response.WriteAsync(new string(' ', 5000));
            Response.Body.Flush();
            var isDone = false;
            var task = Task.Run(() =>
            {
                while (!isDone)
                {
                    Response.WriteAsync(" ");
                    Response.Body.Flush();
                    Thread.Sleep(5000);
                }
            });

            await _productLogic.ChangeStorage();
            isDone = true;
            await task;
            return Ok();
        }

        [NonAction]
        public IActionResult GetStaticImagesForCompression()
        {
            var images = Context.CategoryImages.Select(x => x.Url).Distinct().ToList();
            images.AddRange(Context.BenefitImages.Select(x => x.Url).Distinct().ToList());
            return View(images);
        }
    }
}
