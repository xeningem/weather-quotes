# Plan — Phase 6: Presentation & Description

---

**1. Architecture diagram (Mermaid)**

- [ ] Write a Mermaid `flowchart LR` covering: browser → API → WeatherService (Open-Meteo)
      → `ToNaturalLanguage()` → EmbeddingService (Ollama) → QuoteSearchService (Qdrant) → response
- [ ] Add a second diagram (or subgraph) for the Indexer pipeline: Gutenberg → paragraphs → filter → embed → upsert
- [ ] Verify both diagrams render correctly on GitHub (push to branch, check preview)

---

**2. README**

- [ ] Write opening paragraph: concept first ("real weather → literary soul"), not tech first
- [ ] Add screenshot of the main UI (city search, quote card, debug toggles visible)
- [ ] Add screenshot of Custom weather mode with a Russian-language query
- [ ] Embed the Mermaid architecture diagram
- [ ] Add Architecture section: two-project structure, what each does, key tech choices
- [ ] Add Quick start section: WSL/Qdrant/Ollama prerequisites, `dotnet run` commands
- [ ] Add Corpus section: 44 books, how to add more, re-index caveat

---

**3. About / How it works section in UI**

- [ ] Add a `<details><summary>How it works</summary>…</details>` block below the result area
- [ ] Write 3–4 short paragraphs: weather prose generation, embedding, Qdrant search, filtering
- [ ] Style to match existing prose-block aesthetic (same font, muted colour, left border)
- [ ] Keep it collapsed by default; no Alpine.js state needed (`<details>` is native HTML)

---

**4. Demo mode in UI**

- [ ] Define a `demos` array: 5–6 entries mixing real cities (London rain, Moscow frost,
      Naples summer) and custom-weather prose (in English and Russian)
- [ ] Add a "Try an example" row above the search input, shown only when `!result && !loading`
- [ ] Each demo is a pill button; clicking it sets mode + input and fires search
- [ ] Style as the existing `.day-btn` pill pattern (no new CSS class needed)

---

**5. Screenshots**

- [ ] Take screenshot: main result with a winter city (e.g. Moscow) — quote card visible
- [ ] Take screenshot: Custom weather mode with Russian input
- [ ] Take screenshot: debug toggles on (Prose + Highlights visible)
- [ ] Save to `docs/screenshots/` and reference in README with relative paths
