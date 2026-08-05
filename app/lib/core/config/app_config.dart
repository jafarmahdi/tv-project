/// Build-time configuration. Override at build time with:
///   flutter build web --dart-define=API_BASE_URL=https://api.watchlog.example.com
class AppConfig {
  AppConfig._();

  static const apiBaseUrl = String.fromEnvironment('API_BASE_URL', defaultValue: 'http://localhost:8080');
}
