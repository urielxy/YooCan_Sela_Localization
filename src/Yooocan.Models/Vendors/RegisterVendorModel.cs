using System.ComponentModel.DataAnnotations;
using Yooocan.Enums.Vendors;

namespace Yooocan.Models.Vendors
{
    public class RegisterVendorModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string WebsiteUrl { get; set; }
        public string PhoneNumber { get; set; }
        public string ContactPresonName { get; set; }
        public string ContactPresonRole { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        public VendorBusinessType BusinessType { get; set; }
        public string BusinessTypeOther { get; set; }
    }
}

