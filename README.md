# Anime Tracker

Personal anime tracking system: search anime via a pluggable public API, log what you've
watched, and keep your own rating/review for each entry. Self-hosted on a home server via
Docker Compose.

## Stack

- **Backend**: ASP.NET Core Web API (.NET 10) + EF Core + PostgreSQL
- **Frontend**: Next.js (App Router) + next-intl (en / pt-BR / ja) + Tailwind
- **Anime data sources** (pluggable, see below): [AniList](https://anilist.gitbook.io/anilist-apiv2-docs/) (GraphQL), [Jikan](https://docs.api.jikan.moe/) (MyAnimeList REST)
- **Tooling**: [mise](https://mise.jdx.dev/) pins the .NET and Node versions (see `.mise.toml`)

## Project layout

```
anime-tracker/
  backend/
    src/AnimeTracker.Api/    ASP.NET Core Web API
    tests/AnimeTracker.Api.Tests/
  frontend/   Next.js app
  docker-compose.yml
```

## Pluggable anime providers

Search and lookups go through an `IAnimeProvider` strategy interface instead of talking to
AniList directly — each source (AniList, Jikan) is an adapter implementing it, collected in
an `AnimeProviderRegistry`. Which one is "active" is a runtime feature flag persisted in the
DB (`GET/PUT /api/settings/anime-provider`), not a redeploy — useful since either public API
can go down independently. The frontend's only page right now is the switcher for this flag.

Adding a third source later means: a raw client + a provider adapter + one DI registration —
no changes to controllers or the settings UI.

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
- Frontend: `http://localhost:3000` (currently just the anime-provider settings page)

Frontend local dev without Docker: `cd frontend && cp .env.local.example .env.local && npm run dev`.

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
Compose (db + backend + frontend), verified end to end. Frontend has only the provider
settings page so far — search, watch log UI and i18n come next.
