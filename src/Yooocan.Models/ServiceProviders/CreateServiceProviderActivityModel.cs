using System.ComponentModel.DataAnnotations;

namespace Yooocan.Models.ServiceProviders
{
    public class CreateServiceProviderActivityModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public decimal? Price { get; set; }

        public string OpenDays { get; set; }

        public string Units { get; set; }
    }
}