# WatchLog — Roadmap

This tracks scope deliberately deferred so each layer got real depth instead of everything getting
a shallow stub. Backend (phase 1) and the Flutter **web** client (phase 2) are done; what's below is
what's left, and what's explicitly known to be missing from what already shipped.

## Product feedback backlog — 2026-08-07

Initial response shipped on Friday, August 7, 2026:

- Comments + ratings are now wired into movie, series, and episode surfaces.
- Profile got a richer layout and visible achievement badges.
- In-app admin tools now exist for direct TMDB imports plus bulk year imports.
- Android/iOS project scaffolds were added, and native token storage now uses secure storage.

Still visible after that first pass:

- **UI/UX overhaul**: the current web client works, but it needs a deliberate product-design pass
  for hierarchy, spacing, affordances, empty/loading states, and overall polish.
- **Profile iteration**: the profile is materially better, but still needs another polish pass to
  feel flagship-quality.
- **Admin operations UX**: the Flutter app now has practical admin import tools, but the full
  moderation/dashboard experience is still pending.
- **Native mobile production readiness**: Android/iOS are scaffolded, but device QA, SDK setup,
  signing, and store-grade release work remain.

## Phase 2 follow-ups — Flutter app hardening

The web client is real and working (`app/`) — Material 3 theme, `go_router` + Riverpod, a
hand-written Dio client mirroring every backend controller, JWT auto-refresh, ar/en chrome
localization with RTL, and the core screens: Splash, Login, Register, Home, Discover/Search,
Details (movie + series), Episode Page (season tracking), Statistics, Profile, Notifications,
Settings, AI Assistant. Known gaps, in roughly the order they'd bite:

- **UI/UX polish pass**: revisit navigation clarity, content density, CTA prominence, empty/error
  states, and mobile-sized responsive behavior so the app feels intentional instead of merely
  functional.
- **Native targets**: Android/iOS scaffolds now exist and auth token storage is native-safe via
  `flutter_secure_storage`; the remaining work is build-environment setup, signing, device QA, push
  notification registration, and any desktop targets beyond that.
- **Social/activity feed screen**: the backend's `SocialController` (follow, activity feed,
  comments, likes) has no client screen yet — `Profile` only surfaces the user's own lists.
- **Collections screen**: `CollectionsController` (curated Marvel/DC/Oscar-winners-style lists)
  isn't surfaced in the client.
- **Premium screen**: skipped — there's no subscription/payment feature anywhere in the backend
  yet, so a Premium screen would be UI with nothing behind it.
- **Offline mode**: no local cache (Drift/Isar) or optimistic offline edits yet — every screen is
  online-only today.
- **Push notifications**: in-app notifications (REST + live SignalR push) work; FCM/APNs device
  registration doesn't — `IDeviceService`/`DevicesController` exist and are ready to receive tokens,
  nothing calls them yet.
- **Content localization**: the `ar`/`en` toggle covers the app's chrome (nav, auth, settings,
  buttons) but not movie/series data — TMDB is always queried in English since the backend's
  `ITmdbClient` doesn't forward a `language` param yet.
- **Generated API client**: the Dio client + models are hand-written to mirror the backend DTOs
  (documented per-file) rather than generated from `/swagger/v1/swagger.json` — fine at this size,
  worth revisiting if the API surface keeps growing to avoid manual drift.

## Phase 3 — Admin dashboard (React + TailwindCSS)

- Auth against the same JWT API. The project already has `Admin:InitialAdminEmail` bootstrap and a
  pragmatic in-app admin tools screen in Flutter; this phase is about a dedicated dashboard, better
  permissions UX, and a more complete operational surface.
- Catalog curation tools beyond the current import-focused admin screen: richer review/edit flows
  for films, series, and episodes without touching the database manually.
- Content moderation: comments/reports queue, user management, achievement/collection curation.
- Analytics: platform-wide stats dashboards (reuse `IStatsService`'s aggregation patterns, scoped
  to all users instead of one).

## Phase 4 — Infra & CI/CD hardening

- `backend-ci.yml` and `frontend-ci.yml` both build/test/publish to GHCR on push to `main`; the
  admin dashboard (phase 3) still needs its own CI job once it exists.
- Extend the existing GHCR publish flows with environment-aware tags and release promotion, and
  automate rollout to the `infra/k8s` manifests (still a hand-editable starting point per
  service — `api-*`/`web-*` — not a Helm/Kustomize chart).
- TLS today assumes either a real issuer (cert-manager + Let's Encrypt) or an internal cluster CA
  for a private domain (`*.watchlog.lab` via `/etc/hosts`) — whichever applies, the cert-manager
  `ClusterIssuer` name in `api-ingress.yaml`/`web-ingress.yaml` needs to match what's actually
  running in the target cluster.
- Observability: structured logging sink, tracing (OpenTelemetry), and alerting.

## Phase 5 — Real AI backend

- `IAiAssistantService` currently ships a genuine (non-fake) heuristic implementation — runtime
  parsing, "similar to X" TMDB lookups, genre-affinity from watch history. Swap in an LLM-backed
  implementation behind the same interface for genuinely open-ended prompts ("less confusing than
  Dark") without touching `AiController` or the Flutter client — the client's `AiAssistantScreen`
  already talks to the real `/api/v1/ai/assistant/ask` endpoint, so a better backend is a drop-in.
- Expand `Recommendations`/`AiHistory` into a proper feedback loop (thumbs up/down feeding back into
  ranking).

## Smaller follow-ups noted in the backend itself

- Admin moderation endpoints beyond what's here (ban/suspend, content takedown) — same
  controller/service pattern as everything else, just not built yet.
- GDPR data export/delete endpoints (the schema and cascade-delete FKs are already in place for a
  "delete my account" flow; the endpoint itself isn't wired up).
- Rate limiting policy is a single global + auth-specific policy today; consider per-endpoint tuning
  once real traffic patterns exist.
