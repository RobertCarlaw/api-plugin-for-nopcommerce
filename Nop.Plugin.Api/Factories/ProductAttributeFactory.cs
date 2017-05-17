using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.DataStructures;
using Nop.Plugin.Api.DTOs.ShoppingCarts;
using Nop.Services.Catalog;

namespace Nop.Plugin.Api.Factories
{
    public class ProductAttributeFactory : IShoppingCartFactory<string>, IMapper<string,List<CartItemProductAttributeDto>>
    {
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductAttributeParser _productAttributeParser;

        public ProductAttributeFactory(IProductAttributeService productAttributeService, IProductAttributeParser productAttributeParser)
        {
            _productAttributeService = productAttributeService;
            _productAttributeParser = productAttributeParser;
        }

        public string CreateFor(ShoppingCartItemDto model)
        {
            var productAttributes =
                _productAttributeService.GetProductAttributeMappingsByProductId(model.ProductId.Value);

            var selectedProductAttributeName = model.ProductAttributes.Select(a => a.SelectedValue);

            return productAttributes.Aggregate(string.Empty, (attributesXml, attribute) =>
            {
                var attributeValues = _productAttributeService.GetProductAttributeValues(attribute.Id);

                foreach (var selectedAttributeId in attributeValues
                    .Where(v => selectedProductAttributeName.Contains(v.Name))
                    .Select(v => v.Id)
                    .ToList())
                {
                    attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                        attribute, selectedAttributeId.ToString());
                }
                return attributesXml;
            });
        }

        public List<CartItemProductAttributeDto> Map(string item)
        {
            IList<ProductAttributeValue> productAttributesValues = _productAttributeParser.ParseProductAttributeValues(item);
            List<CartItemProductAttributeDto> model = new List<CartItemProductAttributeDto>();

            foreach (var att in productAttributesValues)
            {
                model.Add(new CartItemProductAttributeDto()
                {
                    Attribute = att.ProductAttributeMapping.ProductAttribute.Name,
                    SelectedValue =  att.Name
                });
            }
            return model;
        }
    }
}
