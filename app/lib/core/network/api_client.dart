import 'package:dio/dio.dart';

import '../config/app_config.dart';
import 'api_exception.dart';
import 'token_storage.dart';

/// Wraps a configured [Dio] instance: attaches the JWT access token to every
/// request, and transparently refreshes + retries once on a 401 before
/// giving up and calling [onUnauthorized] (the app wires this to force a
/// logout/redirect to the login screen).
class ApiClient {
  final TokenStorage tokenStorage;
  late final Dio dio;
  void Function()? onUnauthorized;

  /// A bare Dio instance with no interceptors, used only for the refresh call
  /// itself so a failed refresh can't recursively trigger another refresh.
  final Dio _bareDio = Dio(BaseOptions(baseUrl: AppConfig.apiBaseUrl));

  Future<bool>? _refreshInFlight;

  ApiClient(this.tokenStorage) {
    dio = Dio(BaseOptions(
      baseUrl: AppConfig.apiBaseUrl,
      connectTimeout: const Duration(seconds: 15),
      receiveTimeout: const Duration(seconds: 15),
    ));

    dio.interceptors.add(InterceptorsWrapper(
      onRequest: (options, handler) async {
        final token = await tokenStorage.readAccessToken();
        if (token != null) {
          options.headers['Authorization'] = 'Bearer $token';
        }
        handler.next(options);
      },
      onError: (error, handler) async {
        final isUnauthorized = error.response?.statusCode == 401;
        final alreadyRetried = error.requestOptions.extra['watchlogRetried'] == true;

        if (isUnauthorized && !alreadyRetried) {
          final refreshed = await _refresh();
          if (refreshed) {
            try {
              final retryOptions = error.requestOptions;
              retryOptions.extra['watchlogRetried'] = true;
              final token = await tokenStorage.readAccessToken();
              retryOptions.headers['Authorization'] = 'Bearer $token';
              final response = await dio.fetch(retryOptions);
              return handler.resolve(response);
            } catch (_) {
              // Fall through to the unauthorized handling below.
            }
          }
          await tokenStorage.clear();
          onUnauthorized?.call();
        }

        handler.next(error);
      },
    ));
  }

  Future<bool> _refresh() {
    // De-duplicate concurrent refresh attempts (several requests can 401 at once).
    return _refreshInFlight ??= _doRefresh().whenComplete(() => _refreshInFlight = null);
  }

  Future<bool> _doRefresh() async {
    final refreshToken = await tokenStorage.readRefreshToken();
    if (refreshToken == null) return false;

    try {
      final response = await _bareDio.post('/api/v1/auth/refresh', data: {'refreshToken': refreshToken});
      final accessToken = response.data['accessToken'] as String;
      final newRefreshToken = response.data['refreshToken'] as String;
      await tokenStorage.save(accessToken: accessToken, refreshToken: newRefreshToken);
      return true;
    } catch (_) {
      return false;
    }
  }

  /// Runs [request] and rethrows any Dio error as an [ApiException] with a
  /// user-displayable message, so screens never have to unwrap Dio internals.
  Future<T> guard<T>(Future<T> Function() request) async {
    try {
      return await request();
    } on DioException catch (e) {
      throw ApiException.fromResponseData(e.response?.statusCode, e.response?.data);
    }
  }
}
