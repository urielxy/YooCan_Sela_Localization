using System;
using Yooocan.Enums;

namespace Yooocan.Entities.Blog
{
    public class PostImage : IImage
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public int Order { get; set; }
        public ImageType Type { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime InsertDate { get; set; }
        public int PostId { get; set; }
    }
}
