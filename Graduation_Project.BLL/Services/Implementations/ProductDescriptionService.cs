using System.Net.Http;
using System.Text;
using System.Text.Json;
using Graduation_Project.BLL.Common;
using Graduation_Project.BLL.DTOs.Product;
using Graduation_Project.BLL.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Graduation_Project.BLL.Services.Implementations
{
    public class ProductDescriptionService : IProductDescriptionService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private const string GroqUrl = "https://api.groq.com/openai/v1/chat/completions";

        public ProductDescriptionService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
_apiKey = Environment.GetEnvironmentVariable("GROQ_API_KEY");        }

        public async Task<ServiceResult<string>> GenerateDescriptionAsync(GenerateDescriptionDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.ProductName))
                    return ServiceResult<string>.Failure("Product name is required");

                var prompt = $@"
You are a text autocomplete engine for e-commerce product descriptions.

Your task is to CONTINUE the user's text only.

Product: {dto.ProductName}
Category: {dto.CategoryName}
User input: {dto.PartialText}

Rules:
- Only continue the sentence
- Do NOT rewrite or repeat the full description
- Do NOT start from scratch
- Keep response short (max 1–2 sentences)
- Respond ONLY in English
";

                var requestBody = new
                {
                    model = "llama-3.1-8b-instant",
                    messages = new[]
                    {
                        new
                        {
                            role = "system",
                            content = "You are a text autocomplete engine. You ONLY continue user input, never rewrite full text."
                        },
                        new
                        {
                            role = "user",
                            content = prompt
                        }
                    },
                    max_tokens = 120,
                    temperature = 0.7
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add(
                    "Authorization", $"Bearer {_apiKey}");

                var response = await _httpClient.PostAsync(GroqUrl, content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return ServiceResult<string>.Failure($"Groq API error: {responseBody}");

                var result = JsonSerializer.Deserialize<JsonElement>(responseBody);

                var completion = result
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                // optional: trim spaces
                completion = completion?.Trim();

                return ServiceResult<string>.Success(
                    completion,
                    "Completion generated successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<string>.Failure(
                    $"Error generating description: {ex.Message}");
            }
        }
    }
}