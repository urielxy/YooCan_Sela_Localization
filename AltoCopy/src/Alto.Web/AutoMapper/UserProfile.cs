using System;
using System.Linq;
using Alto.Domain;
using Alto.Domain.Users;
using Alto.Models.Account;
using Alto.Models.Categories;
using AutoMapper;

namespace Alto.Web.AutoMapper
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserDetailsModel, AltoUser>().ForMember(d => d.Limitations, o => o.MapFrom(s => s.LimitationIds == null ? null : s.LimitationIds.Select(limitationId => new UserLimitation
            {
                LimitationId = limitationId
            })));

            CreateMap<InsuranceRegisterationModel, UserFutureService>();
        }
    }
}