# Plan — Forecast Support

## 1. Extend WeatherService with forecast

- [ ] Read Open-Meteo `/forecast` API docs; confirm `daily` array fields: `weathercode`, `temperature_2m_max`, `temperature_2m_min`, `windspeed_10m_max`, `precipitation_sum`, `relativehumidity_2m_max`
- [ ] Add `GetForecastWeatherAsync(string location, int offsetDays)` to `IWeatherService` (or extend `GetCurrentWeatherAsync` with an optional offset parameter — prefer new overload to avoid breaking the interface)
- [ ] Implement: geocode city → lat/lon (reuse existing logic), call `/forecast` with `daily=...&forecast_days=<offset+1>`, extract day at index `offsetDays`, map WMO code → condition/description via existing `WmoCode` helper
- [ ] Return `WeatherData` with an added `Date` property (the forecast date as `DateOnly`) so the UI can display "Wednesday" or "Thu 29 May"
- [ ] Unit-test the new method with a fake `HttpMessageHandler` returning a sample forecast JSON

## 2. Extend the API endpoint

- [ ] Add optional `int offset = 0` parameter to `GET /api/quote`; validate: must be 0–3, return 400 otherwise
- [ ] Route to `GetForecastWeatherAsync` when `offset > 0`, existing `GetCurrentWeatherAsync` when `offset == 0` (or unify behind one method)
- [ ] Update E2E test stubs: `IWeatherService` stub in `QuoteEndpointTests` needs to handle the new signature; existing tests must still pass
- [ ] Add E2E test: `GET /api/quote?location=London&offset=2` returns 200 with valid `weather` + `quotes`
- [ ] Add E2E test: `GET /api/quote?location=London&offset=5` returns 400

## 3. UI day selector

- [ ] Add `offset` Alpine data property (default `0`); add `selectedDay` computed label ("Today", "Tomorrow", "+2 days", "+3 days")
- [ ] Add a day-selector row above the search button (or below it): four pill buttons, each sets `offset` and calls `searchSelected()` if a result is already showing
- [ ] Pass `offset` as query param in `fetch('/api/quote?location=...&offset=...')`
- [ ] Show the forecast date in the weather summary line (e.g. "London · Thu 29 May · 🌧️ light rain · 14°C")
- [ ] Reset `offset` to 0 when the user types a new city (alongside existing toggle reset)
- [ ] Style: pill row matches debug-toggle visual weight; active pill highlighted; row hidden until a city is entered (or always visible — simpler)
