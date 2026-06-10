
using System.Text.Json.Serialization;

namespace Graduation_Project.BLL.DTOs.Recommendation
{
    // الـ DTO اللي بنبعته للـ AI
    public class TrackActivityDto
    {
        [JsonPropertyName("user_id")]
        public int? UserId { get; set; }
 
        [JsonPropertyName("product_id")]
        public int? ProductId { get; set; }
 
        [JsonPropertyName("action_type")]
        public string ActionType { get; set; } = null!;
 
        [JsonPropertyName("search_query")]
        public string? SearchQuery { get; set; }
    }
}
