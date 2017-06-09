using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Delta;
using Nop.Plugin.Api.DTOs.ProductAttributeMappings;
using Nop.Plugin.Api.DTOs.Products;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.ModelBinders;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Controllers
{
    [BearerTokenAuthorize]
    public class ProductAttributeMappingController : BaseApiController
    {
        private readonly IProductApiService _productApiService;
        private readonly IProductService _productService;
        private readonly IProductAttributeService _productAttributeService;

        public ProductAttributeMappingController(IJsonFieldsSerializer jsonFieldsSerializer, IAclService aclService,
            ICustomerService customerService, IStoreMappingService storeMappingService, IStoreService storeService,
            IDiscountService discountService, ICustomerActivityService customerActivityService,
            ILocalizationService localizationService, IPictureService pictureService,
            IProductApiService productApiService, IProductService productService, IProductAttributeService productAttributeService)
            : base(
                jsonFieldsSerializer, aclService, customerService, storeMappingService, storeService, discountService,
                customerActivityService, localizationService, pictureService)
        {
            _productApiService = productApiService;
            _productService = productService;
            _productAttributeService = productAttributeService;
        }

        [HttpPut]
        [ResponseType(typeof(ProductsAttributeMappingRootObjectDto))]
        public IHttpActionResult DeleteProductAttributeValue(
            [ModelBinder(typeof(JsonModelBinder<ProductAttributeValueDto>))] Delta<ProductAttributeValueDto>
                productAttributeValueDelta, string sku)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(sku) ||
                productAttributeValueDelta.Dto.ProductAttributeMappingId <= 0)
            {
                return Error();
            }

            var product = _productApiService.GetProductBySku(sku);

            if (product == null)
            {
                return Error(HttpStatusCode.BadRequest,"Product","cannot find product");
            }

            var mapping =
             _productAttributeService.GetProductAttributeMappingById(
                 productAttributeValueDelta.Dto.ProductAttributeMappingId);

            if (mapping == null)
            {
                return Error(HttpStatusCode.BadRequest, "ProductAttributeMapping", "cannot find mapping");
            }

            var item =
                mapping.ProductAttributeValues.FirstOrDefault(
                    a => string.CompareOrdinal(productAttributeValueDelta.Dto.Name, a.Name) == 0);

            if(item != null)
                _productAttributeService.DeleteProductAttributeValue(item);

            ProductsAttributeMappingRootObjectDto dto = new ProductsAttributeMappingRootObjectDto
            {
                ProducAttributesMappings = new List<ProductAttributeMappingDto>() { mapping.ToDto() }
            };

            var json = _jsonFieldsSerializer.Serialize(dto, "");
            return new RawJsonActionResult(json);
        }

        [HttpPut]
        [ResponseType(typeof(ProductsAttributeMappingRootObjectDto))]
        public IHttpActionResult DeleteProductAttributeMapping(
            [ModelBinder(typeof(JsonModelBinder<ProductAttributeMappingDto>))] Delta<ProductAttributeMappingDto>
                productAttributeMappingDelta, string sku)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(sku))
            {
                return Error();
            }

            var product = _productApiService.GetProductBySku(sku);

            if (product == null)
            {
                return new RawJsonActionResult("");
            }

            var isValid = product.ProductAttributeMappings.Any(a => a.Id == productAttributeMappingDelta.Dto.Id);
            if (!isValid)
            {
                return Error(HttpStatusCode.BadRequest, "ProductAttributeMapping", "cannot find mapping");
            }

            var mapping = _productAttributeService.GetProductAttributeMappingById(productAttributeMappingDelta.Dto.Id);
            _productAttributeService.DeleteProductAttributeMapping(mapping);

            ProductsAttributeMappingRootObjectDto dto = new ProductsAttributeMappingRootObjectDto
            {
                ProducAttributesMappings = product.ProductAttributeMappings.Select(a => a.ToDto()).ToList()
            };

            var json = _jsonFieldsSerializer.Serialize(dto, "");
            return new RawJsonActionResult(json);
        }

        [HttpPost]
        [ResponseType(typeof(ProductsAttributeMappingRootObjectDto))]
        public IHttpActionResult CreateProductAttributeValue([ModelBinder(typeof(JsonModelBinder<ProductAttributeValueDto>))] Delta<ProductAttributeValueDto>productAttributeValueDelta, string sku)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(sku) || productAttributeValueDelta.Dto.ProductAttributeMappingId <=0)
            {
                return Error();
            }

            var product = _productApiService.GetProductBySku(sku);

            if (product == null)
            {
                return new RawJsonActionResult("");
            }

            var mapping =
                _productAttributeService.GetProductAttributeMappingById(
                    productAttributeValueDelta.Dto.ProductAttributeMappingId);

            if (mapping == null)
            {
                return Error();
            }

            var item =
                mapping.ProductAttributeValues.FirstOrDefault(
                    a => string.CompareOrdinal(productAttributeValueDelta.Dto.Name, a.Name) == 0);

            if (item == null)
            {
                item = new ProductAttributeValue();
                productAttributeValueDelta.Merge(item);
                _productAttributeService.InsertProductAttributeValue(item);
            }
            else
            {
                productAttributeValueDelta.Merge(item);
                _productAttributeService.UpdateProductAttributeValue(item);
            }

            ProductsAttributeMappingRootObjectDto dto = new ProductsAttributeMappingRootObjectDto
            {
                ProducAttributesMappings = new List<ProductAttributeMappingDto>() { mapping.ToDto() }
            };

            var json = _jsonFieldsSerializer.Serialize(dto, "");
            return new RawJsonActionResult(json);

        }


        /// <summary>
        /// Saves the product attribute mappings and saves them to the DB
        /// </summary>
        /// <param name="productDelta"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(ProductsAttributeMappingRootObjectDto))]
        public IHttpActionResult CreateProductAttributeMapping([ModelBinder(typeof(JsonModelBinder<ProductAttributeMappingDto>))] Delta<ProductAttributeMappingDto> productAttributeMappingDelta, string sku)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(sku))
            {
                return Error();
            }

            var product = _productApiService.GetProductBySku(sku);

            if (product == null)
            {
                return new RawJsonActionResult("");
            }

            var productAttribute =
                _productAttributeService.GetAllProductAttributes()
                    .FirstOrDefault(a => string.CompareOrdinal(a.Name, productAttributeMappingDelta.Dto.ProductAttributeName)==0);

            if (productAttribute == null)
            {
                productAttribute = new ProductAttribute() {Name = productAttributeMappingDelta.Dto.ProductAttributeName };
                _productAttributeService.InsertProductAttribute(productAttribute);
            }

            ProductAttributeMapping mapping = new ProductAttributeMapping();
            productAttributeMappingDelta.Merge(mapping);
            mapping.ProductAttributeId = productAttribute.Id;
            _productAttributeService.InsertProductAttributeMapping(mapping);
            product.ProductAttributeMappings.Add(mapping);

            ProductsAttributeMappingRootObjectDto dto = new ProductsAttributeMappingRootObjectDto
            {
                ProducAttributesMappings = product.ProductAttributeMappings.Select(a => a.ToDto()).ToList()
            };

            var json = _jsonFieldsSerializer.Serialize(dto, "");
            return new RawJsonActionResult(json);
        }
    }
}
