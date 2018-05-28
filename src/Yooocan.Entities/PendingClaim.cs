using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yooocan.Entities
{
    public class PendingClaim
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
        public bool WasAssigned { get; set; }
        public ApplicationUser CreatedBy { get; set; }
        public string CreatedById { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime LastUpdateDate { get; set; }        
    }
}
