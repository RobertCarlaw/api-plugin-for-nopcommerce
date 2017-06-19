using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.DTOs.Customers;

namespace Nop.Plugin.Api.DTOs.Orders
{
    public class OrdersRootObject : ISerializableObject
    {
        public OrdersRootObject()
        {
            Orders = new List<OrderDto>();
        }

        [JsonProperty("orders")]
        public IList<OrderDto> Orders { get; set; }

        [JsonProperty("order")]
        public OrderDto Order { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "orders";
        }

        [JsonProperty("total_records")]
        public int TotalRecords { get; set; }

        public Type GetPrimaryPropertyType()
        {
            return typeof (OrderDto);
        }

        [JsonProperty("success")]
        public bool Success { get; set; }
    }
}