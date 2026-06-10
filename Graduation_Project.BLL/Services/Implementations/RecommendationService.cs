// FILE: BLL/Services/Implementations/RecommendationService.cs
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Graduation_Project.BLL.Common;
using Graduation_Project.BLL.DTOs.Recommendation;
using Graduation_Project.BLL.Services.Interfaces;

namespace Graduation_Project.BLL.Services.Implementations
{
    public class RecommendationService : IRecommendationService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://stormy-rotative-leisa.ngrok-free.dev"; // نفس الـ AI server

        private static readonly JsonSerializerOptions JsonOpts = new()
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public RecommendationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // ══════════════════════════════════════════════════
        // 1. TRACK ACTIVITY → POST /track-activity
        // بيتبعت بعد كل event في UserBehaviorService
        // ══════════════════════════════════════════════════
        public async Task TrackActivityAsync(
            int?   userId,
            int?   productId,
            string actionType,
            string? searchQuery = null)
        {
            try
            {
                var payload = new TrackActivityDto
                {
                    UserId      = userId,
                    ProductId   = productId,
                    ActionType  = actionType,
                    SearchQuery = searchQuery
                };

                var json    = JsonSerializer.Serialize(payload, JsonOpts);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Fire and forget — مش محتاجين ننتظر الرد
                await _httpClient.PostAsync($"{BaseUrl}/track-activity", content);
            }
            catch
            {
                // لو الـ AI server مش شغال، متوقفش الـ request الأصلي
            }
        }

        // ══════════════════════════════════════════════════
        // 2. SIMILAR PRODUCTS → GET /recommendations/product
        // ══════════════════════════════════════════════════
        public async Task<ServiceResult<ProductRecommendationResponseDto>> GetSimilarProductsAsync(
            int productId,
            int limit = 10)
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    $"{BaseUrl}/recommendations/product?product_id={productId}&limit={limit}");

                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return ServiceResult<ProductRecommendationResponseDto>.Failure(
                        $"AI server error: {response.StatusCode}");

                var result = JsonSerializer.Deserialize<AiProductRecommendationResponse>(body, JsonOpts);

                if (result == null || result.Status != "success")
                    return ServiceResult<ProductRecommendationResponseDto>.Failure(
                        "No recommendations available");

                // Map من الـ AI response للـ DTO بتاعنا
                var dto = new ProductRecommendationResponseDto
                {
                    Status       = result.Status,
                    ProductId    = result.ProductId,
                    TotalResults = result.TotalResults,
                    SimilarProducts = result.SimilarProducts?
                        .Select(MapToRecommendedProduct)
                        .ToList() ?? new()
                };

                return ServiceResult<ProductRecommendationResponseDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return ServiceResult<ProductRecommendationResponseDto>.Failure(
                    $"Error fetching recommendations: {ex.Message}");
            }
        }

        // ══════════════════════════════════════════════════
        // 3. HOME RECOMMENDATIONS → GET /recommendations/home
        // ══════════════════════════════════════════════════
        public async Task<ServiceResult<HomeRecommendationResponseDto>> GetHomeRecommendationsAsync(
            int userId,
            int limit = 10)
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    $"{BaseUrl}/recommendations/home?user_id={userId}&limit={limit}");

                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return ServiceResult<HomeRecommendationResponseDto>.Failure(
                        $"AI server error: {response.StatusCode}");

                var result = JsonSerializer.Deserialize<AiHomeRecommendationResponse>(body, JsonOpts);

                if (result == null || result.Status != "success")
                    return ServiceResult<HomeRecommendationResponseDto>.Failure(
                        "No recommendations available");

                var dto = new HomeRecommendationResponseDto
                {
                    Status  = result.Status,
                    UserId  = result.UserId,
                    Type    = result.Type, // "personalized" أو "popular"
                    Recommendations = result.Recommendations?
                        .Select(MapToRecommendedProduct)
                        .ToList() ?? new()
                };

                return ServiceResult<HomeRecommendationResponseDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return ServiceResult<HomeRecommendationResponseDto>.Failure(
                    $"Error fetching home recommendations: {ex.Message}");
            }
        }

        // ══════════════════════════════════════════════════
        // 4. REFRESH MODEL → POST /recommendations/refresh
        // بيتبعت لما يتضاف/يتعدل/يتحذف منتج
        // ══════════════════════════════════════════════════
        public async Task RefreshRecommendationsAsync()
        {
            try
            {
                await _httpClient.PostAsync(
                    $"{BaseUrl}/recommendations/refresh",
                    new StringContent("{}", Encoding.UTF8, "application/json"));
            }
            catch
            {
                // لو الـ AI server مش شغال، متوقفش الـ request الأصلي
            }
        }

        // ── AI response models (بتوافق الـ JSON اللي بيرجع من AI) ──

        private class AiProductRecommendationResponse
        {
            [JsonPropertyName("status")]        public string Status { get; set; } = null!;
            [JsonPropertyName("product_id")]    public int ProductId { get; set; }
            [JsonPropertyName("total_results")] public int TotalResults { get; set; }
            [JsonPropertyName("similar_products")] public List<Dictionary<string, JsonElement>>? SimilarProducts { get; set; }
        }

        private class AiHomeRecommendationResponse
        {
            [JsonPropertyName("status")]          public string Status { get; set; } = null!;
            [JsonPropertyName("user_id")]         public int UserId { get; set; }
            [JsonPropertyName("type")]            public string Type { get; set; } = null!;
            [JsonPropertyName("recommendations")] public List<Dictionary<string, JsonElement>>? Recommendations { get; set; }
        }

        // Map من الـ Dictionary (اللي بييجي من الـ DataFrame) للـ DTO
        private static RecommendedProductDto MapToRecommendedProduct(
            Dictionary<string, JsonElement> dict)
        {
            return new RecommendedProductDto
            {
                ProductId   = GetInt(dict, "product_id", "ProductId", "id"),
                ProductName = GetString(dict, "product_name", "ProductName", "name") ?? "",
                BrandName   = GetString(dict, "brand_name", "BrandName", "brand"),
                Price       = GetDecimal(dict, "price", "Price", "base_price"),
                ImageUrl    = GetString(dict, "image_url", "ImageUrl", "image_urls"),
                Score       = GetDouble(dict, "score", "similarity_score", "Score")
            };
        }

        // Helper methods للـ Dictionary parsing
        private static int GetInt(Dictionary<string, JsonElement> d, params string[] keys)
        {
            foreach (var k in keys)
                if (d.TryGetValue(k, out var v) && v.TryGetInt32(out var i)) return i;
            return 0;
        }

        private static string? GetString(Dictionary<string, JsonElement> d, params string[] keys)
        {
            foreach (var k in keys)
                if (d.TryGetValue(k, out var v) && v.ValueKind == JsonValueKind.String)
                    return v.GetString();
            return null;
        }

        private static decimal GetDecimal(Dictionary<string, JsonElement> d, params string[] keys)
        {
            foreach (var k in keys)
                if (d.TryGetValue(k, out var v) && v.TryGetDecimal(out var dec)) return dec;
            return 0;
        }

        private static double? GetDouble(Dictionary<string, JsonElement> d, params string[] keys)
        {
            foreach (var k in keys)
                if (d.TryGetValue(k, out var v) && v.TryGetDouble(out var dbl)) return dbl;
            return null;
        }
    }
}