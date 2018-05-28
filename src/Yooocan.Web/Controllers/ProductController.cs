using System.Linq;
using System.Threading.Tasks;
using Yooocan.Dal;
using Yooocan.Entities;
using Yooocan.Logic.Extensions;
using Yooocan.Logic.Products;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Yooocan.Web.ActionFilters;

namespace Yooocan.Web.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IProductLogic _productLogic;

        public ProductController(ApplicationDbContext context, IMapper mapperConfiguration, UserManager<ApplicationUser> userManager, ILogger<BaseController> logger
            ,IProductLogic productLogic) : base(context, logger, mapperConfiguration, userManager)
        {
            _productLogic = productLogic;
        }

        [Route("Product/{id:int}/{name?}", Name = "Product")]
        public async Task<IActionResult> Index(int id, string name)
        {
            var productName = await Context.Products.Where(x => x.Id == id && x.CompanyId != null).Select(x => x.Name).SingleOrDefaultAsync();
            if (productName == null)
                return NotFound();
            if (productName.ToCanonical() != name)
                return RedirectToRoutePermanent("Product", new { id, name = productName.ToCanonical() });

            var model = await _productLogic.GetModelAsync(id);

            ViewBag.ShowingAmazonProducts = true;
            var viewName = IsMobileDevice() ? "IndexMobile" : "Index";
            return View(viewName, model);
        }

        [Route("Product/Alto/{id:int}/{name?}", Name = "AltoProduct")]
        public async Task<IActionResult> Alto(int id, string name)
        {
            var product = await Context.Products.Where(x => x.AltoId == id).Select(x => new { x.Id, x.Name }).FirstOrDefaultAsync();
            if(product == null)
            {
                return RedirectToLocal("/");
            }
            else
            {
                return RedirectToAction("Index", new { product.Id, Name = product.Name.ToCanonical() });
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(CsrfHeadersValidationFilter))]
        public async Task<IActionResult> GetCoupon(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                Logger.LogWarning("User possibly tried to bypass authentication to buy");
                return Unauthorized();
            }

            var productCoupon = await Context.Products.Where(x => x.Id == id).Select(x => x.CouponCode).SingleOrDefaultAsync();
            if (productCoupon != null)
                return Ok(productCoupon);

            var coupon =await (from product in Context.Products
                join companyCoupon in Context.CompanyCoupons on product.CompanyId equals companyCoupon.CompanyId
                where product.Id == id &&
                      (companyCoupon.UsesLeft == null || companyCoupon.UsesLeft > 0)
                select companyCoupon).FirstOrDefaultAsync();

            if (coupon == null)
            {
                Logger.LogError("No coupon available for {productId}", id);
                return BadRequest();
            }
            if (coupon.UsesLeft != null)
            {
                coupon.UsesLeft--;
                await Context.SaveChangesAsync();
            }

            return Ok(coupon.Code);
        }
    }
}