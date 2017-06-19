using System;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.Models.OrdersParameters
{
    public class OrderSearchParametersModel : BaseOrdersParametersModel
    {
        [JsonProperty("order_id")]
        public int OrderNumber { get; set; }

        [JsonProperty("customer_name")]
        public string CustomerName { get; set; }

        [JsonProperty("email_address")]
        public string EmailAddress { get; set; }

        [JsonProperty("fields")]
        public string Fields { get; set; }
    }
}
