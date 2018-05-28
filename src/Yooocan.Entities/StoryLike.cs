using System;

namespace Yooocan.Entities
{
    public class StoryLike
    {
        public int Id { get; set; }
        public Story Story { get; set; }
        public int StoryId { get; set; }
        public ApplicationUser User { get; set; }
        public string UserId { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeleteDate { get; set; }
        public DateTime InsertDate { get; set; }
    }
}