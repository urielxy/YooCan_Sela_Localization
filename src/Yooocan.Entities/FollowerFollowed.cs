using System;

namespace Yooocan.Entities
{
    public class FollowerFollowed
    {
        public int Id { get; set; }
        public DateTime InsertDate { get; set; }
        public bool IsDeleted { get; set; }
        public ApplicationUser FollowerUser { get; set; }
        public string FollowerUserId { get; set; }

        public ApplicationUser FollowedUser { get; set; }
        public string FollowedUserId { get; set; }
    }
}