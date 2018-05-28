using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Yooocan.Models
{
    public class UserBioModel
    {
        public string Id { get; set; }
        [Display(Name = "First name")]
        public string FirstName { get; set; }
        [Display(Name = "Last name")]
        public string LastName { get; set; }
        public string PictureDataUri { get; set; }
        public string HeaderImageDataUri { get; set; }
        public string HeaderImageUrl { get; set; }
        public string Location { get; set; }
        [Display(Name = "About me")]
        public string AboutMe { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public List<StoryCardModel> Stories { get; set; }

        public UserBioModel ShallowCopy()
        {
            return (UserBioModel)MemberwiseClone();
        }
    }
}