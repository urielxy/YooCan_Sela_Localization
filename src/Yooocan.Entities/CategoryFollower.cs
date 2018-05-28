using System;

namespace Yooocan.Entities
{
    public class CategoryFollower
    {
        public int Id { get; set; }
        public Category Category { get; set; }
        public int CategoryId { get; set; }
        public ApplicationUser User { get; set; }
        public string UserId { get; set; }

        public DateTime InsertDate { get; set; }
        public DateTime? DeleteDate { get; set; }
    }
}