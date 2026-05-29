# Validation — Phase 6: Presentation & Description

## Acceptance criteria

- [ ] README is present at repo root and renders correctly on GitHub (no broken images, diagram visible as SVG)
- [ ] At least two screenshots embedded in README (main UI + custom weather mode)
- [ ] Mermaid diagram shows the full request pipeline end-to-end
- [ ] "How it works" section is present in the UI and collapsed by default
- [ ] Demo examples row appears on the start screen and triggers a search on click
- [ ] All existing functionality (city search, forecast, debug toggles) still works after UI changes

## Manual test cases

1. **README cold read** — open the GitHub repo page without knowing the project;
   within 30 seconds understand what it does, what tech it uses, and how to run it locally.

2. **Diagram completeness** — trace a request in the Mermaid diagram from "user types city"
   to "quote appears"; every hop (weather API, prose, Ollama, Qdrant) must be visible.

3. **Demo click** — on a fresh page load, click a demo pill without typing anything;
   a quote card must appear within the normal loading time.

4. **About section** — click "How it works" summary; content expands; click again, collapses.
   Content should be readable by a non-developer (no class names, no API jargon unexplained).

5. **Regression — forecast** — after UI changes, select "+2 days" for any city;
   confirm the day selector still works and the date label appears in the weather summary.

## Regression check

- `dotnet test` passes with no failures after any backend changes (there should be none this phase)
- City autocomplete, suggestion keyboard navigation, and "Try another quote" button still work

## Merge condition

This branch is ready to merge when the README renders fully on GitHub with screenshots and diagram,
and a non-technical person shown the live app understands its purpose without explanation.
