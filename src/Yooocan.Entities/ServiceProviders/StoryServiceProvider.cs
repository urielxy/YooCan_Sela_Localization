using System;

namespace Yooocan.Entities.ServiceProviders
{
    public class StoryServiceProvider
    {
        public int Id { get; set; }
        public ServiceProvider ServiceProvider { get; set; }
        public int ServiceProviderId { get; set; }
        public Story Story { get; set; }
        public int StoryId { get; set; }
        public int Order { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime InsertDate { get; set; }
    }
}