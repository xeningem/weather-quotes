# Roadmap

Phases are ordered by value delivered. Each phase is independently shippable.

---

## Phase 1 — Working end-to-end demo ✓
_Goal: the full pipeline runs locally with real data._

- [x] Qdrant running via Docker Compose
- [x] Indexer downloads books and populates the collection
- [x] API returns real weather + real quotes for any city
- [x] Frontend connects to local API and displays results

**Done when:** type "London", get a quote, feel something.

---

## Phase 2 — Corpus quality & growth ✓
_Goal: better quotes, fewer irrelevant matches._

- [x] Review and curate the weather keyword filter in `GutenbergLoader`
- [x] Add more books with strong weather/nature prose (Hardy, Conrad, Chekhov, London, Eliot, Lawrence — 44 books total)
- [x] Store book metadata (era, genre, language) in Qdrant payload for future filtering
- [x] Tune minimum/maximum paragraph length for quote quality (120–1200 chars, 27 402 paragraphs indexed)

---

## Phase 3 — UI / UX polish ✓
_Goal: the experience feels intentional, not like a prototype._

- [x] Typography and layout refined (spacing, font sizing, reading rhythm)
- [x] Smooth loading state (skeleton, no layout shift)
- [x] Mobile layout (flex-wrap at 480px)
- [x] Quote card shows weather icon + label alongside the literary text
- [x] "Try another quote" button to cycle through results without re-fetching weather

---

## Phase 4 — Connection visibility ✓
_Goal: make it obvious why a specific quote was chosen for the current weather._

- [x] Expose `weatherProse` (the `ToNaturalLanguage()` text used as the embedding query) in the API response
- [x] Show similarity score as a readable indicator (% match label)
- [x] Highlight words in the quote that also appear in the weather prose
- [x] Debug toggles in the UI to show/hide each piece of debug info (Prose, Score, Highlights)

---

## Phase 5 — Forecast support ✓
_Goal: get a quote for weather that hasn't happened yet._

- [x] Extend `WeatherService` to fetch a daily forecast (Open-Meteo `/forecast?daily=...`)
- [x] Add a day selector to the UI (Today / Tomorrow / +2 days / +3 days pills)
- [x] API endpoint: `GET /api/quote?location=...&offset=0..3`

---

## Phase 6 — Presentation & Description
_Goal: make the project legible to someone who has never seen it — both on GitHub and as a live app._

- [ ] Architecture diagram (Mermaid, embeds natively in GitHub README)
- [ ] README — concept-first, screenshots, architecture, quick-start
- [ ] About / How it works — collapsible section in the UI
- [ ] Demo mode — example pills on the start screen (cities + custom weather prose)
