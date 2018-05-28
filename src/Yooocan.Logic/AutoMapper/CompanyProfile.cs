using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Yooocan.Entities.Companies;
using Yooocan.Entities.Products;
using Yooocan.Enums;
using Yooocan.Models.Company;

namespace Yooocan.Logic.AutoMapper
{
    public class CompanyProfile : Profile
    {
        public CompanyProfile()
        {
            CreateMap<CompanyRegisterModel, Company>()
                .ForMember(x => x.Categories, o => o.MapFrom(s => s.CategoryIds.Select(i => new CompanyCategory { CategoryId = i }).ToList()));

            CreateMap<Company, CompanyRegisterModel>()
                .ForMember(x => x.CategoryIds, o => o.MapFrom(s => s.Categories.Select(x => x.CategoryId).ToList()));

            CreateMap<CompanyEditModel, Company>()
                .ForMember(x => x.Categories, o => o.MapFrom(s => s.CategoryIds.Select(i => new CompanyCategory { CategoryId = i }).ToList()));

            CreateMap<Company, CompanyEditModel>()
                .ForMember(x => x.CategoryIds, o => o.MapFrom(s => s.Categories.Select(x => x.CategoryId).ToList()))
                .ForMember(x => x.LogoUri, o => o.MapFrom(s => s.Images.Where(x => x.Type == AltoImageType.Logo && 
                                                                                   x.DeleteDate == null)
                                                                        .Select(x => x.Url)
                                                                        .SingleOrDefault()));

            CreateMap<ContactPersonModel, CompanyContactPerson>().ReverseMap();
            CreateMap<Company, CompanyIndexModel>()
                .AfterMap((s, d) =>
                {
                    var contactPerson = s.ContactPersons.FirstOrDefault();
                    if (contactPerson != null)
                    {
                        d.ContactPersonEmail = contactPerson.Email;
                        d.ContactPersonName = contactPerson.Name;
                        d.ContactPersonPosition = contactPerson.Position;
                    }
                });

            CreateMap<Company, CompanyEditTermsModel>().ReverseMap();
            CreateMap<CompanyShipping, CompanyShippingModel>().ReverseMap();
        }
    }
}
