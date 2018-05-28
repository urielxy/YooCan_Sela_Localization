using System;

namespace Yooocan.Entities.ServiceProviders
{
    public class ServiceProviderImage
    {
        public int Id { get; set; }
        public ServiceProvider ServiceProvider { get; set; }
        public int ServiceProviderId { get; set; }
        public string Url { get; set; }
        public string CdnUrl { get; set; }
        public int Order { get; set; }
        public bool IsPrimaryImage { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime InsertDate { get; set; }
    }
}