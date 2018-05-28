using System;
using System.Collections.Generic;

namespace Yooocan.Models.SearchIndexes
{
    public class ServiceProviderIndexModel
    {
        public string ServiceProviderId { get; set; }
        public string Name { get; set; }
        public string AboutTheCompany { get; set; }
        public List<string> ActivitiesNames { get; set; }
        public List<string> ActivitiesDescriptions { get; set; }
        public List<string> CategoryIds { get; set; }
        public List<string> CategoryNames { get; set; }
        public List<string> LimitationIds { get; set; }
        public List<string> LimitationNames { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }
}