# syntax=docker/dockerfile:1

## ---- Build stage ----
## No official Flutter image from Google; cirruslabs/flutter is the widely-used,
## actively maintained community image for exactly this (CI/Docker Flutter builds).
FROM ghcr.io/cirruslabs/flutter:stable AS build
WORKDIR /app

# Copy the dependency manifest first so `flutter pub get` is cached independently of source changes.
COPY pubspec.yaml pubspec.lock ./
RUN flutter pub get

COPY . .

# Override at build time: docker build --build-arg API_BASE_URL=https://api.watchlog.example.com
ARG API_BASE_URL=http://localhost:8080
RUN flutter build web --release --dart-define=API_BASE_URL=${API_BASE_URL}

## ---- Runtime stage ----
FROM nginx:1.27-alpine AS runtime
COPY --from=build /app/build/web /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
    CMD wget -qO- http://localhost:80/ >/dev/null || exit 1
