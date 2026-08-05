# WatchLog Kubernetes

Two images are published from this repo:

```text
ghcr.io/jafarmahdi/tv-project-api   # backend/  — ASP.NET Core API
ghcr.io/jafarmahdi/tv-project-web   # app/      — Flutter web frontend
```

## Publish the images

Push to `main` (or run the workflow manually):

- `.github/workflows/backend-ci.yml` builds, tests, and publishes `tv-project-api`.
- `.github/workflows/frontend-ci.yml` builds, tests (`flutter analyze` + `flutter test`), and
  publishes `tv-project-web`. It bakes in `API_BASE_URL=https://api.watchlog.lab` at build time
  (Flutter web config is compiled in, not read from env at runtime) — change the `build-args` in
  that workflow if your API's real hostname differs.

Both tag `:latest` and `:sha-<commit>` on every push to `main`.

## Create runtime secrets (API only — the web image has none)

```bash
kubectl create secret generic watchlog-api-secrets -n watchlog \
  --from-literal=ConnectionStrings__Default='Host=...;Database=...;Username=...;Password=...' \
  --from-literal=ConnectionStrings__Redis='...' \
  --from-literal=Jwt__SigningKey='...' \
  --from-literal=Tmdb__ApiKey='...'
```

If either GHCR package is private, also create an image pull secret and reference it from the
relevant deployment (`imagePullSecrets` is commented out in both `api-deployment.yaml` and
`web-deployment.yaml`):

```bash
kubectl create secret docker-registry ghcr-creds -n watchlog \
  --docker-server=ghcr.io \
  --docker-username='<github-username>' \
  --docker-password='<github-pat>'
```

## Apply order

```bash
kubectl apply -f infra/k8s/namespace.yaml
kubectl apply -f infra/k8s/api-configmap.yaml
kubectl apply -f infra/k8s/api-deployment.yaml
kubectl apply -f infra/k8s/api-ingress.yaml
kubectl apply -f infra/k8s/web-deployment.yaml
kubectl apply -f infra/k8s/web-ingress.yaml
```

If you manage the app secret as YAML instead of creating it out-of-band, apply that before
`api-deployment.yaml`.

## Ingress hosts

Both ingress manifests assume a private `*.watchlog.lab` domain resolved via `/etc/hosts` (no
public DNS) and TLS from your own cluster's internal CA issuer — `api.watchlog.lab` for the API,
`watchlog.lab` for the web app. Adjust the `host`/`cert-manager.io/cluster-issuer` values in both
files if your setup differs (public domain + Let's Encrypt, a different internal issuer name, etc.).

## Verify a rollout

```bash
kubectl -n watchlog rollout status deployment/watchlog-api
kubectl -n watchlog rollout status deployment/watchlog-web
kubectl -n watchlog get pods
```
