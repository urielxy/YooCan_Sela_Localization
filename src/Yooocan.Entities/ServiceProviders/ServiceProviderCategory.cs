using System;

namespace Yooocan.Entities.ServiceProviders
{
    public class ServiceProviderCategory
    {
        public Category Category { get; set; }
        public int CategoryId { get; set; }

        public ServiceProvider ServiceProvider { get; set; }
        public int ServiceProviderId { get; set; }

        public DateTime InsertDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}