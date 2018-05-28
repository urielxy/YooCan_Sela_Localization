using System.ComponentModel.DataAnnotations;

namespace Yooocan.Web.Models
{
    public class SignupModel
    {
        [Display(Name = "Company Name")]
        [Required]
        public string CompanyName { get; set; }

        [Display(Name = "Company WebsiteUrl")]
        [Required]
        public string CompanyWebsiteUrl { get; set; }

        [Display(Name = "Company Address")]
        [Required]
        public string CompanyAddress { get; set; }

        [Display(Name = "Company Brands")]
        public string CompanyBrands { get; set; }

        [Display(Name = "Contact Person Name")]
        [Required]
        public string ContactName { get; set; }

        [Display(Name = "Contact Person Email")]
        [Required]
        [DataType(DataType.EmailAddress)]
        public string ContactEmail { get; set; }

        [Display(Name = "Contact Person Phone")]
        [DataType(DataType.PhoneNumber)]
        [Required]
        public string ContactPhone { get; set; }

        [Display(Name = "Contant Person Position")]
        [Required]
        public string ContactPosition { get; set; }

        [Display(Name = "Do you sell online")]
        public bool SellOnline { get; set; }

        [Display(Name = "Do you drop ship?")]
        public bool DropShip { get; set; }
    }
}