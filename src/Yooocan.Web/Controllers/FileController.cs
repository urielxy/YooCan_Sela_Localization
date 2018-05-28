using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Yooocan.Dal;
using Yooocan.Entities;
using Yooocan.Logic;

namespace Yooocan.Web.Controllers
{
    public class FileController : BaseController
    {
        private readonly IBlobUploader _blobUploader;

        public FileController(ApplicationDbContext context, ILogger<Controller> logger, IMapper mapper, UserManager<ApplicationUser> userManager, IBlobUploader blobUploader) : base(context, logger, mapper, userManager)
        {
            _blobUploader = blobUploader;
        }

        public async Task<ActionResult> Upload(IFormFile file, string containerName)
        {
            var url = await _blobUploader.UploadStreamAsync(file.OpenReadStream(), containerName, $"{Guid.NewGuid():N}_{file.FileName}");
            return Json(new {url});
        }
    }
}