using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yooocan.Entities.Referrals
{
    public class ReferralClientData
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string Ip { get; set; }
        public string UserAgent { get; set; }
        public string Referrer { get; set; }
        public string Url { get; set; }
    }
}
