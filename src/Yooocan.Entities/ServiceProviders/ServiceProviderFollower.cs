using System;

namespace Yooocan.Entities.ServiceProviders
{
    public class ServiceProviderFollower
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int ServiceProviderId { get; set; }
        public ServiceProvider ServiceProvider { get; set; }

        public DateTime InsertDate { get; set; }
        public DateTime? DeleteDate { get; set; }
    }
}