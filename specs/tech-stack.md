# Tech Stack

## Current (locked)

| Layer | Choice | Notes |
|---|---|---|
| Backend | ASP.NET Core 10 Minimal API | Single project, no controllers |
| Frontend | Alpine.js + plain HTML/CSS | No build step; may graduate to a framework if UI grows |
| Weather data | Open-Meteo | Free, no API key, geocoding included |
| Embeddings | Ollama `nomic-embed-text` | 768 dims; runs locally in WSL; no API key needed |
| Vector store | Qdrant | Runs as native binary in WSL (no Docker); .NET client via gRPC |
| AI abstraction | Microsoft.Extensions.AI | `IEmbeddingGenerator<string, Embedding<float>>`; Semantic Kernel used only for DI wiring |
| Corpus source | Project Gutenberg | Plain-text `.txt` files; downloaded automatically by the Indexer |

## Intentionally flexible

**Embeddings provider** — `Microsoft.Extensions.AI`'s `IEmbeddingGenerator<string, Embedding<float>>` abstraction means the provider can be swapped (e.g. to OpenAI `text-embedding-3-small`, 1536 dims) without touching search or indexing logic. If the provider changes, the Qdrant collection must be re-indexed from scratch since vector dimensions differ.

**Frontend** — Alpine.js is sufficient while the UI stays simple. If the UI needs richer state (multi-step flows, animations, complex components), migrate to a lightweight framework. The API contract stays stable.

## Not decided yet

- Hosting / deployment target (running locally for now).
