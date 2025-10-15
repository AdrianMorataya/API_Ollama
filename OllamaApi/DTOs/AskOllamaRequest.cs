namespace OllamaApi.DTOs
{
    public class AskOllamaRequest
    {
        public string Prompt { get; set; } = string.Empty;

        // Opcional: permití elegir modelo dinámicamente
        public string Model { get; set; } = "llama3";
    }
}
