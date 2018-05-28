using System.ComponentModel.DataAnnotations;

namespace Yooocan.Models.Admin
{
    public class AddAmazonProductModel
    {
        [Required]
        public string Asin { get; set; }
        public int ProductId { get; set; }
        public int AmazonVendorId { get; set; }
    }
}
