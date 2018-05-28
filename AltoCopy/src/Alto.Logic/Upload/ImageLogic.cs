using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Alto.Dal;
using Alto.Enums;
using ImageProcessorCore;
using Microsoft.EntityFrameworkCore;

namespace Alto.Logic.Upload
{
    public class ImageLogic : IImageLogic
    {
        private readonly AltoDbContext _context;

        public ImageLogic(AltoDbContext context)
        {
            _context = context;
        }
        public MemoryStream Resize(Stream stream, int quality, int? width = null, int? height = null)
        {
            var image = new Image(stream);
            var options = new ResizeOptions
                          {
                              Size = new Size(Math.Min(width ?? int.MaxValue, image.Width), Math.Min(height ?? int.MaxValue, image.Height)),
                              Mode = ResizeMode.Crop
                          };

            var output = new MemoryStream();
            image.Resize(options)
                .AutoOrient()
                .SaveAsJpeg(output, quality);

            return output;
        }
    }
}