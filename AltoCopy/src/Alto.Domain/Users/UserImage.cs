using System;
using Alto.Enums;

namespace Alto.Domain.Users
{
    public class UserImage
    {
        public int Id { get; set; }
        public ImageType Type { get; set; }
        public string Url { get; set; }
        public string CdnUrl { get; set; }

        public int UserId { get; set; }
        public AltoUser User { get; set; }

        public DateTime InsertDate { get; set; }
        public DateTime? DeleteDate { get; set; }
    }
}