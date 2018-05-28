using Alto.Domain;
using Alto.Domain.Companies;
using Alto.Models.Account;
using AutoMapper;

namespace Alto.Web.AutoMapper
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<Limitation, LimitationModel>();
            CreateMap<RegistrationPromo, PromoModel>();
        }
    }
}