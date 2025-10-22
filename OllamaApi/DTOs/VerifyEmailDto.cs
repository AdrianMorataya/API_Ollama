namespace ChatAPI.DTOs
{
    public class VerifyEmailDto
    {
        public string Email { get; set; } = null!;
        public string Code { get; set; } = null!;
    }
}
