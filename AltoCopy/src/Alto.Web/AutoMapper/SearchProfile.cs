using Alto.Logic.Search;
using Alto.Models.Search;
using AutoMapper;

namespace Alto.Web.AutoMapper
{
    public class SearchProfile : Profile
    {
        public SearchProfile()
        {
            CreateMap<SearchResult, SearchResultModel>();
        }
    }
}
