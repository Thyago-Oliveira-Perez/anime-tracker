# Anime Tracker

Personal anime tracking system: search anime via a pluggable public API, log what you've
watched, and keep your own rating/review for each entry. Self-hosted on a home server via
Docker Compose.

## Stack

- **Backend**: ASP.NET Core Web API (.NET 10) + EF Core + PostgreSQL
- **Frontend**: React + Vite + TypeScript + Tailwind CSS v4, `react-router` for routing,
  `react-i18next` for i18n — served via nginx in Docker
- **Anime data sources** (pluggable, see below): [AniList](https://anilist.gitbook.io/anilist-apiv2-docs/) (GraphQL), [Jikan](https://docs.api.jikan.moe/) (MyAnimeList REST)
- **Tooling**: [mise](https://mise.jdx.dev/) pins the .NET and Node versions (see `.mise.toml`)

## Project layout

```
anime-tracker/
  backend/
    src/AnimeTracker.Api/    ASP.NET Core Web API
    tests/AnimeTracker.Api.Tests/
  frontend/
    src/
      i18n/       react-i18next setup + en/pt-BR/ja locale files
      routes/     one file per page (SearchPage, MyListPage, SettingsPage, LocaleLayout)
      components/ shared UI (Nav, LocaleSwitcher, Modal, WatchEntryForm, ...)
      lib/        api.ts (backend client), animeTitle.ts, watchEntryForm.ts
  docker-compose.yml
```

## Pluggable anime providers

Search and lookups go through an `IAnimeProvider` strategy interface instead of talking to
AniList directly — each source (AniList, Jikan) is an adapter implementing it, collected in
an `AnimeProviderRegistry`. Which one is "active" is a runtime feature flag persisted in the
DB (`GET/PUT /api/settings/anime-provider`), not a redeploy — useful since either public API
can go down independently (both were down during development, which is exactly the scenario
this exists for).

Adding a third source later means: a raw client + a provider adapter + one DI registration —
no changes to controllers or the settings UI.

## i18n

Locale-prefixed routing: `/en/...`, `/pt-br/...`, `/ja/...`. `LocaleLayout` validates the
`:localeSegment` route param, syncs `i18next`'s active language to it, and renders the shared
nav + page. `/` redirects to the browser's preferred supported locale (`detectPreferredLocale`
in `src/i18n/locales.ts`), falling back to English. Anime titles pick the right variant per
locale (English/romaji for en & pt-BR, native script for ja) via `localizedTitle`, using the
title fields AniList/Jikan already return — no translation service involved.

All code (identifiers, comments) stays in English regardless of UI locale — only
`src/i18n/locales/*.json` and content you type (reviews) are localized/free-language.

## Getting started (local dev)

```bash
mise install       # installs the pinned dotnet/node versions
cd backend && dotnet run --project src/AnimeTracker.Api
```

Or with Docker Compose (full stack — db + backend + frontend):

```bash
cp .env.example .env    # then edit POSTGRES_PASSWORD etc.
docker compose up -d --build
```

- Backend: `http://localhost:8080` (EF Core migrations run automatically on startup)
- Frontend: `http://localhost:3000` (nginx serving the Vite build)

Frontend local dev without Docker: `cd frontend && cp .env.local.example .env.local && npm run dev` (Vite dev server, default port 5173).

Backend tests: `cd backend/tests/AnimeTracker.Api.Tests && dotnet test`.

## API overview

- `GET  /api/anime/search?q=...` — search the *active* provider (not persisted)
- `GET  /api/anime/{provider}/{externalId}` — anime details from a specific provider
- `GET  /api/watch-entries?status=&favorite=` — list your personal log
- `GET  /api/watch-entries/{id}`
- `POST /api/watch-entries` — add an anime to your log by `{ provider, externalId, ... }` (caches it on first use)
- `PUT  /api/watch-entries/{id}`
- `DELETE /api/watch-entries/{id}`
- `GET  /api/settings/anime-provider` — `{ active, available }`
- `PUT  /api/settings/anime-provider` — `{ provider }`, switches the active source

## Status

Backend done: domain model (multi-provider aware), EF Core + PostgreSQL, AniList + Jikan
providers behind a feature flag, full CRUD API, unit tests for the mapping logic, Docker
Compose (db + backend + frontend), verified end to end.

Frontend has all core screens: Search (with add-to-list modal), My List (with edit/delete +
status/favorite filters), Settings (anime provider switcher), shared nav + locale switcher,
i18n in en/pt-BR/ja. The request/response contract with the backend was verified end to end
(seeded a cached anime directly in Postgres and exercised create/list/update/filter/delete
with the exact payload shapes the UI sends) since both AniList and Jikan happened to be down
during development — actual search-driven add-to-list still needs a live check once either
recovers. Not built yet: anime detail page, stats/dashboard, auth (still intentionally
single-user/no-login per the original plan).
