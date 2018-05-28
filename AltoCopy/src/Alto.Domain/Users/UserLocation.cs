using System;

namespace Alto.Domain.Users
{
    public class UserLocation
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public AltoUser User { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public float? Longitude { get; set; }
        public float? Latitude { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }
}