using System;
using System.Collections.Generic;

namespace Alto.Domain.Imports
{
    public class Vendor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string WebsiteUrl { get; set; }
        public string LogoUrl { get; set; }
        public List<Product> Products { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime? OnBoardingDate { get; set; }
        public string OnBoardingContactPersonEmail { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public int? CommercialTerms { get; set; }
        public decimal? CommercialTermsRate { get; set; }
        public string CommercialTermsOther { get; set; }
        public string CompanyCode { get; set; }
        public string About { get; set; }
        public int BusinessType { get; set; }
        public string BusinessTypeOther { get; set; }
        public bool IsDeleted { get; set; }

        #region Location
        public string LocationText { get; set; }
        public string StreetNumber { get; set; }
        public string StreetName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string GooglePlaceId { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        #endregion

        #region Social Media
        public string Twitter { get; set; }
        public string Instagram { get; set; }
        public string Facebook { get; set; }
        #endregion

        #region Company Contact Info
        public string TelephoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public string TollFreeNumber { get; set; }
        #endregion

        #region Contact Person Info
        public string ContactPersonName { get; set; }
        public string ContactPersonPosition { get; set; }
        public string ContactPersonEmail { get; set; }
        public string ContactPersonPhoneNumber { get; set; }
        public string ContactPersonSkype { get; set; }
        #endregion
    }
}