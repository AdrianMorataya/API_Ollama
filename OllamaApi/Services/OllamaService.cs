using System.Text;
using System.Text.Json;
using OllamaApi.DTOs;
using OllamaApi.Models;

namespace OllamaApi.Services
{
    public class OllamaService
    {
        private readonly HttpClient _httpClient;

        public OllamaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AskOllamaResponse> AskAsync(AskOllamaRequest request)
        {
            var payload = new
            {
                model = request.Model,
                prompt = request.Prompt
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("http://localhost:11434/api/generate", content);
            var responseString = await response.Content.ReadAsStringAsync();

            var lines = responseString.Split("\n", StringSplitOptions.RemoveEmptyEntries);

            var sb = new StringBuilder();
            foreach (var line in lines)
            {
                try
                {
                    using var doc = JsonDocument.Parse(line);
                    if (doc.RootElement.TryGetProperty("response", out var resp))
                    {
                        sb.Append(resp.GetString());
                    }
                }
                catch
                {
                }
            }
            return new AskOllamaResponse
            {
                Response = sb.ToString()
            };
        }
    }
}