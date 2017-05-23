using Nop.Plugin.Api.AutoMapper;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Api.DTOs.Shipping;

namespace Nop.Plugin.Api.MappingExtensions
{
    public static class ShippingOptionDtoMappings
    {
        public static ShippingOptionDto ToDto(this ShippingOption option)
        {
            return option.MapTo<ShippingOption, ShippingOptionDto>();
        }
    }
}