# Validation — Forecast Support

## Acceptance criteria

- [ ] `GET /api/quote?location=London` (no offset) returns the same response shape as before — no regression
- [ ] `GET /api/quote?location=London&offset=1` returns a valid response with `weather.date` matching tomorrow's date
- [ ] `GET /api/quote?location=London&offset=5` returns 400 Bad Request
- [ ] UI shows four day pills: "Today", "Tomorrow", "+2", "+3"; default is "Today"
- [ ] Clicking "Tomorrow" re-fetches and shows a quote; the weather summary shows tomorrow's date and condition
- [ ] If tomorrow's weather differs from today's (e.g. today sunny, tomorrow rain), the returned quote feels noticeably different in mood
- [ ] Selecting a new city resets offset to "Today"
- [ ] The weather summary line shows the forecast date (day name or short date)

## Manual test cases

| Scenario | Check |
|---|---|
| Open app, default state | "Today" pill active; behaviour identical to phase 4 |
| Query "London", switch to "Tomorrow" | Weather summary updates with tomorrow's date; new quote appears |
| Find a day with rain forecast, compare to a sunny day | Quote mood differs noticeably between the two — the literary match feels right for each |
| Query "London", set offset=2, then type "Moscow" and search | Offset resets to "Today"; Moscow result shown for current weather |
| `GET /api/quote?location=London&offset=3` via curl | Returns 200 with `weather.date` = date 3 days from now |

## Regression check

- [ ] All 97 existing automated tests pass (`dotnet test`)
- [ ] `GET /api/quote?location=London` (no offset) still returns `quotes[]`, `weather`, `weatherProse`, `era`, `genre`, `language`
- [ ] Debug toggles (Prose, Score, Highlights) still work with forecast results
- [ ] No JavaScript console errors with any day selection

## Merge condition

This branch is ready to merge when: switching to "Tomorrow" on a day with genuinely different weather from today returns a quote whose mood is unmistakably suited to that forecast — not just technically correct, but emotionally distinct.
