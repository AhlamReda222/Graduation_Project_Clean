  using System.Text.Json.Serialization;

 public class PriceRangeDto
    {
        [JsonPropertyName("min")]
        public decimal Min { get; set; }
 
        [JsonPropertyName("max")]
        public decimal Max { get; set; }
    }