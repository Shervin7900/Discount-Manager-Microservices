using AutoMapper;

namespace BaseApi.Infrastructure.Mappings;

public class BaseMappingProfile : Profile
{
    public BaseMappingProfile()
    {
        // Add common mappings here if any
        // Example: Automatically ignore audit fields when mapping from DTO to Entity
        // This is a generic pattern you can extend in secondary services
    }

    public static void IgnoreAuditFields<TSource, TDestination>(IMappingExpression<TSource, TDestination> map) 
        where TDestination : Domain.Common.IAuditableEntity
    {
        map.ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
           .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
           .ForMember(dest => dest.LastModifiedAt, opt => opt.Ignore())
           .ForMember(dest => dest.LastModifiedBy, opt => opt.Ignore());
    }
}
