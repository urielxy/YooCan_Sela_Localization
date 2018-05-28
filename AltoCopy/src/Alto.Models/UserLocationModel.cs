namespace Alto.Models
{
    public class UserLocationModel
    {
        public int UserId { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public float? Longitude { get; set; }
        public float? Latitude { get; set; }
    }
}