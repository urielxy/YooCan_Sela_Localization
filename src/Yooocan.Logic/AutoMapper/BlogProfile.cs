using System.Linq;
using AutoMapper;
using Yooocan.Entities.Blog;
using Yooocan.Enums;
using Yooocan.Models.Blog;

namespace Yooocan.Logic.AutoMapper
{
    public class BlogProfile : Profile
    {
        public BlogProfile()
        {
            CreateMap<Post, PostModel>()
                .ForMember(x => x.PrimaryImageUrl, o => o.MapFrom(x => x.Images.OrderByDescending(i => i.Type == ImageType.Primary)
                                                                               .ThenBy(i => i.Order)
                                                                               .Select(i => i.Url)
                                                                               .FirstOrDefault()));
        }
    }
}