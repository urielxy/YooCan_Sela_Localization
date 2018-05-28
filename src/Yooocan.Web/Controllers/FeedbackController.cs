using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Yooocan.Dal;
using Yooocan.Entities;
using Yooocan.Logic.Messaging;
using Yooocan.Models;
using Yooocan.Web.ActionFilters;

namespace Yooocan.Web.Controllers
{
    public class FeedbackController : BaseController
    {
        private readonly IEmailSender _emailSender;

        public FeedbackController(ApplicationDbContext context, ILogger<Controller> logger, IMapper mapper, UserManager<ApplicationUser> userManager,
            IEmailSender emailSender) : base(context, logger, mapper, userManager)
        {
            _emailSender = emailSender;
        }

        public IActionResult Feedback()
        {
            var model = new FeedbackModel
                        {
                            Name = $"{User.FindFirstValue(ClaimTypes.Surname)} {User.FindFirstValue(ClaimTypes.Surname)}",
                            Email = User.FindFirstValue(ClaimTypes.Email)
                        };

            return PartialView("_Feedback", model);
        }

        [HttpPost]
        [ServiceFilter(typeof(CsrfHeadersValidationFilter))]
        public async Task<IActionResult> Feedback(FeedbackModel model)
        {
            try
            {
                var spam = Request.Headers["User-Agent"].ToString().Trim() ==
                           "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/40.0.2214.111 Safari/537.36 OPR/27.0.1689.69";
                if (spam)
                    return NotFound();
            }
            catch (Exception)
            {
                // ignored
            }
            var message = $"Name:{model.Name}<br>Email:{model.Email}<br><br>{model.Feedback}";
            await _emailSender.SendEmailAsync(null, "moshe@yoocantech.com;yoav@yoocantech.com;dror@yoocantech.com;", "Feedback on yooocan", message, "Feedback", null);
            return NoContent();
        }
    }
}