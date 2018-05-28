using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Yooocan.Entities.Benefits;
using Yooocan.Enums;
using Yooocan.Models;
using Yooocan.Models.Benefits;
using Yooocan.Models.Cards;

namespace Yooocan.Logic.AutoMapper
{
    public class BenefitProfile : Profile
    {
        public BenefitProfile()
        {
            CreateMap<Benefit, BenefitModel>()
                .ForMember(x => x.MainImageUrl, o => o.MapFrom(s => s.Images
                    .Where(i => i.Type == AltoImageType.Main && i.DeleteDate == null)
                    .Select(i => i.CdnUrl)
                    .Single()))
                .ForMember(x => x.Images, o => o.MapFrom(s => s.Images
                    .Where(i => i.Type == AltoImageType.Normal && i.DeleteDate == null)
                    .OrderBy(i => i.Order)
                    .Select(i => i.CdnUrl)
                    .ToList()))
                .ForMember(x => x.LogoUrl, o => o.MapFrom(s => s.Company.Images
                    .Where(i => i.Type == AltoImageType.Logo && i.DeleteDate == null)
                    .Select(i => i.CdnUrl)
                    .SingleOrDefault()))
                .ForMember(x => x.CompanyName, o => o.MapFrom(s => s.Company.Name))
                .ForMember(x => x.Category,
                    o => o.ResolveUsing(s =>
                    {
                        var mainCategory = s.Categories.FirstOrDefault()?.Category;
                        return mainCategory == null
                            ? (KeyValuePair<int, string>?)null
                            : new KeyValuePair<int, string>(mainCategory.Id, mainCategory.Name);
                    }))
                .ForMember(x => x.ParentCategory,
                    o => o.ResolveUsing(s =>
                    {
                        var mainCategory = s.Categories.FirstOrDefault()?.Category?.ParentCategory;
                        return mainCategory == null
                            ? (KeyValuePair<int, string>?)null
                            : new KeyValuePair<int, string>(mainCategory.Id, mainCategory.Name);
                    }));

            CreateMap<Benefit, BenefitCardModel>()
                .ForMember(x => x.ImageUrl, o => o.ResolveUsing(s =>
                {
                    var url = s.Images
                        .Where(i => i.Type == AltoImageType.Main && i.DeleteDate == null)
                        .Select(i => i.CdnUrl).SingleOrDefault();
                    if (url == null)
                        return null;
                    url = url.Insert(url.Length - 4, "_322");

                    return url;
                }))
                .ForMember(x => x.CategoryId, o => o.MapFrom(s => s.Categories.Select(bc => bc.CategoryId).FirstOrDefault()))
                .ForMember(x => x.CategoryName, o => o.MapFrom(s => s.Categories.Select(bc => bc.Category.Name).FirstOrDefault()))
                .ForMember(x => x.Title, o => o.MapFrom(s => s.Title));

            CreateMap<BenefitEditModel, Benefit>()
                .ForMember(x => x.Categories, o => o.MapFrom(s => s.CategoryIds.Select(i => new BenefitCategory { CategoryId = i }).ToList()))
                .ForMember(x => x.Images, o => o.Ignore());

            CreateMap<Benefit, BenefitEditModel>()
                .ForMember(x => x.Images, o => o.MapFrom(s => s.Images
                    .Where(i => (i.Type == AltoImageType.Normal || i.Type == AltoImageType.Main) && i.DeleteDate == null)
                    .OrderBy(i => i.Order)
                    .Select(i => i.CdnUrl ?? i.Url).ToList()))
                .ForMember(x => x.CategoryIds, o => o.MapFrom(s => s.Categories.Select(x => x.CategoryId).ToList()));

            CreateMap<IFormFile, UploadFileModel>()
                .ForMember(x => x.ContentType, o => o.MapFrom(s => s.ContentType))
                .ForMember(x => x.FileName, o => o.MapFrom(s => s.FileName))
                .ForMember(x => x.Stream, o => o.MapFrom(s => s.OpenReadStream()));
        }
    }
}