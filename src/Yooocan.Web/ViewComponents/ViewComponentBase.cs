using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Yooocan.Dal;

namespace Yooocan.Web.ViewComponents
{
    public abstract class ViewComponentBase : ViewComponent
    {
        protected ApplicationDbContext Context { get; }
        protected IMapper Mapper { get; }
        protected ViewComponentBase(ApplicationDbContext context, IMapper mapper)
        {
            Context = context;
            Mapper = mapper;
        }        
    }
}
