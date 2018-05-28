using Yooocan.Enums;
using System;

namespace Yooocan.Entities
{
    public class FeaturedStory
    {
        public int Id { get; set; }
        public int StoryId { get; set; }
        public Story Story { get; set; }
        public FeaturedType FeaturedType { get; set; }
        public DateTime InsertDate { get; set; }
    }
}