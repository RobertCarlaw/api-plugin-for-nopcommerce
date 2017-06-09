using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.DTOs.Categories;
using Nop.Plugin.Api.DTOs.Orders;
using Nop.Plugin.Api.DTOs.ProductAttributeMappings;
using Nop.Plugin.Api.DTOs.Products;

namespace Nop.Plugin.Api.Helpers
{
    public interface IDTOHelper
    {
        ProductDto PrepareProductDTO(Product product);
        CategoryDto PrepareCategoryDTO(Category category);
        OrderDto PrepareOrderDTO(Order order);

        List<ProductAttributeMappingDto> PrepareProductAttributeMappingDtos(
            IEnumerable<ProductAttributeMapping> mappings);
    }
}
