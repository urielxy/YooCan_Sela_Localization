using System;

namespace Yooocan.Entities
{
    public class NewsletterSubscriber
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public bool Unsubscribed { get; set; }
        public bool IsVerified { get; set; }
        public DateTime InsertDate { get; set; }
        public string IpAddress { get; set; }
    }
}