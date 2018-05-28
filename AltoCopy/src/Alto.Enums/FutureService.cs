using System.ComponentModel.DataAnnotations;

namespace Alto.Enums
{
    public enum FutureService
    {
        [Display(Name = "LIFE INSURANCE")]
        LifeInsurance = 1,

        [Display(Name = "VEHICLE INSURANCE")]
        VehicleInsurance = 2,

        [Display(Name = "PROPERTY INSURANCE")]
        PropertyInsurance = 3,

        [Display(Name = "BUSINESS INSURANCE")]
        BusinessInsurance = 4,

        [Display(Name = "IDENTITY THEFT PROTECTION")]
        IdentityTheftProtection = 5,

        [Display(Name = "CREDIT CARD")]
        CreditCard = 6,

        [Display(Name = "LIFETIME INCOME ANNUITIES")]
        LifetimeIncomeAnnuities = 7,

        [Display(Name = "INVESTMENT SERVICES")]
        InvestmentServices = 8,

        [Display(Name = "SAVING FOR COLLEGE")]
        SavingForCollege = 9,
        [Display(Name = "PET INSURANCE")]
        PetInsurance = 10
    }
}