using System;
using Yooocan.Entities.ServiceProviders;

namespace Yooocan.Entities.Referrals
{
    public class ServiceProviderReferral
    {
        public int Id { get; set; }
        public int ServiceProviderId { get; set; }
        public ServiceProvider Product { get; set; }
        public string Url { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string Ip { get; set; }
        public string UserAgent { get; set; }
        public string Referrer { get; set; }
        public DateTime InsertDate { get; set; }
    }
}
