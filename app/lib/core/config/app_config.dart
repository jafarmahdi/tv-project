/// Build-time configuration. Override at build time with:
///   flutter build web --dart-define=API_BASE_URL=https://api.watchlog.example.com
///
/// The cluster Docker image (see ../../../Dockerfile + nginx.conf) builds with this set to
/// the empty string on purpose: an empty base URL makes every Dio request path-relative
/// (e.g. "/api/v1/auth/login"), which the browser resolves against whatever origin served
/// the page — nginx in that same container proxies /api and /hubs back to the backend
/// Service internally. That's what lets one build work over any hostname/IP and both
/// HTTP and HTTPS, instead of baking in one specific API host. Only local dev (`flutter
/// run`, no dart-define set) falls back to this http://localhost:8080 default.
class AppConfig {
  AppConfig._();

  static const apiBaseUrl = String.fromEnvironment('API_BASE_URL', defaultValue: 'http://localhost:8080');
}
