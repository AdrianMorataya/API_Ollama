namespace ChatAPI.DTOs
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = null!;
        public UserDto User { get; set; } = null!;
    }
}