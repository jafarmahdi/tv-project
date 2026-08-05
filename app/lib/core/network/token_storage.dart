import 'package:shared_preferences/shared_preferences.dart';

/// Persists the JWT access/refresh token pair. Uses `shared_preferences`
/// (backed by `localStorage` on web) rather than `flutter_secure_storage`
/// because the latter has no web implementation — acceptable for a web
/// client where tokens are short-lived and refreshed automatically; native
/// builds should switch this to secure storage before shipping.
class TokenStorage {
  static const _accessKey = 'watchlog.accessToken';
  static const _refreshKey = 'watchlog.refreshToken';

  Future<String?> readAccessToken() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getString(_accessKey);
  }

  Future<String?> readRefreshToken() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getString(_refreshKey);
  }

  Future<void> save({required String accessToken, required String refreshToken}) async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString(_accessKey, accessToken);
    await prefs.setString(_refreshKey, refreshToken);
  }

  Future<void> clear() async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove(_accessKey);
    await prefs.remove(_refreshKey);
  }
}
