using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace Yooocan.Logic.Images
{
    public class ImageLogic : IImageLogic
    {
        public MemoryStream Resize(Stream stream, int quality, int? width = null, int? height = null, TransformationMode? mode = TransformationMode.Cover)
        {
            using (var image = Image.Load(stream))
            {
                var resizeMode = width == null || height == null || mode == TransformationMode.Contain ? ResizeMode.Max : ResizeMode.Min;
                var options = new ResizeOptions
                {
                    Size = new Size(Math.Min(width ?? int.MaxValue, image.Width), Math.Min(height ?? int.MaxValue, image.Height)),
                    Mode = resizeMode
                };

                var output = new MemoryStream();
                image.Mutate(o => o.Resize(options));
                image.Save(output, new JpegEncoder { Quality = quality });
                output.Seek(0, SeekOrigin.Begin);

                return output;
            }
        }
    }
}