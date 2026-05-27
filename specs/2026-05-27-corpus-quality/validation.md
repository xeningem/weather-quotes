# Validation — Corpus Quality & Growth

## Acceptance criteria

- [ ] All test cases below return zero semantically contradictory quotes (sunny quotes for storms, summer quotes for snow, etc.)
- [ ] At least 3 new authors are represented in results across the test cases (Hardy, Conrad, Chekhov, London, Eliot, or Lawrence)
- [ ] Every returned quote is a complete, readable sentence or passage — no mid-sentence fragments, no 10-line walls of text
- [ ] Qdrant payload for each point includes `era`, `genre`, and `language` fields (verify via `GET /collections/literary_quotes/points/scroll`)

## Manual test cases

| City + condition | Must NOT appear | Should appear |
|---|---|---|
| London, heavy rain, 10°C | "blazing sun", "scorching", "not a cloud" | fog, wet streets, grey skies |
| Moscow, snow, -5°C | "sweltering", "summer heat", "blazing" | cold, winter, frost, white |
| Seville, clear, 32°C | "thunder", "downpour", "blizzard" | heat, sunlight, dry |
| Bergen, thunderstorm, 13°C | "bright sunshine", "cloudless sky" | storm, dark, turbulent |
| Tokyo, fog, 15°C | any clearly contradictory weather | mist, shapes, hushed, grey |

## Regression check

- [ ] `GET /api/quote?location=London` returns a valid response with `era`/`genre`/`language` fields populated
- [ ] Query for Voronezh / Bergen thunderstorm returns dark/stormy content (Wuthering Heights, Bleak House, Conrad, or similar register)
- [ ] Qdrant collection `literary_quotes` loads cleanly — no errors in Qdrant logs (native binary in WSL2, not Docker)
- [ ] All 89 automated tests pass (`dotnet test`)

## Merge condition

This branch is ready to merge when:
1. All five manual test cases pass with zero contradictory quotes
2. At least one new author (post-batch-2) appears in results
3. All 89 automated tests pass
