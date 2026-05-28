# Plan — Connection Visibility

## 1. Expose weatherProse in the API

- [ ] Add `WeatherProse` property to the response model (anonymous object or a new `QuoteResponse` record in `QuoteEndpoints.cs`)
- [ ] Pass `weather.ToNaturalLanguage()` as `weatherProse` in the `GET /api/quote` handler
- [ ] Add an E2E test asserting `weatherProse` is present and non-empty in the response
- [ ] Confirm existing 89 tests still pass after the change

## 2. Show similarity score in the UI

- [ ] Read `q.score` (already in API response) and compute `Math.round(q.score * 100)`
- [ ] Display as a muted label in `.quote-meta`: e.g. `· 84% match`
- [ ] Controlled by a `showScore` Alpine boolean (default `false`); toggle hides/shows the label
- [ ] Style to be clearly secondary — same muted colour as era/genre, no border

## 3. Highlight common words in the quote

- [ ] Compute word intersection in Alpine.js: split both `weatherProse` and `q.text` on `\W+`, lowercase, filter stop-words (a, the, in, of, …), find overlap
- [ ] Wrap matched words in `<mark>` via `x-html` (replace plain `x-text` on `.quote-text`)
- [ ] Controlled by a `showHighlights` Alpine boolean (default `false`)
- [ ] Style `mark` with a warm tint (`background: #f5e6c8; border-radius: 2px`) that fits the palette

## 4. Collapsible weather prose block

- [ ] Add a `showProse` Alpine boolean (default `false`)
- [ ] Below the quote card, render a `<details>`-style block (Alpine `x-show`) with the `weatherProse` text
- [ ] Style as a quiet aside: smaller font, muted border-left, italic, clearly subordinate to the quote

## 5. Debug toggles row

- [ ] Add a `Show:` row beneath the "Try another" button with three pill-style checkboxes/toggles: `Prose`, `Score`, `Highlights`
- [ ] Each toggle binds to `showProse` / `showScore` / `showHighlights`
- [ ] Toggles reset to `false` on each new search
- [ ] Style toggles visually lighter than the main card — small, inline, muted
