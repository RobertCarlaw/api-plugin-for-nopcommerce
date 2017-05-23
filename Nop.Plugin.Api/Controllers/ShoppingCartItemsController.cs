using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.Delta;
using Nop.Plugin.Api.DTOs.ShoppingCarts;
using Nop.Plugin.Api.Factories;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.ModelBinders;
using Nop.Plugin.Api.Models.ShoppingCartsParameters;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Controllers
{
    [BearerTokenAuthorize]
    public class ShoppingCartItemsController : BaseApiController
    {
        private readonly IShoppingCartItemApiService _shoppingCartItemApiService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IFactory<ShoppingCartItem> _factory;
        private readonly IShoppingCartFactory<Product> _productShoppingCartFactory;
        private readonly IShoppingCartFactory<Customer> _customerShoppingCartFactory;
        private readonly IShoppingCartFactory<string> _productAttributeCartFactory;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IMapper<string, List<CartItemProductAttributeDto>> _productAttributeXmlToListMapper;

        public ShoppingCartItemsController(IShoppingCartItemApiService shoppingCartItemApiService, 
            IJsonFieldsSerializer jsonFieldsSerializer, 
            IAclService aclService, 
            ICustomerService customerService, 
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService, 
            IShoppingCartService shoppingCartService, 
            IFactory<ShoppingCartItem> factory,
            IPictureService pictureService, IShoppingCartFactory<Product> productShoppingCartFactory, IShoppingCartFactory<Customer> customerShoppingCartFactory, IShoppingCartFactory<string> productAttributeCartFactory, IPriceCalculationService priceCalculationService, IMapper<string, List<CartItemProductAttributeDto>> productAttributeXmlToListMapper)
            :base(jsonFieldsSerializer, 
                 aclService, 
                 customerService, 
                 storeMappingService, 
                 storeService, 
                 discountService,
                 customerActivityService,
                 localizationService,
                 pictureService)
        {
            _shoppingCartItemApiService = shoppingCartItemApiService;
            _shoppingCartService = shoppingCartService;
            _factory = factory;
            _productShoppingCartFactory = productShoppingCartFactory;
            _customerShoppingCartFactory = customerShoppingCartFactory;
            _productAttributeCartFactory = productAttributeCartFactory;
            _priceCalculationService = priceCalculationService;
            _productAttributeXmlToListMapper = productAttributeXmlToListMapper;
        }

        /// <summary>
        /// Receive a list of all shopping cart items
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [ResponseType(typeof(ShoppingCartItemsRootObject))]
        [GetRequestsErrorInterceptorActionFilter]
        public IHttpActionResult GetShoppingCartItems(ShoppingCartItemsParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
            {
                return Error(HttpStatusCode.BadRequest, "limit", "invalid limit parameter");
            }

            if (parameters.Page < Configurations.DefaultPageValue)
            {
                return Error(HttpStatusCode.BadRequest, "page", "invalid page parameter");
            }

            IList<ShoppingCartItem> shoppingCartItems = _shoppingCartItemApiService.GetShoppingCartItems(customerId: null,
                                                                                                         createdAtMin: parameters.CreatedAtMin,
                                                                                                         createdAtMax: parameters.CreatedAtMax, 
                                                                                                         updatedAtMin: parameters.UpdatedAtMin,
                                                                                                         updatedAtMax: parameters.UpdatedAtMax, 
                                                                                                         limit: parameters.Limit,
                                                                                                         page: parameters.Page);

            List<ShoppingCartItemDto> shoppingCartItemsDtos = shoppingCartItems.Select(x => x.ToDto()).ToList();

            var shoppingCartsRootObject = new ShoppingCartItemsRootObject()
            {
                ShoppingCartItems = shoppingCartItemsDtos
            };

            var json = _jsonFieldsSerializer.Serialize(shoppingCartsRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Receive a list of all shopping cart items by customer id
        /// </summary>
        /// <param name="customerId">Id of the customer whoes shopping cart items you want to get</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [ResponseType(typeof(ShoppingCartItemsRootObject))]
        [GetRequestsErrorInterceptorActionFilter]
        public IHttpActionResult GetShoppingCartItemsByCustomerId(int customerId, ShoppingCartItemsForCustomerParametersModel parameters)
        {
            if (customerId <= Configurations.DefaultCustomerId)
            {
                return Error(HttpStatusCode.BadRequest, "customer_id", "invalid customer_id");
            }

            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
            {
                return Error(HttpStatusCode.BadRequest, "limit", "invalid limit parameter");
            }

            if (parameters.Page < Configurations.DefaultPageValue)
            {
                return Error(HttpStatusCode.BadRequest, "page", "invalid page parameter");
            }

            IList<ShoppingCartItem> shoppingCartItems = _shoppingCartItemApiService.GetShoppingCartItems(customerId,
                                                                                                         parameters.CreatedAtMin,
                                                                                                         parameters.CreatedAtMax, parameters.UpdatedAtMin,
                                                                                                         parameters.UpdatedAtMax, parameters.Limit,
                                                                                                         parameters.Page);
            
            if (shoppingCartItems == null)
            {
                return Error(HttpStatusCode.NotFound, "shopping_cart_item", "not found");
            }

            List<ShoppingCartItemDto> shoppingCartItemsDtos = shoppingCartItems.Select(x => x.ToDto()).ToList();

            foreach (var item in shoppingCartItemsDtos)
            {
                var shoppingcartItem = shoppingCartItems.FirstOrDefault(a => a.Id.ToString() == item.Id);

                item.SubTotal = _priceCalculationService.GetSubTotal(shoppingcartItem);
                item.ProductAttributes = _productAttributeXmlToListMapper.Map(shoppingcartItem.AttributesXml);
            }

            var shoppingCartsRootObject = new ShoppingCartItemsRootObject()
            {
                ShoppingCartItems = shoppingCartItemsDtos,
                CustomerId = customerId,
                SubTotal = shoppingCartItemsDtos.Sum(a=>a.SubTotal)
            };

            var json = _jsonFieldsSerializer.Serialize(shoppingCartsRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        [HttpPost]
        [ResponseType(typeof (ShoppingCartItemsRootObject))]
        public IHttpActionResult CreateShoppingCartItem([ModelBinder(typeof (JsonModelBinder<ShoppingCartItemDto>))] Delta<ShoppingCartItemDto> shoppingCartItemDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            // We know that the product id and customer id will be provided because they are required by the validator.
            // TODO: validate
            Product product = _productShoppingCartFactory.CreateFor(shoppingCartItemDelta.Dto);

            if (product == null)
            {
                return Error(HttpStatusCode.NotFound, "product", "not found");
            }

            Customer customer = _customerShoppingCartFactory.CreateFor(shoppingCartItemDelta.Dto); 

            if (customer == null)
            {
                return Error(HttpStatusCode.NotFound, "customer", "not found");
            }
            
            ShoppingCartType shoppingCartType = (ShoppingCartType)Enum.Parse(typeof(ShoppingCartType), shoppingCartItemDelta.Dto.ShoppingCartType);

            ShoppingCartItem newShoppingCartItem = _factory.Initialize();
            shoppingCartItemDelta.Merge(newShoppingCartItem);
            if (!product.IsRental)
            {
                newShoppingCartItem.RentalStartDateUtc = null;
                newShoppingCartItem.RentalEndDateUtc = null;
            }

            string xmlAttributes = _productAttributeCartFactory.CreateFor(shoppingCartItemDelta.Dto);

            IList<string> warnings = _shoppingCartService.AddToCart(customer, product, shoppingCartType, 2, xmlAttributes, 0M, 
                                        shoppingCartItemDelta.Dto.RentalStartDateUtc, shoppingCartItemDelta.Dto.RentalEndDateUtc,
                                        shoppingCartItemDelta.Dto.Quantity ?? 1);

            if (warnings.Count > 0)
            {
                foreach (var warning in warnings)
                {
                    ModelState.AddModelError("shopping cart item", warning);
                }

                return Error(HttpStatusCode.BadRequest);
            }

            // Preparing the result dto of the new product category mapping
            ShoppingCartItemDto newShoppingCartItemDto = newShoppingCartItem.ToDto();
            newShoppingCartItemDto.ProductDto = product.ToDto();
            newShoppingCartItemDto.CustomerDto = customer.ToCustomerForShoppingCartItemDto();
            newShoppingCartItemDto.ShoppingCartType = shoppingCartType.ToString();

            List<ShoppingCartItem> cartItems = _shoppingCartItemApiService.GetShoppingCartItems(customer.Id);

            var shoppingCartsRootObject = new ShoppingCartItemsRootObject();
            shoppingCartsRootObject.ShoppingCartItems = cartItems.Select(x => x.ToDto()).ToList();
            shoppingCartsRootObject.CustomerId = customer.Id;
            var json = _jsonFieldsSerializer.Serialize(shoppingCartsRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpPut]
        [ResponseType(typeof(ShoppingCartItemsRootObject))]
        public IHttpActionResult UpdateShoppingCartItem([ModelBinder(typeof(JsonModelBinder<ShoppingCartItemDto>))] Delta<ShoppingCartItemDto> shoppingCartItemDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            // We kno that the id will be valid integer because the validation for this happens in the validator which is executed by the model binder.
            ShoppingCartItem shoppingCartItemForUpdate =
                _shoppingCartItemApiService.GetShoppingCartItem(int.Parse(shoppingCartItemDelta.Dto.Id));

            if (shoppingCartItemForUpdate == null)
            {
                return Error(HttpStatusCode.NotFound, "shopping_cart_item", "not found");
            }

            Product product = _productShoppingCartFactory.CreateFor(shoppingCartItemDelta.Dto);

            if (product == null)
            {
                return Error(HttpStatusCode.NotFound, "product", "not found");
            }

            Customer customer = _customerShoppingCartFactory.CreateFor(shoppingCartItemDelta.Dto);

            if (customer == null)
            {
                return Error(HttpStatusCode.NotFound, "customer", "not found");
            }

            shoppingCartItemForUpdate.AttributesXml = _productAttributeCartFactory.CreateFor(shoppingCartItemDelta.Dto);
            // Here we make sure that  the product id and the customer id won't be modified.

            shoppingCartItemDelta.Merge(shoppingCartItemForUpdate);

            shoppingCartItemForUpdate.ProductId = product.Id;
            shoppingCartItemForUpdate.CustomerId = customer.Id;
            
            if (!shoppingCartItemForUpdate.Product.IsRental)
            {
                shoppingCartItemForUpdate.RentalStartDateUtc = null;
                shoppingCartItemForUpdate.RentalEndDateUtc = null;
            }

            if (!string.IsNullOrEmpty(shoppingCartItemDelta.Dto.ShoppingCartType))
            {
                ShoppingCartType shoppingCartType = (ShoppingCartType)Enum.Parse(typeof(ShoppingCartType), shoppingCartItemDelta.Dto.ShoppingCartType);
                shoppingCartItemForUpdate.ShoppingCartType = shoppingCartType;
            }
            
            // The update time is set in the service.
            _shoppingCartService.UpdateShoppingCartItem(shoppingCartItemForUpdate.Customer, shoppingCartItemForUpdate.Id,
                shoppingCartItemForUpdate.AttributesXml, shoppingCartItemForUpdate.CustomerEnteredPrice, 
                shoppingCartItemForUpdate.RentalStartDateUtc, shoppingCartItemForUpdate.RentalEndDateUtc,
                shoppingCartItemForUpdate.Quantity);
            
            // Preparing the result dto of the new product category mapping
            ShoppingCartItemDto newShoppingCartItemDto = shoppingCartItemForUpdate.ToDto();
            newShoppingCartItemDto.ProductDto = shoppingCartItemForUpdate.Product.ToDto();
            newShoppingCartItemDto.CustomerDto = shoppingCartItemForUpdate.Customer.ToCustomerForShoppingCartItemDto();
            newShoppingCartItemDto.ShoppingCartType = shoppingCartItemForUpdate.ShoppingCartType.ToString();

            var shoppingCartsRootObject = new ShoppingCartItemsRootObject();

            shoppingCartsRootObject.ShoppingCartItems.Add(newShoppingCartItemDto);

            var json = _jsonFieldsSerializer.Serialize(shoppingCartsRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpDelete]
        [GetRequestsErrorInterceptorActionFilter]
        public IHttpActionResult DeleteShoppingCartItem(int id)
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            ShoppingCartItem shoppingCartItemForDelete = _shoppingCartItemApiService.GetShoppingCartItem(id);

            if (shoppingCartItemForDelete == null)
            {
                return Error(HttpStatusCode.NotFound, "shopping_cart_item", "not found");
            }

            _shoppingCartService.DeleteShoppingCartItem(shoppingCartItemForDelete);

            //activity log
            _customerActivityService.InsertActivity("DeleteShoppingCartItem", _localizationService.GetResource("ActivityLog.DeleteShoppingCartItem"), shoppingCartItemForDelete.Id);

            return new RawJsonActionResult("{}");
        }
    }
}