using System.ComponentModel.DataAnnotations;

namespace Alto.Enums
{
    public enum AccountRelationship
    {
        [Display(Name = "Self (account owner must be 18 or older)")]
        Self = 0,

        [Display(Name = "Parent of the account owner")]
        Parent = 1,

        [Display(Name = "Legal Guardian of the account owner")]
        LegalGuardian = 2
    }
}
