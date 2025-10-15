namespace ChatAPI.DTOs
{
    public class PromptHistoryCreateDto
    {
        public string Prompt { get; set; } = string.Empty;
        public string Response { get; set; } = string.Empty;
    }

    public class PromptHistoryResponseDto
    {
        public int Id { get; set; }
        public string Prompt { get; set; } = string.Empty;
        public string Response { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
