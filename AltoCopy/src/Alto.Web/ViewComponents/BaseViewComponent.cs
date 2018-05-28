using Alto.Dal;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Alto.Web.ViewComponents
{
    public abstract class BaseViewComponent : ViewComponent
    {
        protected AltoDbContext Context { get; }
        protected IMapper Mapper { get; }
        protected BaseViewComponent(AltoDbContext context, MapperConfiguration mapperConfiguration)
        {
            Context = context;
            Mapper = mapperConfiguration.CreateMapper();
        }
    }
}