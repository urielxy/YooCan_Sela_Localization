using System;
using Yooocan.Enums;

namespace Yooocan.Entities
{
    public class StoryImage : IImage
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string CdnUrl { get; set; }
        public int Order { get; set; }
        public Story Story { get; set; }
        public int StoryId { get; set; }
        
        public ImageType Type { get; set; }
        public float? HotAreaLeft { get; set; }
        public float? HotAreaTop { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime InsertDate { get; set; }
    }
}