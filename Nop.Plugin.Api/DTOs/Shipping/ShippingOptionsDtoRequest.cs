using FluentValidation.Attributes;
using Newtonsoft.Json;
using Nop.Plugin.Api.Validators;

namespace Nop.Plugin.Api.DTOs.Shipping
{
    [Validator(typeof(ShippingOptionsDtoRequestValidator))]
    [JsonObject(Title = "shipping_option_request")]
    public class ShippingOptionsDtoRequest
    {
        [JsonProperty("customer_id")]
        public int CustomerId { get; set; }
        [JsonProperty("store_id")]
        public int StoreId { get; set; }
        [JsonProperty("shipping_address")]
        public AddressDto ShippingAddress { get; set; }
    }
}
