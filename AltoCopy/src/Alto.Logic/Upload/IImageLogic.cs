using System.IO;
using System.Threading.Tasks;

namespace Alto.Logic.Upload
{
    public interface IImageLogic
    {
        MemoryStream Resize(Stream stream, int quality, int? width = null, int? height = null);
    }
}