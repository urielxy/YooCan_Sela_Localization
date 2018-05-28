namespace Yooocan.Models
{
    public class VendorListModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string WebsiteUrl { get; set; }
        public bool HasSignedUp { get; set; }
        public bool HasUploadedProducts { get; set; }
        public bool IsDeleted { get; set; }
    }
}