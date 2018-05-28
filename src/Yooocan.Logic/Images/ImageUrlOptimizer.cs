using Microsoft.Extensions.Options;
using Yooocan.Logic.Options;

namespace Yooocan.Logic.Images
{
    public class ImageUrlOptimizer
    {
        private AzureStorageOptions _storageOptions;

        public ImageUrlOptimizer(IOptions<AzureStorageOptions> storageOptionsWrapper)
        {
            _storageOptions = storageOptionsWrapper.Value;
        }

        public string GetOptimizedUrl(string originalUrl, int? width = null, int? height = null, TransformationMode mode = TransformationMode.Cover)
        {
            //TODO: remove after all images transferred from alto storage to yoocan storage
            if (originalUrl?.Contains("altolife.azureedge.net") == true || originalUrl?.Contains("altoprod.blob.core.windows.net") == true)
            {
                originalUrl = originalUrl.Replace("altoprod.blob.core.windows.net", "altolife.azureedge.net");
                if(width > 100)
                {
                    return originalUrl + "_300x200";
                }
                else
                {
                    return originalUrl + "_48x48";
                }
            }
            if (originalUrl == null || (width == null && height == null) ||
                (!originalUrl.Contains(_storageOptions.ImagesCdnPath) && !originalUrl.Contains(_storageOptions.StoragePath)))
            {
                return originalUrl;
            }

            var optimizedUrl = originalUrl
                    .Replace(_storageOptions.StoragePath, _storageOptions.OptimizedImagesCdnPath)
                    .Replace(_storageOptions.ImagesCdnPath, _storageOptions.OptimizedImagesCdnPath) + "?";

            if (width != null)
            {
                optimizedUrl += $"w={width}";
            }

            if (height != null)
            {
                optimizedUrl += $"&h={height}";
            }

            if(width != null && height != null && mode != TransformationMode.Cover)
            {
                optimizedUrl += $"&m={mode.ToString().ToLowerInvariant()}";
            }

            optimizedUrl = optimizedUrl.Replace("?&", "?");

            return optimizedUrl;
        }
    }
}