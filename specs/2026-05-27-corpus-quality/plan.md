# Plan — Corpus Quality & Growth

## 1. Expand keyword filter and paragraph length limits

- [x] Audit the current keyword list in `GutenbergLoader` — list all terms, identify gaps
- [x] Add weather/nature terms that cover fog, mist, dusk, twilight, wind, storm, cold, heat, season transitions
- [x] Set a minimum paragraph length (e.g. 120 chars) to exclude sentence fragments
- [x] Set a maximum paragraph length (e.g. 1200 chars) to exclude dense multi-sentence blocks
- [ ] Re-run the Indexer against the existing books and verify collection size changes

## 2. Add books to the corpus

- [ ] Identify 8–12 Gutenberg IDs for Hardy, Conrad, Chekhov, Jack London with strong outdoor/weather prose
- [ ] Add IDs to `BookDownloader.cs` download list
- [ ] Add any manually downloaded `.txt` files to `books/` with correct `Author - Title.txt` naming
- [ ] Run the Indexer end-to-end with the expanded book list
- [ ] Spot-check 5–10 indexed quotes for quality and relevance

## 3. Store book metadata in Qdrant payload

- [ ] Define the metadata fields: `era` (e.g. "Victorian", "Edwardian", "20th century"), `genre` (e.g. "novel", "short story"), `language` (e.g. "en")
- [ ] Add metadata to the book manifest or infer from filename/Gutenberg ID in the Indexer
- [ ] Extend the Qdrant upsert in `GutenbergLoader` to include metadata fields in payload
- [ ] Verify payload fields appear correctly via Qdrant dashboard or a test query
- [ ] Update `QuoteResult` model to optionally expose metadata (no API changes yet)
