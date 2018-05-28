using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Yooocan.Models.Company
{
    public class ContactPersonModel
    {
        public int Id { get; set; }

        [Required]        
        public string Name { get; set; }
        
        public string Position { get; set; }

        [Required]
        public string Email { get; set; }

        [Display(Name = "Phone Extension")]
        public string PhoneExtension { get; set; }

        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }
        
        public string Skype { get; set; }
    }
}
