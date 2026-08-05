import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../core/models/user_models.dart';
import '../../core/providers/core_providers.dart';

enum AuthStatus { unknown, authenticated, unauthenticated }

class AuthState {
  final AuthStatus status;
  final MeProfile? profile;

  const AuthState({required this.status, this.profile});
  const AuthState.unknown() : this(status: AuthStatus.unknown);
  const AuthState.authenticated(MeProfile profile) : this(status: AuthStatus.authenticated, profile: profile);
  const AuthState.unauthenticated() : this(status: AuthStatus.unauthenticated);

  bool get isAuthenticated => status == AuthStatus.authenticated;
}

/// Owns the app's session: who's signed in (if anyone), and the actions that
/// change that (register/login/logout). The router watches this to decide
/// whether to show the auth flow or the app shell.
class AuthNotifier extends Notifier<AuthState> {
  @override
  AuthState build() {
    Future.microtask(_checkInitialSession);
    return const AuthState.unknown();
  }

  Future<void> _checkInitialSession() async {
    final token = await ref.read(tokenStorageProvider).readAccessToken();
    if (token == null) {
      state = const AuthState.unauthenticated();
      return;
    }
    try {
      final profile = await ref.read(usersApiProvider).getMe();
      state = AuthState.authenticated(profile);
    } catch (_) {
      state = const AuthState.unauthenticated();
    }
  }

  Future<void> register({required String email, required String password, required String displayName}) async {
    final result =
        await ref.read(authApiProvider).register(email: email, password: password, displayName: displayName);
    await ref.read(tokenStorageProvider).save(accessToken: result.accessToken, refreshToken: result.refreshToken);
    final profile = await ref.read(usersApiProvider).getMe();
    state = AuthState.authenticated(profile);
  }

  Future<void> login({required String email, required String password}) async {
    final result = await ref.read(authApiProvider).login(email: email, password: password);
    await ref.read(tokenStorageProvider).save(accessToken: result.accessToken, refreshToken: result.refreshToken);
    final profile = await ref.read(usersApiProvider).getMe();
    state = AuthState.authenticated(profile);
  }

  Future<void> logout() async {
    final refreshToken = await ref.read(tokenStorageProvider).readRefreshToken();
    if (refreshToken != null) {
      try {
        await ref.read(authApiProvider).logout(refreshToken);
      } catch (_) {
        // Best-effort server-side revocation; local sign-out proceeds regardless.
      }
    }
    await ref.read(tokenStorageProvider).clear();
    state = const AuthState.unauthenticated();
  }

  /// Called by [ApiClient.onUnauthorized] when a request 401s and the refresh
  /// token is also invalid/expired — the session is gone, so drop straight
  /// to signed-out without hitting the network again.
  void forceSignOut() {
    state = const AuthState.unauthenticated();
  }

  Future<void> refreshProfile() async {
    if (!state.isAuthenticated) return;
    try {
      final profile = await ref.read(usersApiProvider).getMe();
      state = AuthState.authenticated(profile);
    } catch (_) {
      // Keep the stale profile rather than bouncing the user for a transient failure.
    }
  }
}

final authProvider = NotifierProvider<AuthNotifier, AuthState>(AuthNotifier.new);
