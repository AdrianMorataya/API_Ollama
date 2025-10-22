using System.Text;
using System.Text.Json;
using ChatAPI.DTOs;
using ChatAPI.Models;
using Microsoft.AspNetCore.Http;

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

            return await SendToOllamaAsync(payload, request.Model);
        }

        public async Task<AskOllamaResponse> AskVisionAsync(AskVisionForm request)
        {
            if (request.Image == null || request.Image.Length == 0)
                return new AskOllamaResponse { Response = "No se recibió ninguna imagen." };

            string base64Image;
            using (var ms = new MemoryStream())
            {
                await request.Image.CopyToAsync(ms);
                base64Image = Convert.ToBase64String(ms.ToArray());
            }

            var payload = new
            {
                model = request.Model,
                prompt = request.Prompt,
                images = new[] { base64Image }
            };

            return await SendToOllamaAsync(payload, request.Model);
        }

        private async Task<AskOllamaResponse> SendToOllamaAsync(object payload, string model)
        {
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("http://localhost:11434/api/generate", content);

                if (!response.IsSuccessStatusCode)
                    return new AskOllamaResponse
                    {
                        Response = $"El modelo '{model}' no pudo generar respuesta. Código HTTP: {response.StatusCode}"
                    };

                var responseString = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(responseString))
                    return new AskOllamaResponse
                    {
                        Response = $"El modelo '{model}' devolvió una respuesta vacía."
                    };

                var sb = new StringBuilder();
                var lines = responseString.Split("\n", StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    try
                    {
                        using var doc = JsonDocument.Parse(line);
                        if (doc.RootElement.TryGetProperty("response", out var resp))
                            sb.Append(resp.GetString());
                    }
                    catch { }
                }

                var finalResponse = sb.ToString();
                if (string.IsNullOrWhiteSpace(finalResponse))
                    finalResponse = $"El modelo '{model}' no generó contenido útil.";

                return new AskOllamaResponse { Response = finalResponse };
            }
            catch (Exception ex)
            {
                return new AskOllamaResponse
                {
                    Response = $"Error al contactar el modelo '{model}': {ex.Message}"
                };
            }
        }
    }
}