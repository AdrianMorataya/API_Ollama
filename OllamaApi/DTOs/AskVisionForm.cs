using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public class AskVisionForm
{
    [FromForm(Name = "prompt")]
    public string Prompt { get; set; } = string.Empty;

    [FromForm(Name = "image")]
    public IFormFile Image { get; set; } = null!;

    [FromForm(Name = "model")]
    public string Model { get; set; } = "gemma3:4b";
}
