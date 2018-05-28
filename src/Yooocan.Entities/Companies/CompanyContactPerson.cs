namespace Yooocan.Entities.Companies
{
    public class CompanyContactPerson
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public string Email { get; set; }
        public string PhoneExtension { get; set; }
        public string MobileNumber { get; set; }
        public string Skype { get; set; }
        public int CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
