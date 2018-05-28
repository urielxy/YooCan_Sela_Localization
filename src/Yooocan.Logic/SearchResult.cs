using System.Collections.Generic;
using Yooocan.Entities;
using Yooocan.Entities.Benefits;
using Yooocan.Entities.ServiceProviders;

namespace Yooocan.Logic
{
    public class SearchResult
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Story> Stories { get; set; }
        public IEnumerable<ServiceProvider> ServiceProviders { get; set; }
        public IEnumerable<Benefit> Benefits { get; set; }
    }
}