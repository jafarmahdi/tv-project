import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../core/models/catalog_models.dart';
import '../../core/models/paged_result.dart';
import '../../core/providers/core_providers.dart';

final discoverQueryProvider = NotifierProvider<DiscoverQueryNotifier, String>(DiscoverQueryNotifier.new);

class DiscoverQueryNotifier extends Notifier<String> {
  @override
  String build() => '';

  void set(String value) => state = value;
}

final discoverMoviesProvider = FutureProvider.autoDispose<PagedResult<MovieSummary>>((ref) {
  final query = ref.watch(discoverQueryProvider);
  final catalog = ref.watch(catalogApiProvider);
  return query.trim().isEmpty ? catalog.trendingMovies() : catalog.searchMovies(query.trim());
});

final discoverSeriesProvider = FutureProvider.autoDispose<PagedResult<SeriesSummary>>((ref) {
  final query = ref.watch(discoverQueryProvider);
  final catalog = ref.watch(catalogApiProvider);
  return query.trim().isEmpty ? catalog.trendingSeries() : catalog.searchSeries(query.trim());
});
