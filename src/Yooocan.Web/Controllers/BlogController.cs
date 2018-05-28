using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Yooocan.Dal;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Yooocan.Entities;
using Yooocan.Models.Blog;
using Microsoft.AspNetCore.Authorization;
using Yooocan.Entities.Blog;
using Yooocan.Logic;
using System.Collections.Generic;
using Yooocan.Logic.Options;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Yooocan.Models.New.Stories;
using System;
using Yooocan.Logic.Extensions;
using System.Linq;
using System.Text.RegularExpressions;
using Yooocan.Logic.Images;
using Yooocan.Models;

namespace Yooocan.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BlogController : BaseController
    {
        public BlogController(ApplicationDbContext context, IMapper mapper,
            ILogger<BlogController> logger, UserManager<ApplicationUser> userManager,
            HtmlSanitizer htmlSanitizer, IOptions<AzureStorageOptions> azureStorageOptions, RedisWrapper redisWrapper,
            ImageUrlOptimizer imageUrlOptimizer) : base(context, logger, mapper, userManager)
        {
            HtmlSanitizer = htmlSanitizer;
            RedisWrapper = redisWrapper;
            ImageUrlOptimizer = imageUrlOptimizer;
            AzureStorageOptions = azureStorageOptions.Value;
        }

        public HtmlSanitizer HtmlSanitizer { get; }
        public RedisWrapper RedisWrapper { get; }
        public ImageUrlOptimizer ImageUrlOptimizer { get; }
        public AzureStorageOptions AzureStorageOptions { get; }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePostModel model)
        {
            var post = new Post
            {
                Content = HtmlSanitizer.SanitizeHtml(model.Content.Replace(AzureStorageOptions.StoragePath, AzureStorageOptions.ImagesCdnPath)),
                Title = model.Title.Trim(),
                Description = model.Description.Trim(),
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),

            };
            if (!string.IsNullOrEmpty(model.HeaderImageUrl))
            {
                post.Images = new List<PostImage>
                            {
                                new PostImage
                                {
                                    Type = Enums.ImageType.Header,
                                    Url = model.HeaderImageUrl.Replace(AzureStorageOptions.StoragePath, AzureStorageOptions.ImagesCdnPath),
                                }
                            };
            }
            await Context.Posts.AddAsync(post);
            await Context.SaveChangesAsync();
            return RedirectToAction("Index", new { post.Id });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var existing = await Context.Posts.Include(x => x.Images)
                                              .SingleAsync(x => x.Id == id);

            var model = new CreatePostModel
            {
                Content = HtmlSanitizer.SanitizeHtml(existing.Content),
                Title = existing.Title,
                Description = existing.Description,
                HeaderImageUrl = existing.Images.FirstOrDefault(x => x.Type == Enums.ImageType.Header)?.Url
            };

            return View("Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreatePostModel model)
        {
            var existing = await Context.Posts.Include(x => x.Images)
                                              .SingleAsync(x => x.Id == id);

            existing.Content = HtmlSanitizer.SanitizeHtml(model.Content.Replace(AzureStorageOptions.StoragePath, AzureStorageOptions.ImagesCdnPath));
            existing.Title = model.Title.Trim();
            existing.Description = model.Description.Trim();
            var existingHeaderImage = existing.Images.FirstOrDefault(x => x.Type == Enums.ImageType.Header);
            if (model.HeaderImageUrl != null && model.HeaderImageUrl != existingHeaderImage?.Url)
            {
                if(existingHeaderImage != null)
                    existingHeaderImage.IsDeleted = true;
                existing.Images.Add(new PostImage
                {
                    Type = Enums.ImageType.Header,
                    Url = model.HeaderImageUrl.Replace(AzureStorageOptions.StoragePath, AzureStorageOptions.ImagesCdnPath),
                });
            }

            await Context.SaveChangesAsync();
            await RedisWrapper.RedisDatabase.KeyDeleteAsync(GetKeyName(id));

            return RedirectToAction("Index", new { id });
        }

        public async Task<ActionResult> All()
        {
            var posts = await Context.Posts.IgnoreQueryFilters().AsNoTracking().ToListAsync();
            var models = Mapper.Map<List<Post>, List<PostModel>>(posts);

            return View(models);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TogglePublish(int id)
        {
            var post = await Context.FindAsync<Post>(id);
            post.PublishDate = post.PublishDate == null ? DateTime.UtcNow : (DateTime?)null;
            post.LastUpdateDate = DateTime.Now;
            await Context.SaveChangesAsync();
            await InvalidatePostCache(id);
            return RedirectToAction(nameof(All));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ToggleDelete(int id)
        {
            var post = await Context.Posts.IgnoreQueryFilters().Where(x => x.Id == id).SingleAsync();
            post.IsDeleted = !post.IsDeleted;
            post.LastUpdateDate = DateTime.Now;
            await Context.SaveChangesAsync();
            await InvalidatePostCache(id);
            return RedirectToAction(nameof(All));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ToggleFeatured(int id)
        {
            var post = await Context.FindAsync<Post>(id);
            var currentlyFeaturedPosts = await Context.Posts.Where(x => x.Order != 0).ToListAsync();
            post.Order = post.Order < 0 ? 0 : -1;
            currentlyFeaturedPosts.ForEach(p => p.Order = 0);

            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(All));
        }

        private async Task InvalidatePostCache(int id)
        {
            await RedisWrapper.RedisDatabase.KeyDeleteAsync(GetKeyName(id));
        }

        [HttpGet]
        [Route("Blog/{id:int}/{title?}", Name = "Blog")]
        [AllowAnonymous]
        public async Task<ActionResult> Index(int id, string title)
        {
            var model = await RedisWrapper.GetModelAsync(GetKeyName(id), async () =>
            {
                var post = await Context.Posts
                                        .Include(x => x.Images)
                                        .Include(x => x.User)
                                        .AsNoTracking()
                                        .SingleOrDefaultAsync(x => x.Id == id);
                var postModel = Mapper.Map<StoryModel>(post);
                postModel.Paragraphs[0].Text = OptimizeImages(postModel.Paragraphs[0].Text);

                var lastPosts = await Context.Posts
                                        .Include(x => x.Images)                                        
                                        .Where(x => x.Id != id && x.PublishDate != null)
                                        .OrderByDescending(x => x.InsertDate)
                                        .Take(2)
                                        .AsNoTracking()
                                        .ToListAsync();
                var queriedIds = lastPosts.Select(x => x.Id)
                                            .Union(new[] { id })
                                            .ToList();

                var randomPost = await Context.Posts.Include(x => x.Images)
                                                    .Where(x => !queriedIds.Contains(x.Id) && x.PublishDate != null)
                                                    .OrderBy(x => Guid.NewGuid())
                                                    .AsNoTracking()
                                                    .FirstOrDefaultAsync();
                var randomPostImage = await Context.PostImages.AsNoTracking()
                                                              .Where(x => x.PostId == randomPost.Id)
                                                              .FirstOrDefaultAsync();
                if (randomPost != null)
                {
                    randomPost.Images = new List<PostImage> { randomPostImage };
                    lastPosts.Add(randomPost);
                }

                var lastPostsCards = Mapper.Map<List<StoryCardModel>>(lastPosts);
                postModel.RelatedStories = lastPostsCards;

                return postModel;
            }, TimeSpan.FromDays(1));

            if (model == null)
                return NotFound();

            if (title != model.Title.ToCanonical())
                return RedirectToRoute(new { id, Title = model.Title.ToCanonical() });            

            return View("../Story/Index", model);
        }

        private string GetKeyName(int postId)
        {
            return string.Format(RedisKeys.BlogPost, postId);
        }

        private string OptimizeImages(string content)
        {
            var images = Regex.Matches(content, $@"<img src=""([^""]*)""").Cast<Match>().Select(x => x.Groups[1].Value).ToList();
            var optimizedImages = images.Select(x => ImageUrlOptimizer.GetOptimizedUrl(x, 900)).ToList();
            for (var i = 0; i < images.Count; i++)
            {
                content = content.Replace(images[i], optimizedImages[i]);
            }

            return content;
        }
    }
}
