using System;
using System.Collections.Generic;

namespace Yooocan.Models.SearchIndexes
{
    public class ProductIndexModel
    {
        public string ProductId { get; set; }
        public string Name { get; set; }
        public string Specifications { get; set; }
        public string About { get; set; }
        public string VendorName { get; set; }
        public List<string> CategoryIds { get; set; }
        public List<string> CategoryNames { get; set; }
        public List<string> LimitationIds { get; set; }
        public List<string> LimitationNames { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }
}