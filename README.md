# WatchLog

A TV Time-inspired series & movie tracker, being built with an original visual identity, clean
architecture, and a real production-shaped backend. This repo is being built **backend-first**: the
API + database are the deep, working part today; the Flutter app and React admin dashboard are
deliberately not started yet (see [`docs/ROADMAP.md`](docs/ROADMAP.md)) rather than shipped as
half-finished stubs.

## What's here today

```
backend/            ASP.NET Core 9 Web API — Clean Architecture, fully working
  src/
    WatchLog.Domain/          entities & enums, zero dependencies
    WatchLog.Application/     use-case services, DTOs, FluentValidation, repository interfaces
    WatchLog.Infrastructure/  EF Core + PostgreSQL, Identity, Redis, TMDB client, JWT, WebAuthn/passkeys
    WatchLog.Api/             controllers, SignalR hub, middleware, Swagger
  tests/
    WatchLog.Application.Tests/       unit tests (xUnit + FluentAssertions + Moq)
    WatchLog.Api.IntegrationTests/    integration tests (WebApplicationFactory + Testcontainers)
infra/
  docker-compose.yml  postgres + redis + api + nginx
  nginx/nginx.conf
  k8s/                Deployment/Service/Ingress starting point
docs/
  ER-DIAGRAM.md        mermaid schema
  ROADMAP.md           what's deliberately not built yet, and why
.github/workflows/backend-ci.yml
```

## Feature surface implemented in the API

Auth (JWT + refresh tokens, Google/Microsoft/Facebook/Apple OAuth, WebAuthn passkeys) · user
profiles · TMDB-backed movie/series search & detail with Postgres+Redis caching · episode & movie
tracking (watched/skipped/favorite, season progress, "next episode") · the six built-in lists +
custom lists · stats (totals, monthly activity, favorite genres, heatmap calendar, achievement
badges) · notifications (REST + live push over SignalR) · social (follow, activity feed, comments,
likes) · curated & user collections · an AI assistant endpoint with a genuine heuristic
recommendation engine (runtime-budget parsing, "similar to X", genre-affinity from watch history) ·
rate limiting · health checks · full Swagger/OpenAPI docs.

## Getting started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker](https://www.docker.com/) (for Postgres/Redis/nginx, or the whole stack)
- A free [TMDB API key](https://www.themoviedb.org/settings/api) (movie/series data won't load
  without one — every other feature works fine unconfigured)

### Run everything in Docker

```bash
cp .env.example .env   # fill in Tmdb__ApiKey at minimum
docker compose --env-file .env -f infra/docker-compose.yml up --build
```

- API: http://localhost:8080/swagger
- Health: http://localhost:8080/health

### Run the API locally against dockerized Postgres/Redis

```bash
cp .env.example .env
docker compose --env-file .env -f infra/docker-compose.yml up -d postgres redis

cd backend
dotnet tool restore 2>/dev/null || dotnet tool install --global dotnet-ef
dotnet ef database update --project src/WatchLog.Infrastructure --startup-project src/WatchLog.Api
dotnet run --project src/WatchLog.Api
```

### Run the tests

```bash
cd backend
dotnet test WatchLog.slnx
```

Integration tests spin up real Postgres/Redis containers via Testcontainers automatically — no
manual setup needed, just a running Docker daemon.

### Adding a new migration after model changes

```bash
cd backend
dotnet ef migrations add <Name> --project src/WatchLog.Infrastructure --startup-project src/WatchLog.Api
```

## Architecture notes

- **Clean Architecture + Repository pattern**: `Domain` has zero dependencies; `Application` depends
  only on `Domain` and defines every external seam (`IUnitOfWork`, `ITmdbClient`, `ITokenService`,
  `ICacheService`, ...) as an interface; `Infrastructure` implements those seams; `Api` is thin
  controllers over `Application` services. This is what makes `Application` unit-testable with mocks
  and keeps persistence/TMDB/Redis/JWT concerns swappable.
- **TMDB integration** is cache-aside: `ICatalogService` checks Postgres/Redis first, falls back to
  `ITmdbClient`, and upserts what it fetches — so the same movie/series never gets re-fetched from
  TMDB more than once per cache TTL.
- **Auth** uses ASP.NET Core Identity for the user store, custom JWT access + rotating refresh
  tokens for API auth, the real ASP.NET OAuth handlers for Google/Microsoft/Facebook/Apple
  (config-driven — a provider is simply not registered if its `ClientId` is blank), and
  Fido2NetLib for real WebAuthn passkey ceremonies.
- **AI Assistant** (`IAiAssistantService`) is a genuine heuristic today (see feature list above),
  built behind an interface specifically so a real LLM-backed implementation is a drop-in swap later
  — see `docs/ROADMAP.md` phase 5.

## Roadmap

See [`docs/ROADMAP.md`](docs/ROADMAP.md) for the Flutter app, React admin dashboard, CI/CD +
Kubernetes hardening, and real-AI phases that come next.
