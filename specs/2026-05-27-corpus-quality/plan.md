# Plan — Corpus Quality & Growth

## 1. Expand keyword filter and paragraph length limits

- [x] Audit the current keyword list in `GutenbergLoader` — list all terms, identify gaps
- [x] Add weather/nature terms that cover fog, mist, dusk, twilight, wind, storm, cold, heat, season transitions (~30 → ~75 terms)
- [x] Set a minimum paragraph length (120 chars) to exclude sentence fragments
- [x] Set a maximum paragraph length (1200 chars) to exclude dense multi-sentence blocks
- [x] Re-run the Indexer from scratch — 26 323 weather paragraphs extracted (was ~14 000 before), full re-index in progress

## 2. Add books to the corpus

- [x] Identified 14 new titles: Conrad (Typhoon, The Secret Sharer), Hardy (The Mayor of Casterbridge, The Woodlanders), London (Burning Daylight, Martin Eden), Chekhov (Uncle Vanya, Three Sisters), Eliot (Middlemarch, The Mill on the Floss), Lawrence (Sons and Lovers, Women in Love)
- [x] Added Gutenberg IDs to `BookDownloader.cs` download list
- [x] 12 of 14 books downloaded automatically (Ward Number Six not found at ID 2738 — skip)
- [x] `BookMetadataLookup` in `GutenbergLoader.cs` updated for all new titles
- [x] Indexer re-running with expanded book list (26 323 paragraphs vs ~14 000 before)
- [ ] Spot-check 5–10 indexed quotes for quality and relevance (pending indexer completion)

## 3. Store book metadata in Qdrant payload

- [x] Defined metadata fields: `era` (e.g. "Victorian", "Edwardian", "Early 20th century"), `genre` (e.g. "novel", "short story", "play"), `language` ("en")
- [x] Added `BookMetadata` record and `BookMetadataLookup` dictionary to `GutenbergLoader.cs` (44 books total)
- [x] Indexer upsert extended with `era`, `genre`, `language` payload fields
- [x] `QuoteResult` model updated: `Era?`, `Genre?`, `Language?` nullable fields added
- [x] `QuoteSearchService` reads metadata from Qdrant payload with safe `TryGetValue` access (null-safe for pre-metadata docs)
- [ ] Verify payload fields appear correctly via Qdrant API query (pending indexer completion)
