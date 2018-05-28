using System;
using System.Threading.Tasks;
using Alto.Dal;
using Alto.Domain;
using Alto.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Alto.Web.Controllers
{
    public class UserController : BaseController
    {
        public UserController(AltoDbContext context, MapperConfiguration mapperConfiguration, UserManager<AltoUser> userManager, ILogger<BaseController> logger) : base(context, mapperConfiguration, userManager, logger)
        {
        }

        [Authorize]
        public async Task<IActionResult> SaveLocation(UserLocationModel model)
        {
            var currentUserId = GetCurrentUserId();
            var userLocation = await Context.UserLocations.SingleOrDefaultAsync(x => x.UserId == currentUserId);

            if (userLocation != null)
            {
                userLocation.LastUpdateDate = DateTime.UtcNow;
                
            }

            return NoContent();
        }

        public IActionResult Edit()
        {
            throw new NotImplementedException();
        }
    }
}