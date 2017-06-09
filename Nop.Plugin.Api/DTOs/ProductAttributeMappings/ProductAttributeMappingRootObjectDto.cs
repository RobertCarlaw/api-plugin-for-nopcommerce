using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.DTOs.Products;

namespace Nop.Plugin.Api.DTOs.ProductAttributeMappings
{
    public class ProductsAttributeMappingRootObjectDto : ISerializableObject
    {
        public ProductsAttributeMappingRootObjectDto()
        {
            ProducAttributesMappings = new List<ProductAttributeMappingDto>();
        }

        [JsonProperty("product_attribute_mappings")]
        public IList<ProductAttributeMappingDto> ProducAttributesMappings { get; set; }

        [JsonProperty("product_attribute_mapping")]
        public ProductAttributeMappingDto ProductAttributesMapping { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "product_attributes";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof (ProductDto);
        }
    }
}