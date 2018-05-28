using System.Collections.Generic;

namespace Yooocan.Models.Vendors
{
    public class MyProductsPageModel
    {
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public string VendorLogoUrl { get; set; }
        public string CompanyCode { get; set; }
        public List<MyProductsProductModel> Products { get; set; }
        public decimal? Commission { get; set; }
    }
}