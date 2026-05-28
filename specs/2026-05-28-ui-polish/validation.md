# Validation — UI/UX Polish

## Acceptance criteria

- [ ] Open on a real phone (or DevTools 375px) — no horizontal scroll, text readable without zooming
- [ ] Submit a city — a skeleton/placeholder appears immediately, results fade in without layout shift
- [ ] Quote text reads comfortably: serif font, ≥ 1.2rem, ≥ 1.7 line-height, ≤ 65ch width
- [ ] Each result shows a weather label (e.g. "partly cloudy · 16 °C") in muted secondary style
- [ ] "Try another quote" cycles to the next result without a network request; disappears if only one result
- [ ] The overall experience feels finished, not like a prototype — the subjective bar

## Manual test cases

| Scenario | Check |
|---|---|
| Desktop 1280px, type "London", submit | Results appear with fade-in; quote is readable; weather label visible; "Try another" cycles all 3 quotes |
| Mobile 375px (DevTools), same query | Single-column layout, no horizontal scroll, input full-width, button tappable |
| Slow network (DevTools throttle to Slow 3G) | Skeleton visible for >1s; no layout jump when results arrive |
| City with one result | "Try another" button hidden |
| Submit a second city after first results | `currentIndex` resets to 0, new weather label matches new city |

## Regression check

- [ ] `GET /api/quote?location=London` still returns valid JSON with `quotes[]`, `weather`, `era`, `genre`, `language`
- [ ] All 89 automated tests pass (`dotnet test`)
- [ ] No JavaScript console errors on load or on submit

## Merge condition

This branch is ready to merge when: opening the app on a phone and on a desktop both feel like a finished product — the quote is the centrepiece, the weather label is clear, and cycling through quotes works without friction.
