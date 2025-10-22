using ChatAPI.Data;
using ChatAPI.DTOs;
using ChatAPI.Models;
using ChatAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;

        public AuthController(AppDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username || u.Email == dto.Email))
                return BadRequest("Username or email already exists.");

            var code = new Random().Next(100000, 999999).ToString();

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "User",
                IsActive = true,
                IsEmailConfirmed = false,
                EmailConfirmationCode = code
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var emailService = new EmailService();
            var body = $"Hola {user.Username},<br/><br/>Tu código de verificación es: <b>{code}</b>";
            emailService.SendEmail(user.Email, "Código de verificación", body);

            return Ok(new { message = "Usuario registrado correctamente. Revisa tu correo para ingresar el código de verificación." });
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                return BadRequest("Correo no encontrado.");

            if (user.IsEmailConfirmed)
                return BadRequest("Correo ya verificado.");

            if (user.EmailConfirmationCode != dto.Code)
                return BadRequest("Código incorrecto.");

            user.IsEmailConfirmed = true;
            user.EmailConfirmationCode = null;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Correo verificado correctamente" });
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => (u.Username == dto.UsernameOrEmail || u.Email == dto.UsernameOrEmail) && u.IsActive);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials.");

            if (!user.IsEmailConfirmed)
                return Unauthorized("Debes verificar tu correo antes de iniciar sesión.");

            var token = _jwtService.GenerateToken(user);

            return Ok(new AuthResponseDto
            {
                Token = token,
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role,
                    IsActive = user.IsActive
                }
            });
        }
    }
}