import 'package:flutter/foundation.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:shared_preferences/shared_preferences.dart';

/// Persists the JWT access/refresh token pair. Uses `shared_preferences`
/// (backed by `localStorage` on web) rather than `flutter_secure_storage`
/// because web works best with the browser's own storage model. On native
/// platforms we store the tokens in the OS keychain/keystore instead.
class TokenStorage {
  static const _accessKey = 'watchlog.accessToken';
  static const _refreshKey = 'watchlog.refreshToken';
  static const _secureStorage = FlutterSecureStorage(
    aOptions: AndroidOptions.defaultOptions,
  );

  Future<String?> readAccessToken() async {
    if (!kIsWeb) return _secureStorage.read(key: _accessKey);
    final prefs = await SharedPreferences.getInstance();
    return prefs.getString(_accessKey);
  }

  Future<String?> readRefreshToken() async {
    if (!kIsWeb) return _secureStorage.read(key: _refreshKey);
    final prefs = await SharedPreferences.getInstance();
    return prefs.getString(_refreshKey);
  }

  Future<void> save({
    required String accessToken,
    required String refreshToken,
  }) async {
    if (!kIsWeb) {
      await _secureStorage.write(key: _accessKey, value: accessToken);
      await _secureStorage.write(key: _refreshKey, value: refreshToken);
      return;
    }
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString(_accessKey, accessToken);
    await prefs.setString(_refreshKey, refreshToken);
  }

  Future<void> clear() async {
    if (!kIsWeb) {
      await _secureStorage.delete(key: _accessKey);
      await _secureStorage.delete(key: _refreshKey);
      return;
    }
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove(_accessKey);
    await prefs.remove(_refreshKey);
  }
}
