using System;
using Yooocan.Entities.Benefits;

namespace Yooocan.Entities.Referrals
{
    public class BenefitReferral
    {
        public int Id { get; set; }
        public int BenefitId { get; set; }
        public Benefit Benefit { get; set; }
        public string Url { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string Ip { get; set; }
        public string UserAgent { get; set; }
        public string Referrer { get; set; }
        public DateTime InsertDate { get; set; }
    }
}
