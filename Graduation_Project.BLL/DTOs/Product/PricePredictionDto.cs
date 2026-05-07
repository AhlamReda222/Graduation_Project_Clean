  using System.Text.Json.Serialization;

 public class PricePredictionDto
    {
        [JsonPropertyName("suggested_price")]
        public decimal SuggestedPrice { get; set; }
 
        [JsonPropertyName("price_range")]
        public PriceRangeDto? PriceRange { get; set; }
 
        [JsonPropertyName("reasoning")]
        public string? Reasoning { get; set; }
    }