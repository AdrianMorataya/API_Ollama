using System.Text;
using System.Text.Json;
using ChatAPI.DTOs;
using ChatAPI.Models;

namespace ChatAPI.Services
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

            try
            {
                var response = await _httpClient.PostAsync("http://localhost:11434/api/generate", content);

                if (!response.IsSuccessStatusCode)
                {
                    return new AskOllamaResponse
                    {
                        Response = $"El modelo '{request.Model}' no pudo generar una respuesta. Código HTTP: {response.StatusCode}"
                    };
                }

                var responseString = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(responseString))
                {
                    return new AskOllamaResponse
                    {
                        Response = $"El modelo '{request.Model}' devolvió una respuesta vacía."
                    };
                }

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

                var finalResponse = sb.ToString();
                if (string.IsNullOrWhiteSpace(finalResponse))
                {
                    finalResponse = $"El modelo '{request.Model}' no generó contenido útil.";
                }

                return new AskOllamaResponse { Response = finalResponse };
            }
            catch (Exception ex)
            {
                return new AskOllamaResponse
                {
                    Response = $"Ocurrió un error al contactar el modelo '{request.Model}': {ex.Message}"
                };
            }
        }
    }
}
