using Microsoft.AspNetCore.Mvc;
using OllamaApi.DTOs;
using OllamaApi.Models;
using OllamaApi.Services;

namespace OllamaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OllamaController : ControllerBase
    {
        private readonly OllamaService _ollamaService;

        public OllamaController(OllamaService ollamaService)
        {
            _ollamaService = ollamaService;
        }

        [HttpPost("ask")]
        public async Task<ActionResult<AskOllamaResponse>> Ask([FromBody] AskOllamaRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Prompt))
                return BadRequest("El prompt es obligatorio.");

            var response = await _ollamaService.AskAsync(request);
            return Ok(response);
        }
    }
}
