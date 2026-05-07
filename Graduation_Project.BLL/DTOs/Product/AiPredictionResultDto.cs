using System.Text.Json.Serialization;
 
namespace Graduation_Project.BLL.DTOs.Product
{
    public class AiPredictionResultDto
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }
 
        [JsonPropertyName("product_name")]
        public string? ProductName { get; set; }
 
        [JsonPropertyName("price_prediction")]
        public PricePredictionDto? PricePrediction { get; set; }
 
        [JsonPropertyName("price_note")]
        public string? PriceNote { get; set; }
    }
}