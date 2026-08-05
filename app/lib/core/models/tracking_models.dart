/// Mirrors `WatchLog.Domain.Enums.EpisodeWatchStatus`. The API serializes enums as their
/// underlying int (no `JsonStringEnumConverter` configured), so the order here must match exactly.
enum EpisodeWatchStatus { unwatched, watched, skipped }

extension EpisodeWatchStatusJson on EpisodeWatchStatus {
  int toJson() => index;

  static EpisodeWatchStatus fromJson(int value) => EpisodeWatchStatus.values[value];
}

/// Mirrors `WatchLog.Application.Tracking.EpisodeProgressDto`.
class EpisodeProgress {
  final int episodeNumber;
  final String title;
  final EpisodeWatchStatus status;
  final bool isFavorite;
  final DateTime? watchedAt;

  EpisodeProgress({
    required this.episodeNumber,
    required this.title,
    required this.status,
    required this.isFavorite,
    this.watchedAt,
  });

  factory EpisodeProgress.fromJson(Map<String, dynamic> json) => EpisodeProgress(
        episodeNumber: json['episodeNumber'] as int,
        title: json['title'] as String,
        status: EpisodeWatchStatusJson.fromJson(json['status'] as int),
        isFavorite: json['isFavorite'] as bool,
        watchedAt: json['watchedAt'] == null ? null : DateTime.parse(json['watchedAt'] as String),
      );
}

/// Mirrors `WatchLog.Application.Tracking.SeasonProgressDto`.
class SeasonProgress {
  final int seasonNumber;
  final int totalEpisodes;
  final int watchedEpisodes;
  final List<EpisodeProgress> episodes;

  SeasonProgress({
    required this.seasonNumber,
    required this.totalEpisodes,
    required this.watchedEpisodes,
    required this.episodes,
  });

  factory SeasonProgress.fromJson(Map<String, dynamic> json) => SeasonProgress(
        seasonNumber: json['seasonNumber'] as int,
        totalEpisodes: json['totalEpisodes'] as int,
        watchedEpisodes: json['watchedEpisodes'] as int,
        episodes:
            (json['episodes'] as List<dynamic>).map((e) => EpisodeProgress.fromJson(e as Map<String, dynamic>)).toList(),
      );
}

/// Mirrors `WatchLog.Application.Tracking.NextEpisodeDto`.
class NextEpisode {
  final int seasonNumber;
  final int episodeNumber;
  final String title;
  final String? stillPath;
  final DateTime? airDate;

  NextEpisode({
    required this.seasonNumber,
    required this.episodeNumber,
    required this.title,
    this.stillPath,
    this.airDate,
  });

  factory NextEpisode.fromJson(Map<String, dynamic> json) => NextEpisode(
        seasonNumber: json['seasonNumber'] as int,
        episodeNumber: json['episodeNumber'] as int,
        title: json['title'] as String,
        stillPath: json['stillPath'] as String?,
        airDate: json['airDate'] == null ? null : DateTime.parse(json['airDate'] as String),
      );
}
