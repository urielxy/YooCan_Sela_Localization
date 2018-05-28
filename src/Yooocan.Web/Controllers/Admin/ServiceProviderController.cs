using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Yooocan.Dal;
using Yooocan.Entities;
using Yooocan.Entities.ServiceProviders;
using Yooocan.Logic;
using Yooocan.Models.ServiceProviders;
using Yooocan.Web.ActionFilters;

namespace Yooocan.Web.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class ServiceProviderController : BaseController
    {
        private readonly IServiceProviderLogic _serviceProvderLogic;

        public ServiceProviderController(ApplicationDbContext context, IMapper mapperConfiguration, UserManager<ApplicationUser> userManager, ILogger<BaseController> logger, 
            IServiceProviderLogic serviceProviderLogic) : base(context, logger, mapperConfiguration, userManager)
        {
            _serviceProvderLogic = serviceProviderLogic;
        }

        public async Task<IActionResult> All(AllModel model)
        {
            int perPage = 100;
            if(model.SinglePage)
            {
                perPage = 10000;
            }
            var query = Context.ServiceProviders.Where(x => (!model.OnlyWithEmails || !string.IsNullOrEmpty(x.Email)) && (!model.OnlyPending || !x.IsPublished) &&
                                                                  (!model.OnlyDeleted || x.IsDeleted) && (model.OnlyDeleted || !x.IsDeleted));
            model.PageCount = (int)Math.Ceiling((await query.CountAsync()) / (double)perPage);
            var pageResults = await query.AsNoTracking().OrderBy(x => x.Id).Skip(perPage * model.Page).Take(perPage).ToListAsync();

            model.ServiceProviders = Mapper.Map<List<ServiceProviderAllModel>>(pageResults);
            return View(model);
        }

        [HttpPost]
        [ServiceFilter(typeof(CsrfHeadersValidationFilter))]
        public async Task<IActionResult> TogglePublish(int id)
        {
            var serviceProvider = await Context.FindAsync<ServiceProvider>(id);
            serviceProvider.IsPublished = !serviceProvider.IsPublished;
            serviceProvider.LastUpdateDate = DateTime.Now;
            await Context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        [ServiceFilter(typeof(CsrfHeadersValidationFilter))]
        public async Task<IActionResult> ToggleDelete(int id)
        {
            var serviceProvider = await Context.FindAsync<ServiceProvider>(id);
            serviceProvider.IsDeleted = !serviceProvider.IsDeleted;
            if(serviceProvider.IsDeleted)
                serviceProvider.IsPublished = false;

            serviceProvider.LastUpdateDate = DateTime.Now;
            await Context.SaveChangesAsync();
            return Ok();
        }
    }
}