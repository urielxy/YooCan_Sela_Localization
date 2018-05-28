namespace Alto.Domain.Referrals
{
    public class ReferralClientData
    {
        public string UserId { get; set; }
        public AltoUser User { get; set; }
        public string Ip { get; set; }
        public string UserAgent { get; set; }
        public string Referrer { get; set; }
        public string Url { get; set; }
    }
}
