using System;
using System.Collections.Generic;
using Alto.Domain.Companies;
using Alto.Domain.Users;
using Alto.Enums;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Alto.Domain
{
    public class AltoUser : IdentityUser<int>
    {
        public DateTime InsertDate { get; set; }
        public UserLocation Location { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string City { get; set; }
        public State? State { get; set; }
        public string ZipCode { get; set; }
        public AccountRelationship? AccountRelationship { get; set; }
        public List<UserLimitation> Limitations { get; set; }
        public string LimitationOther { get; set; }
        public int? ReferrerPromoId { get; set; }
        public string Referrer { get; set; }
        public RegistrationPromo ReferrerPromo { get; set; }
        public List<UserImage> Images { get; set; }

        public AltoUser()
        {
            Images = new List<UserImage>();
        }
    }
}
