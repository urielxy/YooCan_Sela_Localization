namespace Yooocan.Entities.Companies
{
    public class CompanyCategory
    {
        public int Id { get; set; }
        public AltoCategory Category { get; set; }
        public int CategoryId { get; set; }
        public Company Company { get; set; }
        public int CompanyId { get; set; }
    }
}
