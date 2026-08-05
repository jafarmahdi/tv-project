import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../network/ai_api.dart';
import '../network/api_client.dart';
import '../network/auth_api.dart';
import '../network/catalog_api.dart';
import '../network/lists_api.dart';
import '../network/notifications_api.dart';
import '../network/stats_api.dart';
import '../network/token_storage.dart';
import '../network/tracking_api.dart';
import '../network/users_api.dart';
import '../../features/auth/auth_provider.dart';

final tokenStorageProvider = Provider<TokenStorage>((ref) => TokenStorage());

final apiClientProvider = Provider<ApiClient>((ref) {
  final client = ApiClient(ref.watch(tokenStorageProvider));
  // A 401 that survives a refresh attempt means the session is truly gone —
  // force the auth state back to signed-out so the router redirects to /login.
  client.onUnauthorized = () => ref.read(authProvider.notifier).forceSignOut();
  return client;
});

final authApiProvider = Provider<AuthApi>((ref) => AuthApi(ref.watch(apiClientProvider)));
final usersApiProvider = Provider<UsersApi>((ref) => UsersApi(ref.watch(apiClientProvider)));
final catalogApiProvider = Provider<CatalogApi>((ref) => CatalogApi(ref.watch(apiClientProvider)));
final trackingApiProvider = Provider<TrackingApi>((ref) => TrackingApi(ref.watch(apiClientProvider)));
final listsApiProvider = Provider<ListsApi>((ref) => ListsApi(ref.watch(apiClientProvider)));
final statsApiProvider = Provider<StatsApi>((ref) => StatsApi(ref.watch(apiClientProvider)));
final notificationsApiProvider = Provider<NotificationsApi>((ref) => NotificationsApi(ref.watch(apiClientProvider)));
final aiApiProvider = Provider<AiApi>((ref) => AiApi(ref.watch(apiClientProvider)));
