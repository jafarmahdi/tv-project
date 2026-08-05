import 'package:flutter/foundation.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../features/ai_assistant/ai_assistant_screen.dart';
import '../../features/auth/auth_provider.dart';
import '../../features/auth/login_screen.dart';
import '../../features/auth/register_screen.dart';
import '../../features/details/movie_details_screen.dart';
import '../../features/details/series_details_screen.dart';
import '../../features/discover/discover_screen.dart';
import '../../features/episode/season_episodes_screen.dart';
import '../../features/home/home_screen.dart';
import '../../features/notifications/notifications_screen.dart';
import '../../features/profile/profile_screen.dart';
import '../../features/settings/settings_screen.dart';
import '../../features/shell/app_shell.dart';
import '../../features/splash/splash_screen.dart';
import '../../features/stats/stats_screen.dart';

/// Bridges Riverpod's [authProvider] into a [Listenable] so [GoRouter] can
/// re-run its redirect logic whenever the session changes (login/logout).
class _AuthRefreshNotifier extends ChangeNotifier {
  _AuthRefreshNotifier(Ref ref) {
    ref.listen(authProvider, (_, _) => notifyListeners());
  }
}

final routerProvider = Provider<GoRouter>((ref) {
  final refresh = _AuthRefreshNotifier(ref);

  return GoRouter(
    initialLocation: '/splash',
    refreshListenable: refresh,
    redirect: (context, state) {
      final auth = ref.read(authProvider);
      final location = state.matchedLocation;

      if (auth.status == AuthStatus.unknown) {
        // Still resolving the initial session check — hold on splash, don't bounce anywhere yet.
        return location == '/splash' ? null : '/splash';
      }

      const publicAuthRoutes = {'/login', '/register'};
      if (!auth.isAuthenticated) {
        // Resolved as signed-out: `/splash` is not a real destination, so it must redirect too —
        // only `/login`/`/register` are legitimately reachable while signed out.
        return publicAuthRoutes.contains(location) ? null : '/login';
      }

      // Signed in: keep them off splash/login/register.
      if (location == '/splash' || publicAuthRoutes.contains(location)) {
        return '/home';
      }
      return null;
    },
    routes: [
      GoRoute(path: '/splash', builder: (context, state) => const SplashScreen()),
      GoRoute(path: '/login', builder: (context, state) => const LoginScreen()),
      GoRoute(path: '/register', builder: (context, state) => const RegisterScreen()),
      StatefulShellRoute.indexedStack(
        builder: (context, state, shell) => AppShell(navigationShell: shell),
        branches: [
          StatefulShellBranch(routes: [GoRoute(path: '/home', builder: (context, state) => const HomeScreen())]),
          StatefulShellBranch(
              routes: [GoRoute(path: '/discover', builder: (context, state) => const DiscoverScreen())]),
          StatefulShellBranch(routes: [GoRoute(path: '/stats', builder: (context, state) => const StatsScreen())]),
          StatefulShellBranch(
              routes: [GoRoute(path: '/profile', builder: (context, state) => const ProfileScreen())]),
        ],
      ),
      GoRoute(
        path: '/movie/:tmdbId',
        builder: (context, state) =>
            MovieDetailsScreen(tmdbId: int.parse(state.pathParameters['tmdbId']!)),
      ),
      GoRoute(
        path: '/series/:tmdbId',
        builder: (context, state) =>
            SeriesDetailsScreen(tmdbId: int.parse(state.pathParameters['tmdbId']!)),
      ),
      GoRoute(
        path: '/series/:tmdbId/season/:seasonNumber',
        builder: (context, state) => SeasonEpisodesScreen(
          seriesTmdbId: int.parse(state.pathParameters['tmdbId']!),
          seasonNumber: int.parse(state.pathParameters['seasonNumber']!),
        ),
      ),
      GoRoute(path: '/notifications', builder: (context, state) => const NotificationsScreen()),
      GoRoute(path: '/settings', builder: (context, state) => const SettingsScreen()),
      GoRoute(path: '/ai-assistant', builder: (context, state) => const AiAssistantScreen()),
    ],
  );
});
