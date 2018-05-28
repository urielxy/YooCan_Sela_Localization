using System.Security;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Yooocan.Dal;
using Yooocan.Entities;
using Yooocan.Logic;
using Yooocan.Logic.Options;
using Yooocan.Web.ActionFilters;

namespace Yooocan.Web.Controllers
{
    public class SchedulerController : BaseController
    {

        private readonly IPrivateMessageLogic _messageLogic;
        private readonly string _apiKey;

        public SchedulerController(ApplicationDbContext context, ILogger<SchedulerController> logger, IMapper mapper, UserManager<ApplicationUser> userManager,
            IPrivateMessageLogic messageLogic, IOptions<SchedulerKeys> apiKey) : base(context, logger, mapper, userManager)
        {
            _messageLogic = messageLogic;
            _apiKey = apiKey.Value.SchedulerKey;
        }

        [HttpPost]
        [ServiceFilter(typeof(CsrfHeadersValidationFilter))]
        [NonAction]
        public async Task SendUnreadMessageAlerts(string key)
        {
            if (key != _apiKey)
            {
                Logger.LogError("Invalid key used for scheduler {action}, {key}", nameof(SendUnreadMessageAlerts), key);
                throw new SecurityException();
            }

            await _messageLogic.SendNotificationsOnIncomingMessageAsync();
        }
    }
}