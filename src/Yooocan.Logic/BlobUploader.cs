using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Yooocan.Dal;
using Yooocan.Entities;
using Yooocan.Models;
using Yooocan.Logic.Images;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace Yooocan.Logic
{
    public abstract class BlobUploader : IBlobUploader
    {
        private readonly IImageLogic _imageLogic;
        private readonly IServiceProvider _serviceProvider;
        private readonly ApplicationDbContext _context;

        protected BlobUploader(IImageLogic imageLogic, IServiceProvider serviceProvider, ApplicationDbContext context)
        {
            _imageLogic = imageLogic;
            _serviceProvider = serviceProvider;
            _context = context;
        }

        protected void LogUploadFile(string url)
        {
            using (var context = _serviceProvider.GetService<ApplicationDbContext>())
            {
                context.FileUploads.Add(new FileUpload
                                        {
                                            Url = url
                                        });
                context.SaveChanges();
            }
        }

        protected Stream Resize(Stream input, int? width, int? height, int quality = 90, TransformationMode? mode = TransformationMode.Cover)
        {
            return _imageLogic.Resize(input, quality, width, height, mode);
        }

        public async Task<string> UploadDataUriImage(string dataUri, string containerName)
        {
            var matches = Regex.Match(dataUri, @"data:(?<type>.+?);base64,\s*?(?<data>.+)");
            var base64 = matches.Groups["data"].Value;
            var bytes = Convert.FromBase64String(base64);
            string url;

            using (var memoryStream = new MemoryStream(bytes))
            {
                url = await UploadStreamAsync(memoryStream, containerName, Guid.NewGuid().ToString("N"));
            }

            var fileUpload = await _context.FileUploads.SingleOrDefaultAsync(x => x.Url == url);
            if(fileUpload != null)
                fileUpload.IsUsed = true;

            return url;
        }

        public abstract Task<string> UploadStreamAsync(Stream stream, string containerName, string fileName, int? width = null, int? height = null, 
            string maxAge = "max-age=31536000", int quality = 90, TransformationMode mode = TransformationMode.Cover);
        public abstract Task UploadFilesAsync(List<UploadFileModel> images, string containerName, int? width = null, int? height = null, int quality = 90);
    }
}