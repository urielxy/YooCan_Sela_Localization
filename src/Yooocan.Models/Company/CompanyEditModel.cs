using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Yooocan.Enums.Company;
using Yooocan.Models.Categories;

namespace Yooocan.Models.Company
{
    public class CompanyEditModel
    {
        public List<AltoCategoryModel> MainCategories { get; set; }

        [Required]
        [Display(Name = "Categories")]
        public List<int> CategoryIds { get; set; }

        public string OtherRequestedCategories { get; set; }

        public int Id { get; set; }       

        [Required]
        [Display(Name = "Company Name")]
        public string Name { get; set; }

        [Required]
        [Url(ErrorMessage = "Please enter a valid url that starts with http:// or https://")]
        [Display(Name = "Website Url")]
        public string WebsiteUrl { get; set; }
        
        public string LogoDataUri { get; set; }
        public string LogoUri { get; set; }

        public string About { get; set; }

        [Required]
        [Display(Name = "Business Type")]
        public BusinessType BusinessType { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [Display(Name = "Telephone Number")]
        public string TelephoneNumber { get; set; }

        [Display(Name = "Fax Number")]
        public string FaxNumber { get; set; }

        [Display(Name = "Toll-Free Number")]
        public string TollFreeNumber { get; set; }

        [Required]
        public List<ContactPersonModel> ContactPersons { get; set; }        
    }
}
