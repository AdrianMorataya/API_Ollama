namespace ChatAPI.DTOs
{
    public class AskOllamaRequest
    {
        public string Prompt { get; set; } = string.Empty;

        public string Model { get; set; } = "llama3";

        public bool NewChat { get; set; } = false;
    }
}
