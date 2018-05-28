using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;
using Alto.Dal;
using Alto.Domain;
using Alto.Domain.Orders;
using Alto.Domain.Products;
using Alto.Enums;
using Alto.Logic.Extensions;
using Alto.Logic.Messaging;
using Alto.Logic.PayPal;
using Alto.Models.Account.Claims;
using Alto.Models.PayPal;
using Alto.Models.Products;
using Alto.Web.Utils;
using AutoMapper;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PayPal.Api;

namespace Alto.Web.Controllers
{
    public class OrderController : BaseController
    {
        private readonly PayPalLogic _payPalLogic;
        private readonly IProductLogic _productLogic;
        private readonly IEmailLogic _emailLogic;
        private readonly MembershipManager _membershipManager;

        public OrderController(PayPalLogic payPalLogic, IProductLogic productLogic, AltoDbContext context, MapperConfiguration mapperConfiguration,
            UserManager<AltoUser> userManager, ILogger<BaseController> logger, 
            IEmailLogic emailLogic, MembershipManager membershipManager) : base(context, mapperConfiguration, userManager, logger)
        {
            _payPalLogic = payPalLogic;
            _productLogic = productLogic;
            _emailLogic = emailLogic;
            _membershipManager = membershipManager;
        }

        [HttpPost]
        public async Task<IActionResult> Create(int id)
        {
            if (!Request.IsAjaxRequest())
            {
                Logger.LogWarning("possible csrf attempt");
                return BadRequest();
            }

            if (_membershipManager.GetMembershipState() != MembershipState.Payed)
            {
                Logger.LogWarning("User {user} possibly tried to bypass authorization to buy", User.FindFirstValue(ClaimTypes.Email));
                return Unauthorized();
            }

            var product = await Context.Products
                .Include(x => x.Shipping)
                .Include(x => x.Company)
                .ThenInclude(x => x.ShippingRules)
                .Where(x => x.Id == id)
                .SingleAsync();

            decimal price = product.Price;
            var description = product.Description.StripHtml().Truncate(200);

            string chosenVariationsText = null;
            Dictionary<int, int> variations = JsonConvert.DeserializeObject<Dictionary<int, int>>(Request.Form["variations"]);
            if (variations.Count > 0)
            {
                var combinationsJson = await Context.ProductVariationCombinations
                    .Where(x => x.ProductId == id)
                    .Select(x => x.Combinations)
                    .SingleAsync();

                var combinations = JsonConvert.DeserializeObject<List<JsonVariationRow>>(combinationsJson);
                var foundVariationMatch = false;
                List<int> chosenVariation = null;
                foreach (var row in combinations)
                {
                    var isRowMatch = variations.All(choseVariation => row.Combinations[choseVariation.Key] == choseVariation.Value);
                    if (isRowMatch)
                    {
                        foundVariationMatch = true;
                        chosenVariation = row.Combinations.Select(x => x.Value).ToList();
                        price = row.Price ?? product.Price;
                        break;
                    }
                }

                if (!foundVariationMatch)
                {
                    Logger.LogError("Didn't find variation in the DB for {product} with {variations}", id, JsonConvert.SerializeObject(variations));
                    throw new ArgumentException("Variation not found");
                }

                var chosenVariations = await Context.ProductVariationValues
                    .Include(x => x.Variation)
                    .Where(x => chosenVariation.Contains(x.Id))
                    .Select(x => new KeyValuePair<string, string>(x.Variation.Name, x.Value))
                    .ToListAsync();

                chosenVariationsText = string.Join(Environment.NewLine, chosenVariations.Select(x => $"{x.Key}: {x.Value}"));
                description += Environment.NewLine + chosenVariationsText;
            }

            var shippingPrice = product.Shipping?.ShippingPrice ??
                                product.Company.ShippingRules.FirstOrDefault(r => r.MaxProductPrice >= price && r.MinProductPrice <= price)?.ShippingPrice ??
                                0;

            var paypalContext = _payPalLogic.GetContext();
            if (!product.IsSoldOnSite || !product.IsPublished || product.DeleteDate != null)
            {
                Logger.LogError("User tried buying {product} that isn't sold on site", id);
                return BadRequest();
            }

            // Give the discount
            var discount = product.Discount ?? product.Company.MembersDiscountRate;
            var discountType = product.DiscountType ?? product.Company.DiscountRateType ?? RateType.Percentage;
            if (discount != null)
            {
                price = discountType == RateType.Absolute
                    ? price - discount.Value
                    : price * (1 - discount.Value / 100);
            }

            var productPriceString = price.ToString("0.00");
            var shippingPriceString = shippingPrice.ToString("0.00");
            var totalPriceString = (price + shippingPrice).ToString("0.00");

            var payment = new Payment
            {
                intent = "sale",
                payer = new Payer {payment_method = "paypal"},
                transactions = new List<Transaction>
                                             {
                                                 new Transaction
                                                 {
                                                     description = description,
                                                     invoice_number = Guid.NewGuid().ToString(),
                                                     amount = new Amount
                                                              {
                                                                  currency = "USD",
                                                                  total = totalPriceString,
                                                                  details = new Details
                                                                            {
                                                                                subtotal = productPriceString,
                                                                                shipping = shippingPriceString
                                                                            }
                                                              },
                                                     item_list = new ItemList
                                                                 {
                                                                     items = new List<Item>
                                                                             {
                                                                                 new Item
                                                                                 {
                                                                                     name = product.Name,
                                                                                     currency = "USD",
                                                                                     description = description,
                                                                                     price = productPriceString,
                                                                                     quantity = "1",
                                                                                     sku = product.Id.ToString()
                                                                                 }
                                                                             }
                                                                 }
                                                 }
                                             },
                redirect_urls = new RedirectUrls
                {
                    cancel_url = Url.Action("Confirmation", null, null, Request.ToUri().Scheme),
                    return_url = Url.Action("Index", "Product", new {Id = id}, Request.ToUri().Scheme)
                },
                experience_profile_id = _payPalLogic.Options.PayPalExperienceProfileId
            };

            var createdPayment = payment.Create(paypalContext);
            var order = new Domain.Orders.Order
            {
                UserId = GetCurrentUserId().Value,
                Email = User.FindFirstValue(ClaimTypes.Email),
                TotalPrice = price + shippingPrice,
                TotalShippingPrice = shippingPrice,
                PaymentId = createdPayment.id,
                Variations = chosenVariationsText,
                Products = new List<OrderProduct>
                                       {
                                           new OrderProduct
                                           {
                                               ProductId = product.Id,
                                               ProductPrice = price,
                                               Quantity = 1,
                                               ShippingPrice = shippingPrice,
                                               Status = OrderStatus.AwaitingPayment
                                           }
                                       }
            };
            Context.Orders.Add(order);
            await Context.SaveChangesAsync();
            return Ok(new CreatePaymentResult
            {
                PaymentID = createdPayment.id
            });
        }

        [HttpPost]
        public async Task<IActionResult> Execute(string paymentID, string payerID)
        {
            if (!Request.IsAjaxRequest())
            {
                Logger.LogWarning("possible csrf attempt");
                return BadRequest();
            }

            var paymentExecution = new PaymentExecution {payer_id = payerID};
            var payment = new Payment {id = paymentID};
            var paypalContext = _payPalLogic.GetContext();
            var approvedPayment = Payment.Get(paypalContext, paymentID);

            var transaction = approvedPayment.transactions[0];
            var payerInfo = approvedPayment.payer.payer_info;
            var order = await Context.Orders.Include(x => x.Products).SingleOrDefaultAsync(x => x.PaymentId == paymentID);
            order.InvoiceId = transaction.invoice_number;
            order.AddressLine1 = payerInfo.shipping_address.line1;
            order.AddressLine2 = payerInfo.shipping_address.line2;
            order.City = payerInfo.shipping_address.city;
            order.Country = payerInfo.shipping_address.country_code;
            if (order.Country.ToUpper() != "US")
            {
                Logger.LogWarning("User put a shipping country other than the US, country code: {CountryCode}", order.Country);
                return BadRequest();
            }

            order.ZipCode = payerInfo.shipping_address.postal_code;
            order.State = payerInfo.shipping_address.state;
            order.FirstName = payerInfo.first_name;
            order.LastName = payerInfo.last_name;
            order.MiddleName = payerInfo.middle_name;
            order.Email = payerInfo.email;
            order.PhoneNumber = approvedPayment.payer.payer_info.phone;
            order.Products.ForEach(x => x.Status = OrderStatus.PaymentAuthorized);
            Context.SaveChanges();
            var productId = int.Parse(transaction.item_list.items[0].sku);
            var product = await Context.Products
                .Include(x => x.Images)
                .Include(x => x.Company)
                .AsNoTracking()
                .SingleAsync(x => x.Id == productId);

            var productPrice = decimal.Parse(transaction.amount.details.subtotal, NumberStyles.Any, CultureInfo.InvariantCulture);
            var shippingPrice = decimal.Parse(transaction.amount.details.shipping, NumberStyles.Any, CultureInfo.InvariantCulture);

            var model = new PurchaseConfirmModel
            {
                ImageUrl = product.Images.Single(x => x.DeleteDate == null && x.Type == ImageType.Main).CdnUrl,
                TotalPaid = productPrice + shippingPrice,
                PriceSaved = productPrice * (product.Discount ?? product.Company.MembersDiscountRate ?? 0) / 100,
                ProductName = product.Name,
                ProductId = productId,
                Quantity = 1,
                ShippingAddress = $"{order.AddressLine1} {order.AddressLine2}",
                Variations = order.Variations?.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>()
            };
            var confirmationView = PartialView("_ConfirmationWindow", model);

            var executedPayment = payment.Execute(paypalContext, paymentExecution);
            if (!string.IsNullOrEmpty(executedPayment.failure_reason))
            {
                Logger.LogError($"execute payment failed, reason: {executedPayment.failure_reason}, payment id: {paymentID}, didn't execute payment");
                return StatusCode(500);
            }
            if (executedPayment.state.ToLower() != "approved")
            {
                Logger.LogError($"unexpected paypal trasaction state after payment execution : {executedPayment.state} for payment id: {paymentID}, didn't execute payment");
                return StatusCode(500);
            }

            //at this point money was taken from buyer's account so have to show him confirmation page
            try
            {
                var variationText = order.Variations.IsNullOrWhiteSpace() ? "" : string.Join("<br>", order.Variations.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(row =>
                    {
                        var splitted = row.Split(':');
                        return $"{splitted[0]}: <strong>{splitted[1]}</strong>";
                    }));
                var emailData = new OrderConfirmationData
                {
                    Email = User.FindFirstValue(ClaimTypes.Email),
                    FirstName = User.FindFirstValue(ClaimTypes.GivenName),
                    ProductImage = product.Images.Single(x => x.Type == ImageType.Main && x.DeleteDate == null).CdnUrl,
                    ShippingPrice = shippingPrice == 0 ? "FREE" : shippingPrice.ToString("C"),
                    ProductName = product.Name,
                    UserId = GetCurrentUserId().Value,
                    ProductPrice = productPrice.ToString("C"),
                    VariationsText = variationText
                };
                await _emailLogic.SendOrderConfirmationEmailAsync(emailData);
                order.Products.ForEach(x => x.Status = OrderStatus.PaymentExecuted);
                var executedTransaction = executedPayment.transactions[0];
                order.SaleId = executedTransaction.related_resources[0].sale.id;
                Context.SaveChanges();
            }
            catch (Exception e)
            {
                Logger.LogError(12462, e,
                    $"Exception occurred after payment executed, view paypal transactions to supply order or to refund. PayPal Payment ID: {paymentID}");
            }
            return confirmationView;
        }
    }
}
