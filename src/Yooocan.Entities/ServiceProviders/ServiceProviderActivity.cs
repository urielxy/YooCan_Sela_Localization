namespace Yooocan.Entities.ServiceProviders
{
    public class ServiceProviderActivity
    {
        public int Id { get; set; }
        public ServiceProvider ServiceProvider { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string OpenDays { get; set; }
        public string Units { get; set; }
        public int Order { get; set; }
        public bool IsDeleted { get; set; }
    }
}