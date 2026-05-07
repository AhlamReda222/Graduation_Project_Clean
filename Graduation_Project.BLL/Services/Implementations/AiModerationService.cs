using System.Text;
using System.Text.Json;
using Graduation_Project.BLL.DTOs.Product;
using Graduation_Project.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Http;
 
namespace Graduation_Project.BLL.Services.Implementations
{
    public class AiModerationService : IAiModerationService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://stormy-rotative-leisa.ngrok-free.dev";
 
        public AiModerationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
 
        // ======================================================
        // 1. IMAGE VALIDATION
        // POST /validate-image → multipart/form-data (file)
        // ======================================================
        public async Task<ImageValidationResultDto> ValidateImageAsync(IFormFile file)
        {
            try
            {
                using var form = new MultipartFormDataContent();
                using var stream = file.OpenReadStream();
 
                var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType =
                    new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
 
                form.Add(fileContent, "file", file.FileName);
 
                var response = await _httpClient.PostAsync($"{BaseUrl}/validate-image", form);
                var responseBody = await response.Content.ReadAsStringAsync();
 
                if (!response.IsSuccessStatusCode)
                {
                    return new ImageValidationResultDto
                    {
                        IsValid = false,
                        Status = "error",
                        Message = $"AI server error: {response.StatusCode}"
                    };
                }
 
                var json = JsonDocument.Parse(responseBody);
                var root = json.RootElement;
                var status = root.GetProperty("status").GetString();
 
                return new ImageValidationResultDto
                {
                    IsValid = status == "approved",
                    Status = status ?? "unknown",
                    Message = root.TryGetProperty("message", out var msg)
                        ? msg.GetString() : null,
                    AiConfidence = root.TryGetProperty("ai_confidence", out var conf)
                        ? conf.GetDouble() : null,
                    Details = root.TryGetProperty("details", out var details)
                        ? details.GetString() : null
                };
            }
            catch (Exception ex)
            {
                return new ImageValidationResultDto
                {
                    IsValid = false,
                    Status = "error",
                    Message = $"Validation failed: {ex.Message}"
                };
            }
        }
 
        // ======================================================
        // 2. ANALYZE PRODUCT (PRICE PREDICTION)
        // POST /analyze-product → multipart/form-data
        // ======================================================
        public async Task<AiPredictionResultDto> PredictProductAsync(
            AiModerationRequestDto request,
            IFormFile? file,
            string? brandName)
        {
            try
            {
                using var form = new MultipartFormDataContent();
 
                form.Add(new StringContent(request.ProductName ?? ""), "product_name");
                form.Add(new StringContent(request.Description ?? ""), "description");
                form.Add(new StringContent(brandName ?? ""), "brand");
 
                if (file != null)
                {
await using var stream = file.OpenReadStream();
var memoryStream = new MemoryStream();
await stream.CopyToAsync(memoryStream);
memoryStream.Position = 0;

var fileContent = new StreamContent(memoryStream);

fileContent.Headers.ContentType =
    new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

form.Add(fileContent, "file", file.FileName);
                    fileContent.Headers.ContentType =
                        new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
 
                    form.Add(fileContent, "file", file.FileName);
                }
 
                var response = await _httpClient.PostAsync($"{BaseUrl}/analyze-product", form);
                var responseBody = await response.Content.ReadAsStringAsync();
 
                if (!response.IsSuccessStatusCode)
                    throw new Exception($"AI error: {response.StatusCode} - {responseBody}");
 
                return JsonSerializer.Deserialize<AiPredictionResultDto>(
                    responseBody,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                ) ?? new AiPredictionResultDto { Status = "error" };
            }
            catch (Exception ex)
            {
                throw new Exception($"Price prediction failed: {ex.Message}");
            }
        }
 
        // ======================================================
        // 3. MODERATE PRODUCT (TEXT MODERATION)
        // POST /moderate-product → application/json
        // ======================================================
        public async Task<AiModerationResultDto> ModerateProductAsync(AiModerationRequestDto request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
 
                var response = await _httpClient.PostAsync($"{BaseUrl}/moderate-product", content);
                var responseBody = await response.Content.ReadAsStringAsync();
 
                if (!response.IsSuccessStatusCode)
                    throw new Exception($"AI failed: {response.StatusCode} - {responseBody}");
 
                var result = JsonDocument.Parse(responseBody);
                var root = result.RootElement;
                var status = root.GetProperty("status").GetString();
 
                return new AiModerationResultDto
                {
                    IsApproved = status == "auto_approved",
                    Status = status ?? "unknown",
                    Message = root.TryGetProperty("message", out var msg)
                        ? msg.GetString() : null,
                    Reason = root.TryGetProperty("reason", out var reason)
                        ? reason.GetString() : null
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Moderation failed: {ex.Message}");
            }
        }
    }
}