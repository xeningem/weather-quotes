# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

Takes current (or forecast) weather for a given location and finds the most similar weather description in fiction/literature. Weather → natural language description → semantic embedding → Qdrant similarity search → literary quote.

## Architecture

Two separate .NET 10 projects:

**`src/WeatherQuotes.Api`** — ASP.NET Core Minimal API + static frontend  
- `GET /api/quote?location=<city>` — fetches weather, embeds it, returns matching quotes  
- `wwwroot/index.html` — Alpine.js single-page UI served as static files  
- Services: `WeatherService` (Open-Meteo, no key), `EmbeddingService` (`IEmbeddingGenerator<string, Embedding<float>>` via `Microsoft.Extensions.AI`), `QuoteSearchService` (Qdrant)
- Interfaces `IWeatherService` and `IQuoteSearchService` extracted for testability

**`src/WeatherQuotes.Indexer`** — one-time console app to build the vector index  
- Downloads ~44 books from Project Gutenberg automatically (`BookDownloader.cs`)  
- File naming convention: `Author Name - Book Title.txt`  
- Strips Gutenberg headers, splits into paragraphs (120–1200 chars), filters by ~75 weather keywords, embeds, upserts into Qdrant  
- Resume-from-checkpoint: reads existing Qdrant point count and skips already-indexed paragraphs  
- **Important**: adding new books that sort alphabetically between existing ones requires a full collection drop and re-index (resume assumes stable ordering)

**Qdrant** runs as a native binary in WSL2 (no Docker). Collection: `literary_quotes`, vectors: `nomic-embed-text` (768 dims, cosine distance). WSL2 port forwarding makes `localhost:6333` reachable from Windows automatically.

**Ollama** runs in WSL2, serves `nomic-embed-text` for embeddings at `http://localhost:11434`.

## Commands

```bash
# Start Qdrant in WSL (downloads binary on first run)
wsl bash scripts/run-qdrant.sh

# Run indexer (drop collection first if adding books between existing ones alphabetically)
dotnet run --project src/WeatherQuotes.Indexer

# Start API
dotnet run --project src/WeatherQuotes.Api

# Run tests
dotnet test

# Build
dotnet build
```

## Configuration

The API and Indexer use Ollama locally — no API key required.

`src/WeatherQuotes.Api/appsettings.json` — not needed for Ollama setup (endpoint hardcoded to `http://localhost:11434/v1`).

## Adding books to the corpus

The Indexer downloads ~44 books automatically on first run. To add more:
1. Add the Gutenberg ID and metadata to `BookDownloader.cs` and `GutenbergLoader.cs`
2. If the new title sorts alphabetically between existing files: delete the Qdrant collection first, then re-run
3. If it sorts at the end: just re-run (resume-from-checkpoint will pick it up)

## Tests

89 tests across three layers:
- **Unit** (`GutenbergLoaderTests`, `WmoCodeTests`, `WeatherDataTests`) — pure functions, no external deps
- **Integration** (`WeatherServiceTests`, `QuoteFilterTests`) — fake `HttpMessageHandler`, internal filter access
- **E2E** (`QuoteEndpointTests`) — `WebApplicationFactory<Program>` with stub `IWeatherService`/`IQuoteSearchService`
