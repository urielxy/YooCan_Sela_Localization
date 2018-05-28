using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Alto.Enums;

namespace Alto.Models.Account
{
    public class UserDetailsModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Birth Date")]
        public DateTime? BirthDate { get; set; }
        
        public string City { get; set; }
        
        public State? State { get; set; }

        [Display(Name = "Zip Code")]
        [RegularExpression(@"^(\d{5})$", ErrorMessage = "Please enter a 5 digit zip code.")]
        [Required]
        public string ZipCode { get; set; }

        [Display(Name = "How did you hear about Alto?")]
        public string Referrer { get; set; }

        public AccountRelationship AccountRelationship { get; set; }
        public List<LimitationModel> Limitations { get; set; }
        public List<int> LimitationIds { get; set; }
        public string LimitationName { get; set; }
        public string LimitationOther { get; set; }
    }
}
