using System;
using System.Collections.Generic;

namespace Yooocan.Entities.ServiceProviders
{
    public class ServiceProvider
    {
        public int Id { get; set; }
        public string LogoUrl { get; set; }
        public string HeaderImageUrl { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string StreetNumber { get; set; }
        public string StreetName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public string ContactPresonName { get; set; }
        public string WebsiteUrl { get; set; }
        public string PhoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public string Email { get; set; }
        public string AboutTheCompany { get; set; }
        public string AdditionalInformation { get; set; }
        public bool IsChapter { get; set; }
        public string TollFreePhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string Facebook { get; set; }
        public string Instagram { get; set; }

        public bool IsPublished { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime InsertDate { get; set; }
        public DateTime LastUpdateDate { get; set; }

        public List<ServiceProviderImage> Images { get; set; }
        public List<ServiceProviderVideo> Videos { get; set; }
        public List<ServiceProviderActivity> Activities { get; set; }
        public List<ServiceProviderLimitation> ServiceProviderLimitations { get; set; }
        public List<ServiceProviderContactRequest> ServiceProviderContactRequests { get; set; }
        public List<ServiceProviderCategory> ServiceProviderCategories { get; set; }

        public ServiceProvider()
        {
            Images = new List<ServiceProviderImage>();
            Videos = new List<ServiceProviderVideo>();
            Activities = new List<ServiceProviderActivity>();
            ServiceProviderLimitations = new List<ServiceProviderLimitation>();
            ServiceProviderCategories = new List<ServiceProviderCategory>();
            ServiceProviderContactRequests = new List<ServiceProviderContactRequest>();
        }
    }
}