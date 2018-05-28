using System;

namespace Yooocan.Entities.ServiceProviders
{
    public class ServiceProviderLimitation
    {
        public Limitation Limitation { get; set; }
        public int LimitationId { get; set; }

        public ServiceProvider ServiceProvider { get; set; }
        public int ServiceProviderId { get; set; }

        public DateTime InsertDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}