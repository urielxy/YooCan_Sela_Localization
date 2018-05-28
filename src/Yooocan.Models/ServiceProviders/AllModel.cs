using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Yooocan.Models.ServiceProviders
{
    public class AllModel
    {
        public List<ServiceProviderAllModel> ServiceProviders { get; set; }
        public int PageCount { get; set; }
        public int Page { get; set; }

        [Display(Name = "Show only service providers with emails")]
        public bool OnlyWithEmails { get; set; }

        [Display(Name = "Show only pending (not published) service providers")]
        public bool OnlyPending { get; set; }

        [Display(Name = "Show only deleted service providers")]
        public bool OnlyDeleted { get; set; }

        [Display(Name = "Show all service providers in one page")]
        public bool SinglePage { get; set; }
    }
}
