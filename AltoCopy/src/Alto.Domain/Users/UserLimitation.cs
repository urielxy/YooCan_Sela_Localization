using System;

namespace Alto.Domain.Users
{
    public class UserLimitation
    {
        public int Id { get; set; }
        public Limitation Limitation { get; set; }
        public int LimitationId { get; set; }
        public AltoUser User { get; set; }
        public int UserId { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime? DeleteDate { get; set; }
    }
}
