using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Yooocan.Dal;
using Yooocan.Entities;
using Yooocan.Logic;
using Yooocan.Models;
using System.Linq;
using System.Text.RegularExpressions;
using System.Net.Http;
using Yooocan.Logic.Options;
using Microsoft.Extensions.Options;
using Yooocan.Logic.Images;
using Yooocan.Web.ActionFilters;

namespace Yooocan.Web.Controllers
{
    public class ImageController : BaseController
    {
        private readonly IBlobUploader _blobUploader;
        private readonly AzureImageResizer _azureImageResizer;
        private AzureStorageOptions _storageOptions;

        public ImageController(ApplicationDbContext context, ILogger<ImageController> logger, IMapper mapper, UserManager<ApplicationUser> userManager, IBlobUploader blobUploader, IOptions<AzureStorageOptions> storageOptionsWrapper, AzureImageResizer azureImageResizer) : base(context, logger, mapper, userManager)
        {
            _blobUploader = blobUploader;
            _azureImageResizer = azureImageResizer;
            _storageOptions = storageOptionsWrapper.Value;
        }

        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(CsrfHeadersValidationFilter))]
        public async Task<ActionResult> UploadImages(List<IFormFile> images, string containerName = "images", int? width = null, int? height = null, int quality = 90)
        {
            var urls = await UploadImagesInternal(images, containerName, width, height, quality);
            return Ok(urls);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(CsrfHeadersValidationFilter))]
        public async Task<ActionResult> UploadImageMce(IFormFile file)
        {
            var url = (await UploadImagesInternal(new List<IFormFile> { file }))[0];
            return Ok($@"{{""location"": ""{url}""}}");
        }

        private async Task<List<string>> UploadImagesInternal(List<IFormFile> images, string containerName = "images", int? width = null, int? height = null, int quality = 90)
        {
            var models = Mapper.Map<List<UploadFileModel>>(images);
            try
            {
                await _blobUploader.UploadFilesAsync(models, containerName, width, height);
            }
            catch (Exception exception)
            {
                var contents = GetFileContents(images);
                Logger.LogError(new EventId(176253), exception, "Failed to upload images: " +
                                                    string.Join("\r\n", contents.Select(x => $"Content-Type: {x.Type}, Base64 Contents: {x.Base64Content}\r\n")));
                throw;
            }
            var urls = models.Select(x => x.Url).ToList();
            return urls;
        }       

        [Route("/Image/Get/{*path}")]
        public async Task<HttpResponseMessage> Get(string path, [FromQuery(Name = "w")]int? width, [FromQuery(Name = "h")]int? height,
            [FromQuery(Name = "m")]TransformationMode mode = TransformationMode.Cover)
        {
            return await _azureImageResizer.GenerateOrGetResizedImage(path, mode, width, height);
        }

        private IEnumerable<(string Type, string Base64Content)> GetFileContents(IEnumerable<IFormFile> files)
        {
            foreach (var file in files)
            {
                using (var stream = file.OpenReadStream())
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    var base64 = Convert.ToBase64String(ms.ToArray());
                    yield return (file.ContentType, base64);
                }
            }
        }

        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(CsrfHeadersValidationFilter))]
        public async Task<ActionResult> UploadDataUriImages(string dataUri, string containerName, int quality = 95)
        {
            var matches = Regex.Match(dataUri, @"data:(?<type>.+?);base64,\s*?(?<data>.+)");
            var base64 = matches.Groups["data"].Value;
            var bytes = Convert.FromBase64String(base64);

            using (var memoryStream = new MemoryStream(bytes))
            {
                var url = await _blobUploader.UploadStreamAsync(memoryStream, containerName, Guid.NewGuid().ToString("N"), quality: quality);
                return Ok(url);
            }
        }
    }
}