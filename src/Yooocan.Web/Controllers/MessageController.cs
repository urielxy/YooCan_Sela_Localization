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
using Yooocan.Models;

namespace Yooocan.Web.Controllers
{
    [Authorize]
    public class MessageController : BaseController
    {
        private readonly IPrivateMessageLogic _messageLogic;

        public MessageController(ApplicationDbContext context, ILogger<Controller> logger, IMapper mapper, UserManager<ApplicationUser> userManager, IPrivateMessageLogic messageLogic) : base(context, logger, mapper, userManager)
        {
            _messageLogic = messageLogic;
        }

        [Route("Messages", Name = "Messages")]
        public async Task<ActionResult> Index(string userId)
        {
            var loggedInUser = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var messages = await _messageLogic.GetConversationsAsync(loggedInUser, userId);
            return View(messages);
        }

        public async Task<ActionResult> LoadConversation(string userId)
        {
            var loggedInUser = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var messages = await _messageLogic.GetConversationAsync(loggedInUser, userId);

            ViewBag.UserId = userId;
            ViewBag.Name = await Context.Users.Where(x => x.Id == userId).Select(x => x.FirstName + " " + x.LastName).SingleAsync();
            return PartialView("_Conversation", messages);
        }

        public async Task<JsonResult> SearchUsers(string q)
        {
            var results = await (from story in Context.Stories
                                 join user in Context.Users on story.UserId equals user.Id
                                 where story.IsPublished && (user.FirstName + " " + user.LastName).Contains(q)
                                 select new
                                        {
                                            value = user.Id,
                                            label = user.FirstName + " " + user.LastName,
                                            avatar = user.PictureUrl
                                        }).Take(50).Distinct().ToListAsync();
            return Json(results);
        }

        public async Task<ActionResult> Post(PrivateMessageModel message)
        {
            await _messageLogic.SendMessageAsync(User, message);
            return NoContent();
        }
    }
}