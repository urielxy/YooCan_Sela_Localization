using System.ComponentModel.DataAnnotations;

namespace Yooocan.Enums.Company
{
    public enum BusinessType
    {
        [Display(Name = "Manufacturer")]
        ManufacturerVendor = 1,

        [Display(Name = "Dealer")]
        ResellerDealer = 2,

        [Display(Name = "Service Provider")]
        ServiceProvider = 3        
    }
}