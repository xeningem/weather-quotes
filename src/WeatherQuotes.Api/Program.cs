using Microsoft.SemanticKernel;
using OpenAI;
using System.ClientModel;
using Qdrant.Client;
using WeatherQuotes.Api.Endpoints;
using WeatherQuotes.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<WeatherService>();

var ollamaClient = new OpenAIClient(
    new ApiKeyCredential("ollama"),
    new OpenAIClientOptions { Endpoint = new Uri("http://localhost:11434/v1") }
);

builder.Services.AddOpenAIEmbeddingGenerator("nomic-embed-text", ollamaClient);
builder.Services.AddSingleton<EmbeddingService>();
builder.Services.AddSingleton(_ => new QdrantClient("localhost"));
builder.Services.AddSingleton<QuoteSearchService>();

builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapQuoteEndpoints();

app.Run();
