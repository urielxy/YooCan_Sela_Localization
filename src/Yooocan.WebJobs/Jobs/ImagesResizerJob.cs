using Microsoft.Azure.WebJobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yooocan.Dal;
using Yooocan.Logic.Images;
using Yooocan.Logic.Options;

namespace Yooocan.WebJobs.Jobs
{
    public class ImagesResizerJob
    {
        public AzureStorageOptions StorageOptions { get; }
        public CloudStorageAccount StorageAccount { get; }
        public ApplicationDbContext Context { get; }
        private ILogger<ImagesResizerJob> Logger { get; }
        public AzureImageResizer AzureImageResizer { get; }

        public ImagesResizerJob(CloudStorageAccount storageAccount, IOptions<AzureStorageOptions> storageOptionsWrapper, ApplicationDbContext context, ILogger<ImagesResizerJob> logger, AzureImageResizer azureImageResizer)
        {
            StorageOptions = storageOptionsWrapper.Value;
            StorageAccount = storageAccount;
            Context = context;
            Logger = logger;
            AzureImageResizer = azureImageResizer;
        }

        public async Task Run([TimerTrigger("0 0 12 * * *", RunOnStartup = false)] TimerInfo timerInfo)
        {
            var storiesImagesPaths = Context.Stories 
                                                    .OrderBy(x => x.IsDeleted)
                                                    .ThenBy(x => x.IsNoIndex)
                                                    .ThenByDescending(x => x.Id)
                                                    .SelectMany(x => x.Images)
                                                    .Where(x => !x.IsDeleted)
                                                    .AsNoTracking()
                                                    .ToList()
                .Select(x => x.CdnUrl.Replace($"{StorageOptions.ImagesCdnPath}/", "")
                                     .Replace($"{StorageOptions.StoragePath}/", ""))
                .ToList();

            var exampleResizedLocation = AzureImageResizer.GetResizedImageLocation(storiesImagesPaths[0], 450, 350);
            var existingResizedImages = GetExistingResizedImages(exampleResizedLocation.Container,
                                                                 exampleResizedLocation.Path
                                                                                       .Substring(0, exampleResizedLocation.Path.LastIndexOf("/")));

            var generatedCount = 0;
            foreach (var path in storiesImagesPaths)
            {
                try
                {
                    var tasks = new List<Task<bool>>
                        {
                            GenerateOrGetResizedImage(existingResizedImages, path, 450, 350),
                            GenerateOrGetResizedImage(existingResizedImages, path, 450, 200),
                            GenerateOrGetResizedImage(existingResizedImages, path, 450, 700),
                            GenerateOrGetResizedImage(existingResizedImages, path, 300, 350),
                            GenerateOrGetResizedImage(existingResizedImages, path, 1200, 350),
                            GenerateOrGetResizedImage(existingResizedImages, path, height: 500)
                        };
                    var wasGenerated = await Task.WhenAll(tasks);
                    if (wasGenerated.Any(x => x))
                    {
                        generatedCount++;
                    }
                }
                catch (Exception e)
                {
                    Logger.LogWarning(e, $"error during generating sizes for {path}");
                }
            };

            Logger.LogInformation($"finished generating different sizes for {generatedCount} images, skipped {storiesImagesPaths.Count - generatedCount} images.");
        }

        private async Task<bool> GenerateOrGetResizedImage(HashSet<string> existing, string path, int? width = null, int? height = null)
        {
            var neededPath = AzureImageResizer.GetResizedImageLocation(path, width, height).Path;
            if (existing.Contains(neededPath))
                return false;
            else
            {
                await Task.Run(async () => (await AzureImageResizer.GenerateOrGetResizedImage(path, TransformationMode.Cover , width, height)).Dispose());
            }

            return true;
        }

        private HashSet<string> GetExistingResizedImages(string containerName, string path)
        {
            var blobClient = StorageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);
            var blobDirectory = container.GetDirectoryReference(path);
            var blobs = blobDirectory.ListBlobs().ToList();

            return new HashSet<string>(blobs.OfType<CloudBlockBlob>().Select(x => x.Name));            
        }
    }
}
