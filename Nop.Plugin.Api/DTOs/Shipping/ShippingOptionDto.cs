using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTOs.Shipping
{
    [JsonObject(Title = "shipping_option")]
    public class ShippingOptionDto
    {
        [JsonProperty("system_name")]
        public string SystemName { get; set; }

        [JsonProperty("rate")]
        public decimal Rate { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
