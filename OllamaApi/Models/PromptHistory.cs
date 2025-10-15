using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ChatAPI.Models
{
    public class PromptsHistory
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [JsonIgnore]
        public User? User { get; set; }

        [Required]
        public string Prompt { get; set; } = null!;

        [Required]
        public string Response { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}