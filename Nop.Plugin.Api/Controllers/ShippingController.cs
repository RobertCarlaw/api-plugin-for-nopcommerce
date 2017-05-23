using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;
using Nop.Core.Domain.Common;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.DTOs.Shipping;
using Nop.Plugin.Api.DTOs.ShoppingCarts;
using Nop.Plugin.Api.DTOs.Stores;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.ModelBinders;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Controllers
{
    [BearerTokenAuthorize]
    public class ShippingController : BaseApiController
    {
        private readonly IShippingService _shippingService;
        private readonly IShoppingCartItemApiService _shoppingCartItemApiService;
        private readonly ICustomerService _customerService;

        public ShippingController(IJsonFieldsSerializer jsonFieldsSerializer, IAclService aclService, ICustomerService customerService, IStoreMappingService storeMappingService, IStoreService storeService, IDiscountService discountService, ICustomerActivityService customerActivityService, ILocalizationService localizationService, IPictureService pictureService, IShippingService shippingService, IShoppingCartItemApiService shoppingCartItemApiService) : base(jsonFieldsSerializer, aclService, customerService, storeMappingService, storeService, discountService, customerActivityService, localizationService, pictureService)
        {
            _shippingService = shippingService;
            _shoppingCartItemApiService = shoppingCartItemApiService;
            _customerService = customerService;
        }

        [System.Web.Http.HttpPost]
        [ResponseType(typeof(ShippingOptionsRootObject))]
        [GetRequestsErrorInterceptorActionFilter]
        public IHttpActionResult GetShippingOptions(ShippingOptionsDtoRequest requestDto)
        {
            if (!ModelState.IsValid)
            {
                return Error(HttpStatusCode.BadRequest, "customer_id", "invalid customer_id");
            }

            var cartItems = _shoppingCartItemApiService.GetShoppingCartItems(requestDto.CustomerId);

            if (cartItems == null)
            {
                return Error(HttpStatusCode.NotFound, "shopping_cart_item", "not found");
            }

            var customer = _customerService.GetCustomerById(requestDto.CustomerId);
            var address = requestDto.ShippingAddress.ToEntity();

            GetShippingOptionResponse response = _shippingService.GetShippingOptions(cartItems,address, customer, "", requestDto.StoreId);

            if (!response.Success)
            {
                return Error(HttpStatusCode.NotFound, "cartItems", "not found");
            }

            var model = new ShippingOptionsRootObject
            {
                ShippingOptions = response.ShippingOptions.Select(a => a.ToDto()).ToList()
            };

            var json = _jsonFieldsSerializer.Serialize(model,"");
            return new RawJsonActionResult(json); ;
        }

    }
}
