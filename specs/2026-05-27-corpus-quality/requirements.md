# Requirements — Corpus Quality & Growth

Phase 2 improves the raw material the product runs on. The value of Weather Quotes is the moment of recognition — the world outside suddenly has a voice borrowed from literature. That moment depends entirely on the quality and variety of the indexed corpus: too few books, too narrow a keyword filter, or fragments that read like sentence halves all destroy the effect. This phase expands the corpus, broadens the keyword net, and stores richer metadata so future phases can filter by it.

## In scope

- Expand the weather/nature keyword list in `GutenbergLoader` to index a wider range of relevant paragraphs
- Fix paragraph length limits (min 120 / max 1200 chars) so quotes are neither fragments nor walls of text
- Add 14 more books with strong weather/nature prose: Conrad, Hardy, Chekhov, Jack London, George Eliot, D.H. Lawrence
- Store book metadata (era, genre, language) in the Qdrant payload alongside existing fields
- Expose metadata fields (`Era`, `Genre`, `Language`) in `QuoteResult` API response (nullable — docs without metadata return null)

## Out of scope / deferred

- Score-based or automated keyword filtering (manual keyword expansion is sufficient at this scale)
- Using stored metadata for API filtering (Phase 3+ concern)
- Translations or non-English corpora

## Decisions

- We will expand the keyword list manually rather than build a scoring system, because the corpus is small enough that hand-curation is faster and easier to reason about.
- We will fix paragraph length limits during the same re-index pass as keyword expansion, not as a separate step.
- We will store metadata (era, genre, language) in Qdrant payload now even though the API does not filter by it yet, because re-indexing is expensive and it costs nothing to include while we are already re-indexing.
- Metadata fields in `QuoteResult` are nullable strings so the API stays backwards-compatible when serving docs indexed before metadata was added.

## Constraints

- Adding books in alphabetical positions between existing ones requires a full Qdrant collection drop and re-index from scratch (resume-from-checkpoint assumes stable ordering).
- Embeddings use `nomic-embed-text` (768 dims, Ollama in WSL2). Changing the model would require a full re-index.
- Books must come from Project Gutenberg (plain `.txt`, out of copyright). `BookDownloader` fetches by Gutenberg ID; "not found" IDs are skipped silently.
- Qdrant runs as a native binary in WSL2 (no Docker). `localhost:6333` is reachable from Windows via WSL2 port forwarding.
- The product is a personal tool — quality bar is "feel something", not precision@k.
