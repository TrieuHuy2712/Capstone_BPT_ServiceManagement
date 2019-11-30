using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace BPT_Service.Application.AutoMapper
{
    public class AutoMapperConfig
    {
        public static MapperConfiguration RegisterMappings()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new DomainToViewModelMappingProfile());
                cfg.AddProfile(new ViewModelToDomainMappingProfile());
                // cfg.ConstructServicesUsing(ObjectFactory.GetInstance);

            });
        }
    }
}