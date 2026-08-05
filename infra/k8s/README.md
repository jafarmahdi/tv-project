# WatchLog Kubernetes

The backend image published by this repo is:

```text
ghcr.io/jafarmahdi/tv-project-api
```

## Publish the image

Push to `main`, or run `.github/workflows/backend-ci.yml` manually on `main`.

That workflow builds and tests the backend, then publishes:

- `ghcr.io/jafarmahdi/tv-project-api:latest`
- `ghcr.io/jafarmahdi/tv-project-api:sha-<commit>`

## Create runtime secrets

Create the application secrets before deploying:

```bash
kubectl create secret generic watchlog-api-secrets -n watchlog \
  --from-literal=ConnectionStrings__Default='Host=...;Database=...;Username=...;Password=...' \
  --from-literal=ConnectionStrings__Redis='...' \
  --from-literal=Jwt__SigningKey='...' \
  --from-literal=Tmdb__ApiKey='...'
```

If the GHCR package is private, also create an image pull secret:

```bash
kubectl create secret docker-registry ghcr-creds -n watchlog \
  --docker-server=ghcr.io \
  --docker-username='<github-username>' \
  --docker-password='<github-pat>'
```

Then uncomment `imagePullSecrets` in `api-deployment.yaml`, or patch the deployment with the same
secret name.

## Apply order

```bash
kubectl apply -f infra/k8s/namespace.yaml
kubectl apply -f infra/k8s/api-configmap.yaml
kubectl apply -f infra/k8s/api-deployment.yaml
kubectl apply -f infra/k8s/api-ingress.yaml
```

If you manage the app secret as YAML instead of creating it out-of-band, apply that before the
deployment.
