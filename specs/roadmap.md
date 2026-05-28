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

## Phase 4 — Forecast support
_Goal: get a quote for weather that hasn't happened yet._

- [ ] Extend `WeatherService` to fetch a daily forecast (Open-Meteo supports this)
- [ ] Add a date/day selector to the UI ("today", "tomorrow", "+3 days")
- [ ] API endpoint: `GET /api/quote?location=...&date=...`
