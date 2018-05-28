using System.IO;

namespace Alto.Models
{
    public class UploadFileModel
    {
        public Stream Stream { get; set; }
        public string FileName { get; set; }
        public string Url { get; set; }
        public string ContentType { get; set; }
    }
}