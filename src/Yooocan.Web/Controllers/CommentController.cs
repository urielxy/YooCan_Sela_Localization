using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Yooocan.Dal;
using Yooocan.Entities;
using Yooocan.Logic;
using Yooocan.Models;
using Yooocan.Web.ActionFilters;

namespace Yooocan.Web.Controllers
{
    [Produces("application/json")]
    public class CommentController : BaseController
    {
        private readonly IDatabase _redisDatabase;
        private readonly IEmailLogic _emailLogic;

        public CommentController(ApplicationDbContext context, ILogger<CommentController> logger, IMapper mapper, UserManager<ApplicationUser> userManager, IDatabase redisDatabase, IEmailLogic emailLogic)
            : base(context, logger, mapper, userManager)
        {
            _redisDatabase = redisDatabase;
            _emailLogic = emailLogic;
        }

        [Authorize]
        [HttpPost]
        [ServiceFilter(typeof(CsrfHeadersValidationFilter))]
        public async Task<IActionResult> Create(int storyId, string text)
        {
            if (text.Trim().Length == 0)
                return BadRequest("Comment text is required");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var comment = new StoryComment
                              {
                                  StoryId = storyId,
                                  Text = text,
                                  UserId = userId
                              };

            Context.StoryComments.Add(comment);
            await Context.SaveChangesAsync();
            RemoveStoryCommentsFromCache(storyId);

            var story = Context.Stories.Include(x => x.User).Single(x => x.Id == storyId);
            if (userId != story.UserId)
            {
                await _emailLogic.SendYourStoryGotCommentEmailAsync(new EmailUserData
                                                                    {
                                                                        Email = story.User.Email,
                                                                        FirstName = story.User.FirstName,
                                                                        LastName = story.User.LastName,
                                                                        ProfileImageUrl = story.User.PictureUrl,
                                                                        UserId = userId
                                                                    }, Url.RouteUrl("Story", new {story.Id, story.Title}, "https"),
                    story.Title, comment.Id);
            }

            return CreatedAtAction("Index",new
                                               {
                                                   id = comment.Id
                                               });
        }

        [Authorize]
        [HttpDelete]
        [HttpPost]
        [ServiceFilter(typeof(CsrfHeadersValidationFilter))]
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await Context.StoryComments.SingleAsync(x => x.Id == id);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (comment.UserId != userId && !User.IsInRole("Admin"))
            {
                return Unauthorized();
            }
            comment.IsDeleted = true;
            await Context.SaveChangesAsync();
            RemoveStoryCommentsFromCache(comment.StoryId);

            return NoContent();
        }

        private void RemoveStoryCommentsFromCache(int storyId)
        {
            var key = string.Format(RedisKeys.StoryModel,storyId);
            _redisDatabase.HashDelete(key, RedisKeys.Comments, CommandFlags.FireAndForget);
        }

        public IActionResult Index(int id)
        {
            throw new NotImplementedException();
        }
    }
}