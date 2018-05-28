using System.Reflection;
using AutoMapper;

namespace Alto.Web.AutoMapper
{
    public static class AutoMapperInitializer
    {
        public static MapperConfiguration Init()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfiles(typeof(BenefitProfile).GetTypeInfo().Assembly);
            });
            
            return config;
        }
    }
}