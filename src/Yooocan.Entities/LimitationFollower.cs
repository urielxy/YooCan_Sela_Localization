using System;

namespace Yooocan.Entities
{
    public class LimitationFollower
    {
        public int Id { get; set; }
        public Limitation Limitation { get; set; }
        public int LimitationId { get; set; }
        public ApplicationUser User { get; set; }
        public string UserId { get; set; }

        public DateTime InsertDate { get; set; }
        public DateTime? DeleteDate { get; set; }
    }
}