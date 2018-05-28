using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Yooocan.Dal;
using Yooocan.Entities;
using Yooocan.Logic;
using Yooocan.Models.Vendors;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Collections.Generic;
using System;
using StackExchange.Redis;
using Yooocan.Models.Admin;
using Yooocan.Logic.Extensions;
using System.Text.RegularExpressions;
using Yooocan.Logic.Amazon;
using Yooocan.Web.ActionFilters;
using CsvHelper;
using System.IO;
using Microsoft.Extensions.Caching.Memory;

namespace Yooocan.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        private readonly IAdminLogic _adminLogic;
        private readonly IDatabase _redisDatabase;
        private readonly AmazonLogic _amazonLogic;
        private readonly IMemoryCache _memoryCache;

        public AdminController(ApplicationDbContext context, ILogger<AdminController> logger, IMapper mapper, UserManager<ApplicationUser> userManager, IAdminLogic adminLogic,
            IDatabase redisDatabase, AmazonLogic amazonLogic, IMemoryCache memoryCache) : base(context, logger, mapper, userManager)
        {
            _adminLogic = adminLogic;
            _redisDatabase = redisDatabase;
            _amazonLogic = amazonLogic;
            _memoryCache = memoryCache;
        }

        public ActionResult Dashboard()
        {
            var model = _adminLogic.GetDashboard();
            return View(model);
        }

        public ActionResult SetStoryProductsOld()
        {            
            return OldIframeContainer();
        }

        public async Task<ActionResult> SetStoryProducts()
        {
            var model = await _adminLogic.GetSetStoryProductsDataAsync();
            return OldView(model);
        }

        [HttpPost]
        [ServiceFilter(typeof(CsrfHeadersValidationFilter))]
        public ActionResult SetStoryProductsReviewed(int id)
        {
            var story = Context.Stories.Single(x => x.Id == id);
            story.IsProductsReviewed = true;
            Context.SaveChanges();

            return NoContent();
        }

        [HttpPost]
        [ServiceFilter(typeof(CsrfHeadersValidationFilter))]
        public ActionResult SetStoryNoIndex(int id, bool noIndex)
        {
            var story = Context.Stories.Single(x => x.Id == id);
            story.IsNoIndex = noIndex;
            Context.SaveChanges();

            _redisDatabase.KeyDelete(string.Format(RedisKeys.StoryModel, id));

            return NoContent();
        }

        public ActionResult VendorRegistrationDashboard()
        {
            return OldIframeContainer();
        }
        public async Task<ActionResult> VendorRegistrationDashboardOld()
        {
            var vendorRegistrations = await Context.VendorRegistrations.Where(x => !x.WasHandled)
                                                                 .OrderByDescending(x => x.Id)
                                                                 .ToListAsync();
            var models = Mapper.Map<List<RegisterVendorModel>>(vendorRegistrations);

            return OldView(models);
        }

        [HttpPost]
        [ServiceFilter(typeof(CsrfHeadersValidationFilter))]
        public async Task<ActionResult> DeleteVendorRegistration(int id)
        {
            var vendorRegistration = await Context.VendorRegistrations.SingleAsync(x => x.Id == id);
            vendorRegistration.WasHandled = true;
            await Context.SaveChangesAsync();

            return RedirectToAction("VendorRegistrationDashboard");
        }

        public ActionResult SetVendorCommercialTerms(int id, bool isExisting, bool saveSucceeded)
        {
            return OldIframeContainer();
        }

        public async Task<ActionResult> SetVendorCommercialTermsOld(int id, bool isExisting, bool saveSucceeded)
        {
            Vendor vendor;
            if (isExisting)
                vendor = await Context.Vendors.Where(x => x.Id == id)
                                              .SingleAsync();
            else
            {
                var vendorRegistration = await Context.VendorRegistrations.Where(x => x.Id == id)
                                                                          .SingleAsync();
                vendor = Mapper.Map<Vendor>(vendorRegistration);
                ViewBag.RegistrationId = vendorRegistration.Id;
            }

            var vendorModel = Mapper.Map<VendorModel>(vendor);

            var grantedEmails = new List<string> { vendorModel.ContactPersonEmail };
            if (isExisting)
            {
                grantedEmails = await GetCurrentGrantedEmails(id);
            }
            grantedEmails = grantedEmails.Select(x => x.ToLowerInvariant()).ToList();
            vendorModel.ContactPersonEmail = string.Join(", ", grantedEmails);

            var registeredGrantedUsers = await Context.Users.Where(user => grantedEmails.Contains(user.Email)).AsNoTracking().ToListAsync();
            var notRegisteredEmails = grantedEmails.Except(registeredGrantedUsers.Select(x => x.Email.ToLowerInvariant()))
                                                   .ToList();
            ViewBag.NotRegisteredEmails = notRegisteredEmails;

            var notConfirmedEmails = registeredGrantedUsers.Where(x => !x.EmailConfirmed).ToList();
            ViewBag.NotConfirmedEmails = notConfirmedEmails;

            if (saveSucceeded)
                ViewBag.SaveSucceeded = true;

            return OldView(vendorModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetVendorCommercialTerms(int? registrationId, VendorModel model)
        {
            var vendorFromClient = Mapper.Map<Vendor>(model);
            var isNewVendor = registrationId.HasValue;

            if (isNewVendor)
            {
                ViewBag.RegistrationId = registrationId.Value;
                var vendorRegistration = await Context.VendorRegistrations
                    .Where(x => x.Id == registrationId)
                    .SingleAsync();

                var newMail = vendorFromClient.ContactPersonEmail;
                Mapper.Map(vendorRegistration, vendorFromClient);
                vendorFromClient.ContactPersonEmail = newMail;

                if (Context.Vendors.Any(x => x.Name == vendorFromClient.Name))
                {
                    ModelState.AddModelError("Name",
                                            $"Vendor name already exists: {vendorFromClient.Name}");
                    return OldView(model);
                }

                Context.Vendors.Add(vendorFromClient);
                vendorRegistration.WasHandled = true;
                await Context.SaveChangesAsync();
            }

            var vendorFromDb = registrationId != null
                ? vendorFromClient
                : await Context.Vendors.Where(x => x.Id == vendorFromClient.Id).SingleAsync();

            vendorFromDb.CommercialTermsOther = vendorFromClient.CommercialTermsOther;
            vendorFromDb.CommercialTermsRate = vendorFromClient.CommercialTermsRate;
            vendorFromDb.CommercialTerms = vendorFromClient.CommercialTerms;
            vendorFromDb.LastUpdateDate = DateTime.UtcNow;
            await Context.SaveChangesAsync();

            var currentGrantedEmails = await GetCurrentGrantedEmails(vendorFromDb.Id);

            var requestedGrantedEmails = vendorFromClient.ContactPersonEmail.Split(',')
                                                                                    .Select(x => x.Trim().ToLowerInvariant())
                                                                                    .ToList();
            var emailsToGrant = requestedGrantedEmails.Except(currentGrantedEmails)
                                                              .ToList();

            var emailsToRevoke = currentGrantedEmails.Except(requestedGrantedEmails).ToList();

            foreach (var email in emailsToGrant)
            {
                await AddVendorClaimToUserAsync(email, vendorFromDb.Id);
            }
            foreach (var email in emailsToRevoke)
            {
                await RevokeVendorClaimFromUserAsync(email, vendorFromDb.Id);
            }

            return RedirectToAction(nameof(SetVendorCommercialTermsOld), new { id = vendorFromDb.Id, isExisting = true, saveSucceeded = true });
        }

        private async Task<List<string>> GetCurrentGrantedEmails(int vendorId)
        {
            var currentVendorClaims = await Context.Users.Where(x => x.Claims.Any(claim => claim.ClaimType == "vendor" &&
                                                                                           claim.ClaimValue == vendorId.ToString()))
                                                                             .Select(x => x.Email.ToLowerInvariant())
                                                         .ToListAsync();
            var currentVendorPendingClaims = await Context.PendingClaims.Where(x => x.ClaimType == "vendor" &&
                                                                                    x.ClaimValue == vendorId.ToString() &&
                                                                                    !x.WasAssigned)
                                                                        .Select(x => x.Email.ToLowerInvariant())
                                                                        .ToListAsync();
            var currentGrantedEmails = currentVendorPendingClaims.Union(currentVendorClaims).Distinct();

            return currentGrantedEmails.ToList();
        }

        private async Task AddVendorClaimToUserAsync(string email, int vendorId)
        {
            var claim = new Claim("vendor", vendorId.ToString());
            var user = await UserManager.FindByEmailAsync(email);
            if (user == null || !user.EmailConfirmed)
            {
                var pendingClaim = new PendingClaim
                {
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value,
                    CreatedById = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                    Email = email.ToLower()
                };
                Context.PendingClaims.Add(pendingClaim);
            }
            else
            {
                await UserManager.AddClaimAsync(user, claim);
            }

            await Context.SaveChangesAsync();
        }

        private async Task RevokeVendorClaimFromUserAsync(string email, int vendorId)
        {
            var claim = new Claim("vendor", vendorId.ToString());
            var user = await UserManager.FindByEmailAsync(email);
            if (user == null || !user.EmailConfirmed)
            {
                var pendingClaims = await Context.PendingClaims.Where(x => x.Email.ToLowerInvariant() == email).ToListAsync();
                pendingClaims.ForEach(c => Context.PendingClaims.Remove(c));
            }
            else
            {
                await UserManager.RemoveClaimAsync(user, claim);
            }

            await Context.SaveChangesAsync();

        }

        public ActionResult StoryOfTheDay()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StoryOfTheDay(int id, string text, string recipient)
        {
            var story = await Context.Stories.SingleOrDefaultAsync(x => x.Id == id && x.IsPublished && !x.IsDeleted);
            if (story == null)
            {
                ModelState.AddModelError(string.Empty, "Story not found");
                return View(id);
            }
            if (text != null)
            {
                text = Regex.Replace(text, @"\r\n?|\n", "<br/>"); 
            }
            var storyUrl = Url.RouteUrl("story", new
                                                 {
                                                     id = id,
                                                     title = story.Title.ToCanonical(),
                                                     utm_source = "Story of the day",
                                                     utm_medium = "email",
                                                     utm_campaign = "Story of the day"
                                                 }, "https");
            await _adminLogic.SendStoryOfTheDayAsync(id, text, storyUrl, recipient);

            return RedirectToLocal("/");
        }

        public async Task<ActionResult> StoryServiceProviders(int id)
        {
            var story = await Context.Stories.Include(x => x.StoryServiceProviders).SingleOrDefaultAsync(x => x.Id == id && x.IsPublished && !x.IsDeleted);
            if (story == null)
            {
                return NotFound();
            }

            ViewBag.Title = story.Title;
            ViewBag.Id = story.Id;
            ViewBag.ServiceProviders = string.Join(", ", story.StoryServiceProviders.OrderBy(x => x.Order).Select(x => x.ServiceProviderId));
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> StoryServiceProviders(int id, string serviceProviderIds)
        {
            var story = await _adminLogic.SetStoryServiceProvidersAsync(id, serviceProviderIds);
            return RedirectToRoute("Story", new { story.Id, story.Title });
        }

        public async Task<ActionResult> FeaturedStories()
        {
            var data = await Context.FeaturedStories
                .Select(x => new FeaturedStoryModel
                             {
                                 StoryId = x.Story.Id,
                                 Title = x.Story.Title,
                                 FeaturedType = x.FeaturedType
                             })
                .ToListAsync();
            
            return View(data);
        }

        [HttpPost]
        [ServiceFilter(typeof(CsrfHeadersValidationFilter))]
        public async Task<ActionResult> FeaturedStories([FromBody] List<FeaturedStoryModel> data)
        {
            if (data.Count == 0)
            {
                return BadRequest("Empty list");
            }

            Context.RemoveRange(Context.FeaturedStories.ToList());
            var featuredStories = data.Select(x => new FeaturedStory
                                                   {
                                                       StoryId = x.StoryId,
                                                       FeaturedType = x.FeaturedType
                                                   }).ToList();
            Context.FeaturedStories.AddRange(featuredStories);
            await Context.SaveChangesAsync();

            return Ok();
        }

        public ActionResult DeleteCache()
        {
            return View();
        }

        [HttpPost]
        [ServiceFilter(typeof(CsrfHeadersValidationFilter))]
        public IActionResult DeleteCache(string resource)
        {
            _redisDatabase.KeyDelete(resource);
            return View();
        }

        public IActionResult AddAmazonProducts()
        {
            return View(new AddAmazonProductModel());
        }

        public IActionResult RefreshAmazonProducts()
        {
            var result = _amazonLogic.RefreshData();
            return Ok($"{result.ChangedImages} product images changed, {result.OutOfStockProducts} out of stock products found");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddAmazonProducts(AddAmazonProductModel model)
        {
            var result = _amazonLogic.AddAmazonProduct(model.Asin);
            model.AmazonVendorId = result.AmazonVendorId;
            model.ProductId = result.ProductId;

            return View(model);
        }

        [Route("ConfirmedUsersEmails.csv", Name = nameof(ConfirmedUsersEmails))]
        public async Task<IActionResult> ConfirmedUsersEmails()
        {
            var cachedCsv = await _memoryCache.GetOrCreateAsync(nameof(ConfirmedUsersEmails), async (entry) =>
            {
                var users = await Context.Users.Where(x => x.EmailConfirmed)
                                                .OrderBy(x => x.InsertDate)
                                                .Select(x => new { x.Email, x.FirstName, x.LastName, x.InsertDate })                                                
                                                .ToListAsync();
                var memoryStream = new MemoryStream();
                var csvWriter = new CsvWriter(new StreamWriter(memoryStream) { AutoFlush = true });
                csvWriter.WriteRecords(users);
                csvWriter.Flush();
                var csv = memoryStream.ToArray();

                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(15));
                entry.SetValue(csv);
                return csv;
            });
            return File(cachedCsv, "text/csv");
        }
    }
}
