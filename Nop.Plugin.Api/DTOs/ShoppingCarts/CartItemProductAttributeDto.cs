using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTOs.ShoppingCarts
{
    [JsonObject(Title = "product_attributes")]
    public class CartItemProductAttributeDto
    {
        [JsonProperty("attribute")]
        public string Attribute { get; set; }

        [JsonProperty("selected")]
        public string SelectedValue { get; set; }
    }
}
