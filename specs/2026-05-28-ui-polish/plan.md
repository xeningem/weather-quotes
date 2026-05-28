# Plan — UI/UX Polish

## 1. Typography and layout

- [ ] Audit current `index.html` — list every font-size, line-height, margin, and colour in use
- [ ] Choose a readable serif or serif-adjacent stack for quote text (system-ui or Google Fonts with no build step); set body in sans-serif
- [ ] Set quote font-size ≥ 1.2rem, line-height ≥ 1.7, max-width ≈ 65ch for comfortable reading
- [ ] Tighten spacing: consistent vertical rhythm between input, weather summary, and quote cards
- [ ] Ensure author / book attribution is visually subordinate (smaller, muted colour) without disappearing

## 2. Smooth loading state

- [ ] Add an Alpine.js `loading` boolean; set it true on submit, false when results arrive
- [ ] Show a skeleton placeholder (2–3 grey bars matching quote card shape) while `loading === true`
- [ ] Use a CSS `opacity` + `transition` fade-in on the result cards so they appear without a jump
- [ ] Verify no layout shift: measure element heights are stable before and after data arrives

## 3. Mobile layout

- [ ] Add `<meta name="viewport" content="width=device-width, initial-scale=1">` if not present
- [ ] Set container `max-width: 680px; margin: 0 auto; padding: 1rem` — works on both 375px and 1440px
- [ ] Ensure the search input + button stack vertically (or go full-width) below 480px
- [ ] Test at 375px, 768px, 1280px — no horizontal scroll, no clipped text

## 4. Weather label on the quote card

- [ ] Read `weather.description` and `weather.temperatureCelsius` from the API response (already returned)
- [ ] Display as a small inline label per card: `"fog · 8 °C"` in muted text, above or below the quote text
- [ ] Style label to be clearly secondary — smaller font, lighter colour, no border

## 5. "Try another quote" button

- [ ] Store all returned quotes in Alpine.js state (`quotes[]`, `currentIndex`)
- [ ] Render only `quotes[currentIndex]`; initialise `currentIndex = 0`
- [ ] Add a "Try another quote →" button below the card; `@click="currentIndex = (currentIndex + 1) % quotes.length"`
- [ ] Hide the button if `quotes.length <= 1`
- [ ] Reset `currentIndex = 0` on each new search
