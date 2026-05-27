---
description: Start a feature spec for the next roadmap phase — creates branch, asks 3 questions, writes plan/requirements/validation docs
---

# feature-spec

Kick off structured spec work for the next roadmap phase.

## Steps

### 1. Read context

Read all three of these files before doing anything else:
- `specs/roadmap.md` — find the first phase that has unchecked items (`- [ ]`)
- `specs/mission.md` — understand the product's purpose and constraints
- `specs/tech-stack.md` — understand the technical context

### 2. Create a git branch

Name it after the phase, e.g. `phase-2-corpus-quality`. Create and check out:

```bash
git checkout -b phase-N-short-name
```

### 3. Ask the user — BEFORE writing any files

You MUST call `AskUserQuestion` with exactly these three questions grouped in a single call:

**Question 1 — Scope** (header: "Scope")
Ask which items from the phase the user wants to include, and whether anything should be excluded or deferred. List the roadmap items as options.

**Question 2 — Key decisions** (header: "Decisions")
Ask about 2–3 open design/approach questions that are specific to this phase. Base them on what you read in mission.md and tech-stack.md. Keep options concrete (e.g. "manual curation vs. automated scoring").

**Question 3 — Validation** (header: "Done when")
Ask how the user will know the feature is complete and ready to merge — e.g. specific cities that should return better results, a qualitative bar, a metric.

Do NOT write anything to disk until you have the user's answers.

### 4. Create the spec directory

Use today's date and a slug derived from the phase name:

```
specs/YYYY-MM-DD-feature-name/
```

### 5. Write the three documents

#### `requirements.md`
- One-paragraph summary of what this phase delivers and why (from mission.md context)
- **In scope** — bulleted list from the user's answer to Q1
- **Out of scope / deferred** — anything explicitly excluded
- **Decisions** — the choices made in Q2, stated as facts ("We will X because Y")
- **Constraints** — anything from tech-stack.md or mission.md that bounds the work

#### `plan.md`
A numbered list of task groups. Each group has:
- A bold header (e.g. **1. Extend GutenbergLoader**)
- 3–6 concrete sub-tasks as checkboxes
- Groups should be independently reviewable (one PR each, if possible)

Base the tasks on the roadmap items and the scope confirmed in Q1. Keep it implementation-oriented, not vague.

#### `validation.md`
- **Acceptance criteria** — derived from the user's Q3 answer, written as checkboxes
- **Manual test cases** — 3–5 specific test scenarios (e.g. "Query 'London' in winter rain → no summer quotes")
- **Regression check** — confirm existing behaviour still works after the changes
- **Merge condition** — one sentence: "This branch is ready to merge when…"

### 6. Report back

Tell the user:
- The branch name
- The directory created
- One-line summary of each file written
