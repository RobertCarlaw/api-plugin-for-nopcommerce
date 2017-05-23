using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.DTOs.ShoppingCarts;

namespace Nop.Plugin.Api.DTOs.Shipping
{
    public class ShippingOptionsRootObject : ISerializableObject
    {
        [JsonProperty("shopping_options")]
        public List<ShippingOptionDto> ShippingOptions { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "shopping_options";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(ShippingOptionDto);
        }
    }
}
