using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alto.Dal;
using Alto.Domain;
using Alto.Logic.Upload;
using Alto.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Alto.Web.Controllers
{
    public class ImageController :BaseController
    {
        private readonly IBlobUploader _blobUploader;

        public ImageController(AltoDbContext context, MapperConfiguration mapperConfiguration, 
            UserManager<AltoUser> userManager, ILogger<BaseController> logger,
            IBlobUploader blobUploader) : base(context, mapperConfiguration, userManager, logger)
        {
            _blobUploader = blobUploader;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UploadImages(List<IFormFile> images, string containerName, int? width, int? height, int quality = 90)
        {
            var models = Mapper.Map<List<UploadFileModel>>(images);
            await _blobUploader.UploadFilesAsync(models, containerName, width, height);
            var urls = models.Select(x => x.Url).ToList();

            return Ok(urls);
        }
    }
}