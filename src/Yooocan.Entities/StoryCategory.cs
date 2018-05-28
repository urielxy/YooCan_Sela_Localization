using System;

namespace Yooocan.Entities
{
    public class StoryCategory
    {
        public Category Category { get; set; }
        public int CategoryId { get; set; }

        public Story Story { get; set; }
        public int StoryId { get; set; }
        public bool IsPrimary { get; set; }

        public DateTime InsertDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}