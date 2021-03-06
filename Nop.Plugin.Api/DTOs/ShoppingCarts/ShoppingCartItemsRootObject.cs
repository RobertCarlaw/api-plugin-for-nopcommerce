﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.DTOs.Customers;

namespace Nop.Plugin.Api.DTOs.ShoppingCarts
{
    public class ShoppingCartItemsRootObject : ISerializableObject
    {
        public ShoppingCartItemsRootObject()
        {
            ShoppingCartItems = new List<ShoppingCartItemDto>();
        }

        [JsonProperty("shopping_carts")]
        public IList<ShoppingCartItemDto> ShoppingCartItems { get; set; }

        [JsonProperty("customer_guid")]
        public string CustomerGuid { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "shopping_carts";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof (ShoppingCartItemDto);
        }
    }
}