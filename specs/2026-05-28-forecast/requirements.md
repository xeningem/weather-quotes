# Requirements — Forecast Support

Phase 5 extends Weather Quotes from a present-moment tool to one that lets you look ahead. Instead of only matching the weather outside your window right now, you can ask: what will the weather feel like tomorrow — and what passage from literature captures that mood? This keeps the core value intact (a moment of recognition between the world and literature) while adding temporal depth: you can prepare emotionally for rain on Thursday, or feel anticipation for a clear weekend.

## In scope

- Extend `WeatherService` to fetch a daily forecast from Open-Meteo's `/forecast` endpoint (supports up to 16 days ahead); extract the representative condition, temperature, humidity, wind for a given day offset
- Add `offset` parameter to `GET /api/quote?location=...&offset=0|1|2|3` (0 = today, 1 = tomorrow, etc.); defaults to 0 so existing behaviour is unchanged
- Add a day-selector row to the UI: pill buttons "Today", "Tomorrow", "+2", "+3" binding to `offset`; selecting a day re-runs the search automatically

## Out of scope / deferred

- Offsets beyond +3 (UI becomes unwieldy; forecast accuracy drops)
- ISO date format in the API (timezone complexity not worth it for a personal tool)
- Hourly forecast granularity (daily representative values are sufficient for mood matching)
- Caching forecast responses (out of scope for a local personal tool)

## Decisions

- **`offset` as integer day count (not ISO date)** — simpler API contract, no timezone logic needed in the browser; the server uses the offset to index into Open-Meteo's `daily` array
- **Forecast uses daily aggregate values** — Open-Meteo daily API returns `temperature_2m_max`, `weathercode` (WMO), `windspeed_10m_max`, `precipitation_sum`; we derive condition and description from WMO code exactly as the current weather path does
- **Default offset = 0** — existing API calls without `offset` continue to return current weather; no breaking change

## Constraints

- Open-Meteo `/forecast` endpoint requires lat/lon (not city name); the existing `WeatherService` already geocodes city → lat/lon via Open-Meteo Geocoding API, so this lookup can be reused
- `WeatherData` record is shared; forecast must return the same shape so `QuoteEndpoints` and `ToNaturalLanguage()` need no changes
- No API key required (Open-Meteo is free)
- `.NET 10`, Alpine.js, no build step
