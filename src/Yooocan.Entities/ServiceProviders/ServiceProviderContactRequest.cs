namespace Yooocan.Entities.ServiceProviders
{
    public class ServiceProviderContactRequest
    {
        public int Id { get; set; }
        public ServiceProvider ServiceProvider { get; set; }
        public int ServiceProviderId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Message { get; set; }
    }
}