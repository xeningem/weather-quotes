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

## Phase 2 — Corpus quality & growth
_Goal: better quotes, fewer irrelevant matches._

- [ ] Review and curate the weather keyword filter in `GutenbergLoader`
- [ ] Add more books with strong weather/nature prose (Hardy, Conrad, Chekhov, London)
- [ ] Store book metadata (era, genre, language) in Qdrant payload for future filtering
- [ ] Tune minimum/maximum paragraph length for quote quality

---

## Phase 3 — UI / UX polish
_Goal: the experience feels intentional, not like a prototype._

- [ ] Typography and layout refined (spacing, font sizing, reading rhythm)
- [ ] Smooth loading state (no layout shift)
- [ ] Mobile layout
- [ ] Quote card shows a short weather icon or label alongside the literary text
- [ ] "Try another quote" button to cycle through results without re-fetching weather

---

## Phase 4 — Forecast support
_Goal: get a quote for weather that hasn't happened yet._

- [ ] Extend `WeatherService` to fetch a daily forecast (Open-Meteo supports this)
- [ ] Add a date/day selector to the UI ("today", "tomorrow", "+3 days")
- [ ] API endpoint: `GET /api/quote?location=...&date=...`
