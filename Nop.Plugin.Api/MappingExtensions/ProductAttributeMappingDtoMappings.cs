using Nop.Plugin.Api.AutoMapper;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.DTOs.ProductAttributeMappings;

namespace Nop.Plugin.Api.MappingExtensions
{
    public static class ProductAttributeMappingDtoMappings
    {
        public static ProductAttributeMappingDto ToDto(this ProductAttributeMapping mapping)
        {
            return mapping.MapTo<ProductAttributeMapping, ProductAttributeMappingDto>();
        }
    }
}