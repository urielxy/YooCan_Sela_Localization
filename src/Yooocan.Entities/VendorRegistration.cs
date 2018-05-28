using Yooocan.Enums.Vendors;
using Yooocan.Models.Vendors;

namespace Yooocan.Entities
{
    public class VendorRegistration
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string WebsiteUrl { get; set; }
        public string PhoneNumber { get; set; }
        public string ContactPresonName { get; set; }
        public string ContactPresonRole { get; set; }
        public string Email { get; set; }
        public VendorBusinessType BusinessType { get; set; }
        public string BusinessTypeOther { get; set; }
        public bool WasHandled { get; set; }
    }
}