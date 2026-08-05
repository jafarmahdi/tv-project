import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../core/models/catalog_models.dart';
import '../../core/models/tracking_models.dart';
import '../../core/providers/core_providers.dart';

class EpisodeRow {
  final EpisodeSummary summary;
  final EpisodeProgress progress;
  const EpisodeRow({required this.summary, required this.progress});
}

class SeasonWithProgress {
  final SeasonDetail season;
  final SeasonProgress progress;
  final List<EpisodeRow> rows;
  const SeasonWithProgress({required this.season, required this.progress, required this.rows});
}

typedef SeriesSeasonKey = ({int seriesTmdbId, int seasonNumber});

final seasonWithProgressProvider = FutureProvider.autoDispose.family<SeasonWithProgress, SeriesSeasonKey>((ref, key) async {
  final catalog = ref.watch(catalogApiProvider);
  final tracking = ref.watch(trackingApiProvider);

  final results = await Future.wait([
    catalog.season(key.seriesTmdbId, key.seasonNumber),
    tracking.seasonProgress(key.seriesTmdbId, key.seasonNumber),
  ]);
  final season = results[0] as SeasonDetail;
  final progress = results[1] as SeasonProgress;

  final progressByNumber = {for (final p in progress.episodes) p.episodeNumber: p};
  final rows = season.episodes
      .map((e) => EpisodeRow(
            summary: e,
            progress: progressByNumber[e.episodeNumber] ??
                EpisodeProgress(episodeNumber: e.episodeNumber, title: e.title, status: EpisodeWatchStatus.unwatched, isFavorite: false),
          ))
      .toList();

  return SeasonWithProgress(season: season, progress: progress, rows: rows);
});
