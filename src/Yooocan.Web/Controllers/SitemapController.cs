using AutoMapper;
using f14.NetCore.Sitemap;
using f14.NetCore.Sitemap.Entries;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Yooocan.Dal;
using Yooocan.Entities;
using Yooocan.Logic.Extensions;

namespace Yooocan.Web.Controllers
{
    public class SitemapController : BaseController
    {
        public SitemapController(ApplicationDbContext context, ILogger<Controller> logger, IMapper mapper, UserManager<ApplicationUser> userManager)
            : base(context, logger, mapper, userManager)
        {
        }

        public async Task<IActionResult> Index()
        {
            var lastModifiedStory = await Context.Stories.Where(x => !x.IsDeleted && x.IsPublished && !x.IsNoIndex)
                                                         .Select(x => x.LastUpdateDate)
                                                         .MaxAsync();
            var sitemaps = new List<IndexEntry>
            {
                new IndexEntry
                {
                    Url = Url.Action(nameof(Stories), null, null, "https"),
                    Modified = lastModifiedStory
                }
            };

            var xml = new XmlSitemapBuilder(new XElement(Constants.SitemapIndexName), sitemaps).Build();
            return Content(xml.ToString(), "application/xml");
        }

        public async Task<IActionResult> Stories()
        {
            var stories = await Context.Stories.Where(x => !x.IsDeleted && x.IsPublished && !x.IsNoIndex)
                                               .Select(x => new UrlEntry
                                               {
                                                   Url = Url.RouteUrl("Story", new { x.Id, Title = x.Title.ToCanonical() }, "https"),
                                                   Modified = x.LastUpdateDate
                                               })
                                               .ToListAsync();

            var xml = new XmlSitemapBuilder(new XElement(Constants.UrlsetName, Constants.GoogleImageAttribute), stories).Build();
            return Content(xml.ToString(), "application/xml");
        }
    }
}