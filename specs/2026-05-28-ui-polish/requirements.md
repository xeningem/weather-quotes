# Requirements — UI/UX Polish

Phase 3 makes the experience feel intentional, not like a prototype. The app's value is a moment of recognition — weather outside finding a voice in literature — and the UI must not get in the way of that moment. Typography should serve reading, the loading state should feel calm, and the layout must work on the device where you actually check the weather: your phone.

## In scope

- **Typography and layout** — font sizing, line height, spacing tuned so the quote reads like a page from a book, not a web widget
- **Smooth loading state** — skeleton or fade-in transition; no layout shift when results arrive
- **Mobile layout** — single-column, comfortable padding, no horizontal scroll at 375px
- **Weather label on the quote card** — a small text label (e.g. "fog · 8 °C") alongside the literary text, sourced from the weather already returned by the API; no icon font dependency
- **"Try another quote" button** — client-side: the API already returns 3 quotes; the button cycles through them without a new network request

## Out of scope / deferred

- Weather icons (SVG/icon font) — text label is sufficient and avoids a dependency
- Dark mode
- Animations beyond a simple fade
- Multi-quote carousel with swipe gesture (may revisit in a later phase)

## Decisions

- **Weather label, not icon** — avoids adding an icon font or SVG sprite; a short text label ("overcast · 14 °C") carries the same information with zero additional assets.
- **Client-side cycling for "Try another"** — API already returns up to 3 quotes per request; cycling in Alpine.js state costs nothing and keeps Ollama idle.
- **No build step** — the frontend stays as plain HTML/CSS/Alpine.js; no bundler, no npm.

## Constraints

- Frontend: Alpine.js + plain HTML/CSS, no build step (from tech-stack.md)
- The API contract (`/api/quote`) must not change — only the UI consumes it differently
- All existing 89 automated tests must continue to pass
