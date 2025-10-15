using Microsoft.OpenApi.Models;
using OllamaApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Ollama API",
        Version = "v1",
        Description = "API en C# conectada a Ollama (modelos locales o en la nube).",
        Contact = new OpenApiContact
        {
            Name = "Ollama",
            Email = "adrianmorataya01@gmail.com"
        }
    });
});

builder.Services.AddHttpClient<OllamaService>();
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.DocumentTitle = "Ollama API Docs";
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ollama API v1");
    });
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
