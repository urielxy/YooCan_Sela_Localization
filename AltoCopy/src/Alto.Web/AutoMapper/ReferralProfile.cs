using Alto.Domain;
using Alto.Domain.Referrals;
using Alto.Models.Account;
using AutoMapper;

namespace Alto.Web.AutoMapper
{
    public class ReferralProfile : Profile
    {
        public ReferralProfile()
        {
            CreateMap<ReferralClientData, ProductReferral>();
            CreateMap<ReferralClientData, BenefitReferral>();
        }
    }
}