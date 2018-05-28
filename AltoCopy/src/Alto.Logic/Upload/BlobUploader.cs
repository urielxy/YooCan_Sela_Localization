using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Alto.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Alto.Logic.Upload
{
    public abstract class BlobUploader : IBlobUploader
    {
        private readonly IImageLogic _imageLogic;
        private readonly IServiceProvider _serviceProvider;

        protected BlobUploader(IImageLogic imageLogic, IServiceProvider serviceProvider)
        {
            _imageLogic = imageLogic;
            _serviceProvider = serviceProvider;
        }

        //protected void LogUploadFile(string url)
        //{
        //    using (var context = _serviceProvider.GetService<ApplicationDbContext>())
        //    {
        //        context.FileUploads.Add(new FileUpload
        //                                {
        //                                    Url = url
        //                                });
        //        context.SaveChanges();
        //    }
        //}

        protected Stream Resize(Stream input, int? width, int? height, int quality = 90)
        {
            return _imageLogic.Resize(input, quality, width, height);
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

            return url;
        }

        public abstract Task<string> UploadStreamAsync(Stream stream, string containerName, string fileName, int? width = null, int? height = null, string maxAge = "max-age=31536000", int quality = 90);
        public abstract Task UploadFilesAsync(List<UploadFileModel> images, string containerName, int? width = null, int? height = null, int quality = 90);
    }
}