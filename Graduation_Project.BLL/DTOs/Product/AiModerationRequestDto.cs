using System.Text.Json.Serialization;

namespace Graduation_Project.BLL.DTOs.Product
{
 public class AiModerationRequestDto
    {
        [JsonPropertyName("product_name")]
        public string ProductName { get; set; }
 
        [JsonPropertyName("description")]
        public string Description { get; set; }
 
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
 
        [JsonPropertyName("category")]
        public string Category { get; set; }
 
        [JsonPropertyName("images")]
        public List<string> Images { get; set; } = new();
    }}