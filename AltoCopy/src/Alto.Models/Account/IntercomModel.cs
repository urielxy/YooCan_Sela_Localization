using System;

namespace Alto.Models.Account
{
    public class IntercomModel
    {
        public string AppId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }
        public DateTimeOffset? MembershipExpiryDate { get; set; }
    }
}
