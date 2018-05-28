namespace Alto.Models.Account
{
    public class LimitationModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ParentLimitationName { get; set; }
        public int? ParentLimitationId { get; set; }
    }
}
