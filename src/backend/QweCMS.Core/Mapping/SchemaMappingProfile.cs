using AutoMapper;
using QweCMS.Core.Entities;
using QweCMS.Core.Models;

namespace QweCMS.Core.Mapping;

/// <summary>
/// Профиль маппинга для схем
/// </summary>
public class SchemaMappingProfile : Profile
{
    public SchemaMappingProfile()
    {
        // Маппинг CreateSchemaDto -> SchemaEntity
        CreateMap<CreateSchemaDto, SchemaEntity>();

        // Маппинг UpdateSchemaDto -> SchemaEntity
        CreateMap<UpdateSchemaDto, SchemaEntity>();

        // Маппинг SchemaEntity -> SchemaListDto
        CreateMap<SchemaEntity, SchemaListDto>()
            .ForMember(dest => dest.MixinsCount, opt => opt.MapFrom(src => src.Mixins.Count));
    }
}