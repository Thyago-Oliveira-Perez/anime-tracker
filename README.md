# Anime Tracker

Personal anime tracking system: search anime via the AniList public API, log what you've
watched, and keep your own rating/review for each entry. Self-hosted on a home server via
Docker Compose.

## Stack

- **Backend**: ASP.NET Core Web API (.NET 10) + EF Core + PostgreSQL
- **Frontend**: Next.js (App Router) + next-intl (en / pt-BR / ja) + Tailwind
- **External API**: [AniList GraphQL API](https://anilist.gitbook.io/anilist-apiv2-docs/)
- **Tooling**: [mise](https://mise.jdx.dev/) pins the .NET and Node versions (see `.mise.toml`)

## Project layout

```
anime-tracker/
  backend/    ASP.NET Core Web API
  frontend/   Next.js app
  docker-compose.yml
```

## Getting started (local dev)

```bash
mise install       # installs the pinned dotnet/node versions
cd backend && dotnet run --project src/AnimeTracker.Api
```

Or with Docker Compose (db + backend):

```bash
cp .env.example .env    # then edit POSTGRES_PASSWORD etc.
docker compose up -d --build
```

The API listens on `http://localhost:8080`. EF Core migrations run automatically on startup.

Frontend setup instructions will be added once it's scaffolded.

## API overview

- `GET  /api/anime/search?q=...` — search AniList (not persisted)
- `GET  /api/anime/{aniListId}` — anime details from AniList
- `GET  /api/watch-entries?status=&favorite=` — list your personal log
- `GET  /api/watch-entries/{id}`
- `POST /api/watch-entries` — add an anime to your log (caches it from AniList on first use)
- `PUT  /api/watch-entries/{id}`
- `DELETE /api/watch-entries/{id}`

## Status

Backend done: domain model, EF Core + PostgreSQL, AniList GraphQL client, full CRUD API,
Docker Compose (db + backend), verified end to end. Frontend not started yet.
