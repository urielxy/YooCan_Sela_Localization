using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Yooocan.Logic.Extensions;
using Yooocan.Logic.Options;

namespace Yooocan.Logic.Images
{
    public class AzureImageResizer
    {
        private readonly IBlobUploader _blobUploader;
        private readonly ILogger<AzureImageResizer> _logger;
        private readonly HttpClient _httpClient;
        private readonly AzureStorageOptions _storageOptions;

        public AzureImageResizer(IOptions<AzureStorageOptions> storageOptionsWrapper, IBlobUploader blobUploader, 
                                 ILogger<AzureImageResizer> logger, HttpClient httpClient)
        {
            _storageOptions = storageOptionsWrapper.Value;
            _blobUploader = blobUploader;
            _logger = logger;
            _httpClient = httpClient;
        }

        public static List<int?> AllowedWidths { get; } = new List<int?> { null, 50, 150, 300, 450, 600, 900, 1200 };
        public static List<int?> AllowedHeights { get; } = new List<int?> { null, 50, 200, 350, 500, 700 };

        public async Task<HttpResponseMessage> GenerateOrGetResizedImage(string path, TransformationMode mode, int? width = null, int? height = null)
        {
            if (string.IsNullOrEmpty(path) || (width == null && height == null) || !AllowedWidths.Contains(width) || !AllowedHeights.Contains(height))
            {
                return new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.BadRequest, Content = new StringContent("Invalid path, height or width") };
            }

            path = ConvertToRelativePath(path);
                
            (string Container, string Path) resizedImageLocation;
            try
            {
                resizedImageLocation = GetResizedImageLocation(path, width, height, mode);
            }
            catch (Exception e)
            {
                return new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.NotFound, Content = new StringContent(e.Message) };
            }

            var container = resizedImageLocation.Container;
            var resizedImagePath = resizedImageLocation.Path;
            var resizedUrl = $"{_storageOptions.StoragePath}/{container}/{resizedImagePath}";
            
            var fileResponse = await _httpClient.GetAsync(resizedUrl);
            if (!fileResponse.IsSuccessStatusCode)
            {
                var originalUrl = $"{_storageOptions.StoragePath}/{path}";
                var originalImage = await _httpClient.GetStreamAsync(originalUrl);
                await _blobUploader.UploadStreamAsync(originalImage, container, resizedImagePath, width, height, mode: mode);
                fileResponse = await _httpClient.GetAsync(resizedUrl);
                _logger.LogInformation($"finished generating resized image for {path}, width: {width}, height: {height}, result storage url: {resizedUrl}");
            }

            return fileResponse;
        }

        public (string Container, string Path) GetResizedImageLocation(string relativePath, int? width = null, int? height = null, 
            TransformationMode? mode = TransformationMode.Cover)
        {
            var originalFileNameWithoutExtension = Path.GetFileNameWithoutExtension(relativePath);
            var containerSeparatorIndex = relativePath.IndexOf("/");
            if (containerSeparatorIndex < 0)
            {
                throw new ArgumentException("Path is missing a container", relativePath);
            }
            var container = relativePath.Substring(0, containerSeparatorIndex);
            var blobLocation = relativePath.Substring(containerSeparatorIndex + 1);

            var resizedFileNameWithoutExtension = originalFileNameWithoutExtension;
            if (width != null)
            {
                resizedFileNameWithoutExtension += $"_w{width}";
            }
            if (height != null)
            {
                resizedFileNameWithoutExtension += $"_h{height}";
            }
            if(width != null && height != null && mode != TransformationMode.Cover)
            {
                resizedFileNameWithoutExtension += $"_m{mode}";
            }

            var resizedBlobLocation = blobLocation.ReplaceLast(originalFileNameWithoutExtension, $"resized/{resizedFileNameWithoutExtension}");
            return (container, resizedBlobLocation);
        }

        private string ConvertToRelativePath(string path)
        {
            if (!path.StartsWith("http"))
                return path;

            //fixing the start of the path because some hosts (HTTP.SYS) corrupt it even if it was properly url encoded
            path = path.Replace("https:/", "https://").Replace("https:///", "https://");

            if (!path.StartsWith(_storageOptions.ImagesCdnPath) && !path.StartsWith(_storageOptions.StoragePath))
                throw new ArgumentException("relative path or path in yoocan azure storage needed", "path"); ;

            return path.Replace(_storageOptions.ImagesCdnPath, "").Replace(_storageOptions.StoragePath, "").Substring(1);
        }
    }
}
