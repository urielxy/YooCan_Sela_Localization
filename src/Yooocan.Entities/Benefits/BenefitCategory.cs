using System;
using Yooocan.Entities.Companies;

namespace Yooocan.Entities.Benefits
{
    public class BenefitCategory
    {
        public int Id { get; set; }
        public AltoCategory Category { get; set; }
        public int CategoryId { get; set; }
        public Benefit Benefit { get; set; }
        public int BenefitId { get; set; }

        public DateTime InsertDate { get; set; }
    }
}
