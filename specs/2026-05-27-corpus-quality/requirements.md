# Requirements — Corpus Quality & Growth

Phase 2 improves the raw material the product runs on. The value of Weather Quotes is the moment of recognition — the world outside suddenly has a voice borrowed from literature. That moment depends entirely on the quality and variety of the indexed corpus: too few books, too narrow a keyword filter, or fragments that read like sentence halves all destroy the effect. This phase expands the corpus, broadens the keyword net, and stores richer metadata so future phases can filter by it.

## In scope

- Expand the weather/nature keyword list in `GutenbergLoader` to index a wider range of relevant paragraphs
- Fix paragraph length limits (min and max character thresholds) so quotes are neither fragments nor walls of text
- Add more books with strong weather/nature prose: Hardy, Conrad, Chekhov, Jack London (and others with similar register)
- Store book metadata (era, genre, language) in the Qdrant payload alongside existing fields

## Out of scope / deferred

- Tune paragraph length thresholds as a standalone task (subsumed into the keyword expansion work)
- Score-based or automated keyword filtering (manual keyword expansion is sufficient at this scale)
- Using the stored metadata for filtering in the API (Phase 3+ concern)

## Decisions

- We will expand the keyword list manually rather than build a scoring system, because the corpus is small enough that hand-curation is faster and easier to reason about.
- We will fix paragraph length limits during the same re-index pass as keyword expansion, not as a separate step.
- We will store metadata (era, genre, language) in Qdrant payload now even though the API does not use it yet, because re-indexing is expensive and it costs nothing to include while we are already re-indexing.

## Constraints

- Adding books requires re-running the Indexer from scratch; the Qdrant collection is rebuilt on each run.
- Embeddings use `nomic-embed-text` (768 dims, Ollama in WSL). Changing the model would require a full re-index.
- Books must come from Project Gutenberg (plain `.txt`, out of copyright). The `BookDownloader` fetches by Gutenberg ID.
- The product is a personal tool — quality bar is "feel something", not precision@k.
