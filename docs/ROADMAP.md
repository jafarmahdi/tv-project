# WatchLog — Roadmap

This tracks scope the [build plan](../README.md) deliberately deferred so the backend got real
depth instead of every layer getting a shallow stub. Nothing here is started yet.

## Phase 2 — Flutter app (iOS / Android / Web / Windows / macOS / Linux)

- Project scaffold: Flutter workspace targeting all six platforms, Material 3, adaptive layouts.
- Theming: dynamic color, dark/light, glassmorphism + soft-gradient design language (Apple TV /
  Netflix / Letterboxd / Spotify inspired — not a TV Time clone).
- Routing/state: `go_router` + a state layer (Riverpod or Bloc), a generated Dio client from the
  API's OpenAPI/Swagger doc (`/swagger/v1/swagger.json`) so the client never hand-drifts from the API.
- Localization: `ar`/`en`, full RTL support.
- Screens: Splash, Login, Register, Home, Discover, Search, Details, Episode Page, Statistics,
  Profile, Notifications, Settings, Premium, AI Assistant.
- Offline mode: local cache (Drift/Isar) for watched data + optimistic offline edits with background sync.
- Push notifications: FCM (Android/Web) + APNs (iOS/macOS) wired to the `devices` table already in
  the schema — `IDeviceService`/`DevicesController` are ready to receive tokens today.

## Phase 3 — Admin dashboard (React + TailwindCSS)

- Auth against the same JWT API (an `Admin` role already exists in the Identity schema — needs a
  role-seeding step and `[Authorize(Roles = "Admin")]` guards on moderation endpoints).
- Content moderation: comments/reports queue, user management, achievement/collection curation.
- Analytics: platform-wide stats dashboards (reuse `IStatsService`'s aggregation patterns, scoped
  to all users instead of one).

## Phase 4 — Infra & CI/CD hardening

- GitHub Actions: add jobs for the Flutter app (build + test per platform) and the admin dashboard
  (lint + build), alongside the existing `backend-ci.yml`.
- Container registry push + image tagging on tag/release, wired into the `infra/k8s` manifests
  (currently a hand-editable starting point, not a Helm/Kustomize chart).
- Production TLS: real certs (cert-manager/ACME) for the nginx reverse proxy and k8s Ingress.
- Observability: structured logging sink, tracing (OpenTelemetry), and alerting.

## Phase 5 — Real AI backend

- `IAiAssistantService` currently ships a genuine (non-fake) heuristic implementation — runtime
  parsing, "similar to X" TMDB lookups, genre-affinity from watch history. Swap in an LLM-backed
  implementation behind the same interface for genuinely open-ended prompts ("less confusing than
  Dark") without touching `AiController` or the Flutter client.
- Expand `Recommendations`/`AiHistory` into a proper feedback loop (thumbs up/down feeding back into
  ranking).

## Smaller follow-ups noted in the backend itself

- Admin moderation endpoints beyond what's here (ban/suspend, content takedown) — same
  controller/service pattern as everything else, just not built yet.
- GDPR data export/delete endpoints (the schema and cascade-delete FKs are already in place for a
  "delete my account" flow; the endpoint itself isn't wired up).
- Rate limiting policy is a single global + auth-specific policy today; consider per-endpoint tuning
  once real traffic patterns exist.
