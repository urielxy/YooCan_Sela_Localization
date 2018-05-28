using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Linq;
using Yooocan.Models;
using Yooocan.Logic.Images;
using Yooocan.Dal;

namespace Yooocan.Logic
{
    public class AzureUploader : BlobUploader
    {
        private readonly CloudStorageAccount _storageAccount;

        public AzureUploader(CloudStorageAccount storageAccount, IImageLogic imageLogic, IServiceProvider serviceProvider, ApplicationDbContext context)
            : base(imageLogic, serviceProvider, context)
        {
            _storageAccount = storageAccount;
        }

        public override async Task<string> UploadStreamAsync(Stream stream, string containerName, string fileName,
            int? width = null, int? height = null, string maxAge = "max-age=31536000", int quality = 90, TransformationMode mode = TransformationMode.Cover)
        {
            //Resize is cpu-bound and blocks, so this method is not really async
            using (var resized = Resize(stream, width, height, quality, mode))
            {
                // Create the blob client.
                CloudBlobClient blobClient = _storageAccount.CreateCloudBlobClient();

                // Retrieve reference to a previously created container.
                CloudBlobContainer container = blobClient.GetContainerReference(containerName);

                CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
                blockBlob.Properties.CacheControl = maxAge;
                blockBlob.Properties.ContentType = "image/jpeg";

                await blockBlob.UploadFromStreamAsync(resized);

                var fileUrl = blockBlob.Uri.AbsoluteUri;
                LogUploadFile(fileUrl);

                return fileUrl;
            }
        }

        public override async Task UploadFilesAsync(List<UploadFileModel> files, string containerName, int? width = null, int? height = null, int quality = 90)
        {
            await Task.WhenAll(files.Select(async (file, index) =>
            {
                using (file.Stream)
                {
                    if (file.Stream.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString("N");
                        {
                            file.Url = await UploadStreamAsync(file.Stream, containerName, fileName, width, height);
                        }
                    }
                }
            }));
        }
    }
}