using System;

namespace Yooocan.Entities
{
    public class Limitation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Limitation ParentLimitation { get; set; }
        public int? ParentLimitationId { get; set; }

        public DateTime InsertDate { get; set; }
    }
}