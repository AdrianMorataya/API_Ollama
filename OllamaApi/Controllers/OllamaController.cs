using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChatAPI.Data;
using ChatAPI.DTOs;
using ChatAPI.Models;
using ChatAPI.Services;
using System.Security.Claims;

namespace OllamaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OllamaController : ControllerBase
    {
        private readonly OllamaService _ollamaService;
        private readonly AppDbContext _context;

        public OllamaController(OllamaService ollamaService, AppDbContext context)
        {
            _ollamaService = ollamaService;
            _context = context;
        }

        [Authorize]
        [HttpPost("ask")]
        public async Task<ActionResult<AskOllamaResponse>> Ask([FromBody] AskOllamaRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Prompt))
                return BadRequest("El prompt es obligatorio.");

            var response = await _ollamaService.AskAsync(request);

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var history = new PromptsHistory
            {
                UserId = userId,
                Prompt = request.Prompt,
                Response = response.Response,
                CreatedAt = DateTime.UtcNow
            };

            _context.PromptsHistory.Add(history);
            await _context.SaveChangesAsync();

            return Ok(response);
        }

        [Authorize]
        [HttpGet("history")]
        public async Task<ActionResult<IEnumerable<PromptsHistory>>> GetHistory()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var history = await _context.PromptsHistory
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();

            return Ok(history);
        }

        [Authorize]
        [HttpGet("history/{id}")]
        public async Task<ActionResult<PromptsHistory>> GetHistoryById(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var record = await _context.PromptsHistory
                .FirstOrDefaultAsync(h => h.Id == id && h.UserId == userId);

            if (record == null)
                return NotFound("Historial no encontrado o no te pertenece.");

            return Ok(record);
        }

        [Authorize]
        [HttpDelete("history/{id}")]
        public async Task<IActionResult> DeleteHistory(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var record = await _context.PromptsHistory
                .FirstOrDefaultAsync(h => h.Id == id && h.UserId == userId);

            if (record == null)
                return NotFound("Historial no encontrado o no te pertenece.");

            _context.PromptsHistory.Remove(record);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
