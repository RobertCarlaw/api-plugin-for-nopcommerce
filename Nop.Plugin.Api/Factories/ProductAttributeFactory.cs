using System.Linq;
using Nop.Plugin.Api.DTOs.ShoppingCarts;
using Nop.Services.Catalog;

namespace Nop.Plugin.Api.Factories
{
    public class ProductAttributeFactory : IShoppingCartFactory<string>
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
    }
}
