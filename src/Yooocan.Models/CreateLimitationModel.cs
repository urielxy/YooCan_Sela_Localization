namespace Yooocan.Models
{
    public class CreateLimitationModel
    {
        public string Name { get; set; }
        public string ParentLimitationName { get; set; }
        public int? ParentLimitationId { get; set; }
    }
}