using System;

namespace Yooocan.Entities
{
    public class StoryLimitation
    {
        public Story Story { get; set; }
        public int StoryId { get; set; }

        public Limitation Limitation { get; set; }
        public int LimitationId { get; set; }
        public bool IsPrimary { get; set; }

        public DateTime InsertDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}