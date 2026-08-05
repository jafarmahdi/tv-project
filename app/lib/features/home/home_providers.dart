import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../core/models/catalog_models.dart';
import '../../core/models/paged_result.dart';
import '../../core/providers/core_providers.dart';

final trendingMoviesProvider =
    FutureProvider.autoDispose<PagedResult<MovieSummary>>((ref) => ref.watch(catalogApiProvider).trendingMovies());

final trendingSeriesProvider =
    FutureProvider.autoDispose<PagedResult<SeriesSummary>>((ref) => ref.watch(catalogApiProvider).trendingSeries());

final unreadNotificationCountProvider = FutureProvider.autoDispose<int>((ref) async {
  final notifications = await ref.watch(notificationsApiProvider).getAll(unreadOnly: true);
  return notifications.length;
});
