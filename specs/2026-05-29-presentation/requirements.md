# Phase 6 — Presentation & Description

Weather Quotes is a finished product with a strong concept and clean execution.
This phase makes it legible to the outside world — both for a technical viewer browsing GitHub
and a non-technical person who receives a live link. It does not add new features;
it makes the existing ones discoverable and understandable.

## In scope

- **README** — public-facing repository description: what the project does, how it works,
  architecture overview, screenshots, quick-start instructions
- **Architecture diagram** — Mermaid diagram embedded in README (renders natively on GitHub);
  covers the data flow from weather API → prose → embedding → Qdrant → quote
- **About / How it works** — collapsible section in the UI explaining the pipeline
  in plain language (no jargon), visible without leaving the app
- **Demo mode in UI** — a set of pre-baked example prompts (city + weather scenario)
  that a visitor can click to immediately see a result without typing anything

## Out of scope / deferred

- Public deployment (Fly.io, Railway, etc.) — deferred; infrastructure decisions not made yet
- Video walkthrough / screen recording
- Localization of the UI

## Decisions

- **Diagram format**: Mermaid, embedded in README. GitHub renders it natively as SVG;
  no external tool needed to export a PNG.
- **About section**: collapsible `<details>` element in the existing Alpine.js page,
  shown below the result area. No new page, no routing.
- **Demo prompts**: a small hardcoded array of `{ label, city, scenario }` objects in the
  frontend. Clicking one fills the city input (or custom-weather textarea) and triggers search.
  Mix of real city + "Custom weather" prose examples to showcase both modes.
- **Audience balance**: README leads with the concept and a screenshot (non-technical hook),
  then explains the architecture (technical depth). About section in UI stays concept-first.

## Constraints

- No build step in the frontend — Mermaid must be rendered by GitHub, not by the app
- Alpine.js only; no additional JS libraries for the About section
- README must work without the app running (screenshots, not live embeds)
