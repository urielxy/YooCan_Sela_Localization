using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Yooocan.Logic.Images;
using Yooocan.Models;

namespace Yooocan.Logic
{
    public interface IBlobUploader
    {
        Task<string> UploadStreamAsync(Stream stream, string containerName, string fileName, int? width = null, int? height = null, string maxAge = "max-age=31536000", 
            int quality = 90, TransformationMode mode = TransformationMode.Cover);

        Task UploadFilesAsync(List<UploadFileModel> images, string containerName, int? width = null, int? height = null, int quality = 90);

        Task<string> UploadDataUriImage(string dataUri, string containerName);
    }
}