using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Yooocan.Models
{
    public class EditProductModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Specifications { get; set; }
        [Required]
        public string About { get; set; }
        [Required]
        public decimal Price { get; set; }
        public decimal? ListPrice { get; set; }
        [Required]
        public string Url { get; set; }
        public string PrimaryImageUrl { get; set; }
        public string VendorName { get; set; }
        public string VideoUrl { get; set; }
        [Required]
        public int VendorId { get; set; }
        public List<int> Categories { get; set; }
        public List<int> Limitations { get; set; }
        public List<string> Images { get; set; }
    }
}