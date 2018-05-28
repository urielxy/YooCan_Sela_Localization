using System;

namespace Yooocan.Entities.ServiceProviders
{
    public class ServiceProviderVideo
    {
        public int Id { get; set; }
        public ServiceProvider ServiceProvider { get; set; }
        public int ServiceProviderId { get; set; }
        public int Order { get; set; }
        public bool IsPrimaryVideo { get; set; }
        public string YouTubeId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime InsertDate { get; set; }
    }
}