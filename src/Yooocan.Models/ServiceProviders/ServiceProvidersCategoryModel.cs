using System.Collections.Generic;

namespace Yooocan.Models.ServiceProviders
{
    public class ServiceProvidersCategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string HeaderImageUrl { get; set; }
        public string MobileHeaderImageUrl { get; set; }
        public List<RelatedServiceProviderModel> ServiceProviders { get; set; }
    }
}