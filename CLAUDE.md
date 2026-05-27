# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

Takes current (or forecast) weather for a given location and finds the most similar weather description in fiction/literature. Weather → natural language description → semantic embedding → Qdrant similarity search → literary quote.

## Architecture

Two separate .NET 9 projects:

**`src/WeatherQuotes.Api`** — ASP.NET Core Minimal API + static frontend  
- `GET /api/quote?location=<city>` — fetches weather, embeds it, returns matching quotes  
- `wwwroot/index.html` — Alpine.js single-page UI served as static files  
- Services: `WeatherService` (Open-Meteo, no key), `EmbeddingService` (OpenAI via Semantic Kernel), `QuoteSearchService` (Qdrant)

**`src/WeatherQuotes.Indexer`** — one-time console app to build the vector index  
- Downloads books from Project Gutenberg automatically (`BookDownloader.cs`)  
- File naming convention: `Author Name - Book Title.txt`  
- Strips Gutenberg headers, splits into paragraphs, filters by weather keywords, embeds, upserts into Qdrant  
- Run once before starting the API

**Qdrant** runs as a native binary in WSL2 (no Docker). Collection: `literary_quotes`, vectors: `text-embedding-3-small` (1536 dims, cosine distance). WSL2 port forwarding makes `localhost:6333` reachable from Windows automatically.

## Commands

```bash
# Start Qdrant in WSL (downloads binary on first run)
wsl bash scripts/run-qdrant.sh

# Run indexer
dotnet run --project src/WeatherQuotes.Indexer

# Start API
dotnet run --project src/WeatherQuotes.Api

# Build
dotnet build
```

## Configuration

`src/WeatherQuotes.Api/appsettings.json` (not committed):
```json
{ "OpenAI": { "ApiKey": "..." } }
```

`src/WeatherQuotes.Indexer/appsettings.json` (not committed):
```json
{ "OpenAI": { "ApiKey": "..." } }
```

Indexer also accepts `OPENAI__APIKEY` env var.

## Adding books to the corpus

The Indexer downloads 20 books automatically on first run. To add more: place `.txt` files from [Project Gutenberg](https://www.gutenberg.org/) into `books/` (relative to working dir when running the Indexer). File name: `Author Name - Book Title.txt`. Re-run the Indexer to index new additions.
