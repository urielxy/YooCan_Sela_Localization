using System.IO;

namespace Yooocan.Logic.Images
{
    public interface IImageLogic
    {
        MemoryStream Resize(Stream stream, int quality, int? width = null, int? height = null, TransformationMode? mode = TransformationMode.Cover);
    }
}