using System.ComponentModel.DataAnnotations;

namespace Yooocan.Models.ServiceProviders
{
    public class ContactServiceProviderModel
    {
        [Required]
        public int ServiceProviderId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        public string Address { get; set; }
        public string Phone { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public string RecaptchToken { get; set; }

        public string ClientIp { get; set; }
    }
}