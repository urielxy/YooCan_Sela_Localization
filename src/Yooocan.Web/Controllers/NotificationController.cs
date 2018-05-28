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
using Yooocan.Logic;
using Yooocan.Models.New;

namespace Yooocan.Web.Controllers
{
    [Authorize]
    public class NotificationController : BaseController
    {
        private readonly INotificationLogic _notificationLogic;
        public NotificationController(ApplicationDbContext context, ILogger<Controller> logger, IMapper mapper, UserManager<ApplicationUser> userManager, INotificationLogic notificationLogic) : base(context, logger, mapper, userManager)
        {
            _notificationLogic = notificationLogic;
        }

        public async Task<ActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var model = await _notificationLogic.GetUserNotificationsAsync(userId, true);

            return PartialView("_Popup", model);
        }

        public async Task<int> UnreadCount()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var unreadCount = await _notificationLogic.GetUnreadCountAsync(userId);

            return unreadCount;
        }
    }
}
