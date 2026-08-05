import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../core/models/catalog_models.dart';
import '../../core/models/paged_result.dart';
import '../../core/providers/core_providers.dart';

final movieDetailProvider =
    FutureProvider.autoDispose.family<MovieDetail, int>((ref, tmdbId) => ref.watch(catalogApiProvider).movieDetail(tmdbId));

final similarMoviesProvider = FutureProvider.autoDispose.family<PagedResult<MovieSummary>, int>(
    (ref, tmdbId) => ref.watch(catalogApiProvider).similarMovies(tmdbId));

final movieWatchProvidersProvider = FutureProvider.autoDispose
    .family<List<WatchProvider>, int>((ref, tmdbId) => ref.watch(catalogApiProvider).movieWatchProviders(tmdbId));

final seriesDetailProvider = FutureProvider.autoDispose
    .family<SeriesDetail, int>((ref, tmdbId) => ref.watch(catalogApiProvider).seriesDetail(tmdbId));

final similarSeriesProvider = FutureProvider.autoDispose.family<PagedResult<SeriesSummary>, int>(
    (ref, tmdbId) => ref.watch(catalogApiProvider).similarSeries(tmdbId));

final seriesWatchProvidersProvider = FutureProvider.autoDispose
    .family<List<WatchProvider>, int>((ref, tmdbId) => ref.watch(catalogApiProvider).seriesWatchProviders(tmdbId));
