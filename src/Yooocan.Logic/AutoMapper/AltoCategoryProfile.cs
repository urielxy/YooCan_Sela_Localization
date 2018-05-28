using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Yooocan.Entities.Companies;
using Yooocan.Enums;
using Yooocan.Models.Categories;

namespace Yooocan.Logic.AutoMapper
{
    public class AltoCategoryProfile : Profile
    {
        public AltoCategoryProfile()
        {
            CreateMap<AltoCategory, AltoCategoryModel>();
            CreateMap<AltoCategory, AltoCategoryMenuModel>()
                .ForMember(x => x.SubCategories, o => o.MapFrom(x => x.SubCategories
                    .Where(sub => !sub.IsDeleted && sub.IsActive)
                    .Select(sub => new KeyValuePair<int, string>(sub.Id, sub.Name))
                    .ToList()))
                .ForMember(x => x.FooterImageUrl, o => o.MapFrom(x => x.Images
                    .Where(i => i.Type == AltoImageType.Footer)
                    .Select(i => i.CdnUrl)
                    .SingleOrDefault()));
        }
    }
}