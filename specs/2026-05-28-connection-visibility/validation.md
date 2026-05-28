# Validation — Connection Visibility

## Acceptance criteria

- [ ] `GET /api/quote?location=London` returns `weatherProse` as a non-empty string in the JSON response
- [ ] With "Score" toggle on: a `84% match` (or similar) label appears next to the attribution; it disappears when toggled off
- [ ] With "Highlights" toggle on: at least one word in the quote text is visually highlighted; the highlight is a warm tint, not distracting
- [ ] With "Prose" toggle on: the weather prose block appears below the card; its text matches the `weatherProse` field from the API
- [ ] All three toggles default to off — the default experience is identical to phase 3
- [ ] Toggles reset to off when a new city is searched
- [ ] Subjective bar: opening the app on London rain → turning on all three → it is immediately clear why the Dickens passage was chosen

## Manual test cases

| Scenario | Check |
|---|---|
| Query "London", all toggles off | UI looks exactly like phase 3 — no extra elements visible |
| Query "Moscow" (rain/snow), enable Prose | Prose text describes cold/wet weather; matches the mood of the returned quote |
| Query "Seville" (clear, hot), enable Score | Score ≥ 60% for top result; lower for the others |
| Enable Highlights on any result | At least one weather word (rain, grey, cold, fog, etc.) appears highlighted in the quote |
| Search new city while toggles are on | Toggles reset to off; new result shown clean |

## Regression check

- [ ] All 89 existing automated tests pass (`dotnet test`)
- [ ] New E2E test for `weatherProse` passes
- [ ] `GET /api/quote?location=London` still returns `quotes[]`, `weather`, `era`, `genre`, `language` — no fields removed
- [ ] No JavaScript console errors with any combination of toggle states

## Merge condition

This branch is ready to merge when: enabling all three debug toggles on any result makes the weather-to-quote connection immediately legible without needing an explanation.
