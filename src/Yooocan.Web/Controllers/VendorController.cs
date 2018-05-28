using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Yooocan.Dal;
using Yooocan.Entities;
using Yooocan.Logic;
using Yooocan.Logic.Messaging;
using Yooocan.Models;
using Yooocan.Models.Vendors;
using Yooocan.Web.ActionFilters;

namespace Yooocan.Web.Controllers
{
    [Authorize]
    public class VendorController : BaseController
    {
        private readonly IEmailSender _emailSender;
        private readonly IOldProductLogic _productLogic;

        public VendorController(ApplicationDbContext context, ILogger<VendorController> logger, IMapper mapper, UserManager<ApplicationUser> userManager,
            IEmailSender emailSender, IOldProductLogic productLogic) : base(context, logger, mapper, userManager)
        {
            _emailSender = emailSender;
            _productLogic = productLogic;
        }

        #region Admin Actions
    
        [Authorize(Roles = "Admin")]
        public ActionResult Index(bool showDeleted = false)
        {
            return RedirectToAction("Index", "Company");
            //return OldIframeContainer();
        }
    
        [Authorize(Roles = "Admin")]
        public ActionResult IndexOld(bool showDeleted = false)
        {
            ViewBag.ShowDeleted = showDeleted;

            var query = Context.Vendors.Where(x => x.AltoId != null).AsQueryable();
            if (!showDeleted)
            {
                query = query.Where(x => !x.IsDeleted);
            }
            var vendors = query.Include(x => x.Products)
                               .ToList();
            var models = Mapper.Map<IEnumerable<VendorListModel>>(vendors);
            return OldView(models);
        }

        // GET: Vendor/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Vendor/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return OldIframeContainer();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult CreateOld()
        {
            return OldView();
        }

        // POST: Vendor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(CreateVendorModel model)
        {
            try
            {
                if (Context.Vendors.Any(x => x.Name == model.Name))
                {
                    ModelState.AddModelError("Name", "Vendor name already exists");
                    return OldView(model);
                }

                var vendor = Mapper.Map<Vendor>(model);
                Context.Vendors.Add(vendor);
                Context.SaveChanges();
                //return RedirectToAction("Details", new {id = vendor.Id});
                return RedirectToAction("Index");
            }
            catch
            {
                return OldView();
            }
        }

        // GET: Vendor/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Vendor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ServiceFilter(typeof(CsrfHeadersValidationFilter))]
        public async Task<ActionResult> Delete(int id)
        {
            return await ToggleDelete(id, true);           
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ServiceFilter(typeof(CsrfHeadersValidationFilter))]
        public async Task<ActionResult> Undelete(int id)
        {
            return await ToggleDelete(id, false);
        }

        private async Task<ActionResult> ToggleDelete(int id, bool delete)
        {
            var vendor = await Context.Vendors.Include(x => x.Products)
                                              .SingleAsync(x => x.Id == id);
            vendor.IsDeleted = delete;
            vendor.LastUpdateDate = DateTime.UtcNow;

            foreach (var product in vendor.Products)
            {
                product.IsDeleted = delete;
                product.IsPublished = false;
                product.LastUpdateDate = DateTime.UtcNow;
            }

            await Context.SaveChangesAsync();

            return Redirect(Request.Headers["Referer"]);
        }

        #endregion

        #region SignUp - Registration

        [Authorize(Policy = "MyVendor")]
        public ActionResult SignUp(int id)
        {
            return OldIframeContainer();
        }

        // GET: Vendor/SignUpOld/5
        [Authorize(Policy = "MyVendor")]
        public async Task<ActionResult> SignUpOld(int id)
        {
            var vendor = await Context.Vendors.Where(x => x.Id == id)
                                              .SingleOrDefaultAsync();
            if (vendor == null)
            {
                Logger.LogWarning($"Vendor id {id} not found");

                return NotFound();
            }

            var vendorModel = Mapper.Map<VendorModel>(vendor);

            return OldView(vendorModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "MyVendor")]
        public async Task<ActionResult> SignUp(int id, VendorModel model)
        {
            var vendorFromDb = await Context.Vendors.Where(x => x.Id == id)
                                                    .SingleOrDefaultAsync();

            model.Id = vendorFromDb.Id;
            model.CommercialTerms = vendorFromDb.CommercialTerms;
            model.CommercialTermsOther = vendorFromDb.CommercialTermsOther;
            model.CommercialTermsRate = vendorFromDb.CommercialTermsRate;

            Mapper.Map(model, vendorFromDb);

            if (!User.IsInRole("Admin"))
            {
                vendorFromDb.OnBoardingContactPersonEmail = model.ContactPersonEmail;
                vendorFromDb.OnBoardingDate = DateTime.UtcNow;
            }
            vendorFromDb.LastUpdateDate = DateTime.UtcNow;

            await Context.SaveChangesAsync();

            return User.IsInRole("Admin") ? RedirectToAction(nameof(MyProducts), new {Id = id}) : RedirectToAction(nameof(ConfirmSignUp));
        }

        public ActionResult ConfirmSignUp()
        {
            return OldView();
        }

        #endregion

        #region MyProducts

        [Authorize]
        public async Task<ActionResult> MyProductsOld()
        {
            var vendorId = int.Parse(User.FindFirst("vendor").Value);
            var model = await _productLogic.GetMyProducts(vendorId);
            if (model == null)
                return NotFound();

            return OldView(model);
        }

        public ActionResult MyProducts(int? id)
        {
            return OldIframeContainer();
        }

        [Route("Vendor/MyProductsOld/{id:int}")]
        [Authorize(Policy = "MyVendor")]
        public async Task<ActionResult> MyProductsOld(int id)
        {
            var model = await _productLogic.GetMyProducts(id);
            return model == null ? NotFound() : OldView(model);
        }

        #endregion

        #region Pre Registration
        [AllowAnonymous]
        public ActionResult Register()
        {
            return RedirectToAction("Register", "Company");
            //return OldIframeContainer();
        }

        [AllowAnonymous]
        public ActionResult RegisterOld()
        {
            return OldView();
        }

        [HttpPost]
        [ServiceFilter(typeof(CsrfHeadersValidationFilter))]
        [AllowAnonymous]
        public async Task<ActionResult> RegisterOld(RegisterVendorModel model)
        {
            if (ModelState.IsValid)
            {
                var vendorRegistration = Mapper.Map<VendorRegistration>(model);
                Context.VendorRegistrations.Add(vendorRegistration);
                Context.SaveChanges();

                var content = JsonConvert.SerializeObject(model, Formatting.Indented, new Newtonsoft.Json.Converters.StringEnumConverter());
                await _emailSender.SendEmailAsync(null, "moshe@yoocantech.com", "Vendor registered", content, "VendorRegistered", null);

                return RedirectToAction(nameof(RegisterConfirmation));
            }

            LogModelStateErrors();

            return OldView(model);
        }

        public ActionResult RegisterConfirmation()
        {
            return OldView();
        }
        #endregion
    }
}