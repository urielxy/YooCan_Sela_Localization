using System.Collections;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Yooocan.Dal;
using Yooocan.Entities;
using Yooocan.Models;

namespace Yooocan.Web.Controllers
{
    public class LimitationController : BaseController
    {
        public LimitationController(ApplicationDbContext context, ILogger<LimitationController> logger, IMapper mapper, UserManager<ApplicationUser> userManager) : base(context, logger, mapper, userManager)
        {
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var limitations = Context.Limitations.Include(x=> x.ParentLimitation);
            var model = Mapper.Map<IEnumerable<LimitationListModel>>(limitations);

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            var limitations = Context.Limitations
                .Where(x => x.ParentLimitationId == null)
                .OrderBy(x=> x.Name)
                .Select(x => new SelectListItem
                                 {
                                     Text = x.Name,
                                     Value = x.Id.ToString()
                                 }).ToList();
            limitations.Insert(0, new SelectListItem {Text = "Select parent limitation"});
            ViewBag.ParentLimitations = limitations;

            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateLimitationModel model)
        {
            if (Context.Limitations.Any(x => x.Name == model.Name))
            {
                ModelState.AddModelError("Name", "Name already exists");
                return View(model);
            }

            Context.Limitations.Add(new Limitation {Name = model.Name, ParentLimitationId = model.ParentLimitationId});
            Context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}