using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace FCG.Game.Application.DTOs
{
    public class OrderApiRequest
    {
        [JsonPropertyName("userId")]
        public string UserId { get; set; } = string.Empty;

        [JsonPropertyName("currency")]
        public string Currency { get; set; } = "BRL";

        [JsonPropertyName("items")]
        public List<OrderItemApiRequest> Items { get; set; } = new();
    }

    public class OrderItemApiRequest
    {
        [JsonPropertyName("jogoId")]
        public string JogoId { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("unitPrice")]
        public decimal UnitPrice { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
    }
}