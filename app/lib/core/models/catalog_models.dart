DateTime? _parseDate(dynamic value) => value == null ? null : DateTime.parse(value as String);

/// Mirrors `WatchLog.Application.Catalog.MovieSummaryDto`.
class MovieSummary {
  final int tmdbId;
  final String title;
  final String? posterPath;
  final String? backdropPath;
  final DateTime? releaseDate;
  final double voteAverage;
  final List<String> genres;

  MovieSummary({
    required this.tmdbId,
    required this.title,
    this.posterPath,
    this.backdropPath,
    this.releaseDate,
    required this.voteAverage,
    required this.genres,
  });

  factory MovieSummary.fromJson(Map<String, dynamic> json) => MovieSummary(
        tmdbId: json['tmdbId'] as int,
        title: json['title'] as String,
        posterPath: json['posterPath'] as String?,
        backdropPath: json['backdropPath'] as String?,
        releaseDate: _parseDate(json['releaseDate']),
        voteAverage: (json['voteAverage'] as num).toDouble(),
        genres: (json['genres'] as List<dynamic>).cast<String>(),
      );
}

/// Mirrors `WatchLog.Application.Catalog.SeriesSummaryDto`.
class SeriesSummary {
  final int tmdbId;
  final String title;
  final String? posterPath;
  final String? backdropPath;
  final DateTime? firstAirDate;
  final double voteAverage;
  final List<String> genres;

  SeriesSummary({
    required this.tmdbId,
    required this.title,
    this.posterPath,
    this.backdropPath,
    this.firstAirDate,
    required this.voteAverage,
    required this.genres,
  });

  factory SeriesSummary.fromJson(Map<String, dynamic> json) => SeriesSummary(
        tmdbId: json['tmdbId'] as int,
        title: json['title'] as String,
        posterPath: json['posterPath'] as String?,
        backdropPath: json['backdropPath'] as String?,
        firstAirDate: _parseDate(json['firstAirDate']),
        voteAverage: (json['voteAverage'] as num).toDouble(),
        genres: (json['genres'] as List<dynamic>).cast<String>(),
      );
}

class CastMember {
  final int tmdbId;
  final String name;
  final String? character;
  final String? profilePath;

  CastMember({required this.tmdbId, required this.name, this.character, this.profilePath});

  factory CastMember.fromJson(Map<String, dynamic> json) => CastMember(
        tmdbId: json['tmdbId'] as int,
        name: json['name'] as String,
        character: json['character'] as String?,
        profilePath: json['profilePath'] as String?,
      );
}

/// Mirrors `WatchLog.Application.Catalog.MovieDetailDto`.
class MovieDetail {
  final int tmdbId;
  final String title;
  final String? originalTitle;
  final String? overview;
  final String? posterPath;
  final String? backdropPath;
  final DateTime? releaseDate;
  final int? runtimeMinutes;
  final double voteAverage;
  final List<String> genres;
  final List<CastMember> cast;
  final String? trailerYoutubeKey;

  MovieDetail({
    required this.tmdbId,
    required this.title,
    this.originalTitle,
    this.overview,
    this.posterPath,
    this.backdropPath,
    this.releaseDate,
    this.runtimeMinutes,
    required this.voteAverage,
    required this.genres,
    required this.cast,
    this.trailerYoutubeKey,
  });

  factory MovieDetail.fromJson(Map<String, dynamic> json) => MovieDetail(
        tmdbId: json['tmdbId'] as int,
        title: json['title'] as String,
        originalTitle: json['originalTitle'] as String?,
        overview: json['overview'] as String?,
        posterPath: json['posterPath'] as String?,
        backdropPath: json['backdropPath'] as String?,
        releaseDate: _parseDate(json['releaseDate']),
        runtimeMinutes: json['runtimeMinutes'] as int?,
        voteAverage: (json['voteAverage'] as num).toDouble(),
        genres: (json['genres'] as List<dynamic>).cast<String>(),
        cast: (json['cast'] as List<dynamic>).map((e) => CastMember.fromJson(e as Map<String, dynamic>)).toList(),
        trailerYoutubeKey: json['trailerYoutubeKey'] as String?,
      );
}

/// Mirrors `WatchLog.Application.Catalog.SeasonSummaryDto`.
class SeasonSummary {
  final int seasonNumber;
  final String name;
  final String? posterPath;
  final DateTime? airDate;
  final int episodeCount;

  SeasonSummary({
    required this.seasonNumber,
    required this.name,
    this.posterPath,
    this.airDate,
    required this.episodeCount,
  });

  factory SeasonSummary.fromJson(Map<String, dynamic> json) => SeasonSummary(
        seasonNumber: json['seasonNumber'] as int,
        name: json['name'] as String,
        posterPath: json['posterPath'] as String?,
        airDate: _parseDate(json['airDate']),
        episodeCount: json['episodeCount'] as int,
      );
}

/// Mirrors `WatchLog.Application.Catalog.SeriesDetailDto`.
class SeriesDetail {
  final int tmdbId;
  final String title;
  final String? originalTitle;
  final String? overview;
  final String? posterPath;
  final String? backdropPath;
  final DateTime? firstAirDate;
  final DateTime? lastAirDate;
  final String status;
  final double voteAverage;
  final List<String> genres;
  final List<CastMember> cast;
  final List<SeasonSummary> seasons;
  final String? trailerYoutubeKey;

  SeriesDetail({
    required this.tmdbId,
    required this.title,
    this.originalTitle,
    this.overview,
    this.posterPath,
    this.backdropPath,
    this.firstAirDate,
    this.lastAirDate,
    required this.status,
    required this.voteAverage,
    required this.genres,
    required this.cast,
    required this.seasons,
    this.trailerYoutubeKey,
  });

  factory SeriesDetail.fromJson(Map<String, dynamic> json) => SeriesDetail(
        tmdbId: json['tmdbId'] as int,
        title: json['title'] as String,
        originalTitle: json['originalTitle'] as String?,
        overview: json['overview'] as String?,
        posterPath: json['posterPath'] as String?,
        backdropPath: json['backdropPath'] as String?,
        firstAirDate: _parseDate(json['firstAirDate']),
        lastAirDate: _parseDate(json['lastAirDate']),
        status: json['status'] as String,
        voteAverage: (json['voteAverage'] as num).toDouble(),
        genres: (json['genres'] as List<dynamic>).cast<String>(),
        cast: (json['cast'] as List<dynamic>).map((e) => CastMember.fromJson(e as Map<String, dynamic>)).toList(),
        seasons: (json['seasons'] as List<dynamic>)
            .map((e) => SeasonSummary.fromJson(e as Map<String, dynamic>))
            .toList(),
        trailerYoutubeKey: json['trailerYoutubeKey'] as String?,
      );
}

/// Mirrors `WatchLog.Application.Catalog.EpisodeSummaryDto`.
class EpisodeSummary {
  final int episodeNumber;
  final String title;
  final String? overview;
  final String? stillPath;
  final DateTime? airDate;
  final int? runtimeMinutes;

  EpisodeSummary({
    required this.episodeNumber,
    required this.title,
    this.overview,
    this.stillPath,
    this.airDate,
    this.runtimeMinutes,
  });

  factory EpisodeSummary.fromJson(Map<String, dynamic> json) => EpisodeSummary(
        episodeNumber: json['episodeNumber'] as int,
        title: json['title'] as String,
        overview: json['overview'] as String?,
        stillPath: json['stillPath'] as String?,
        airDate: _parseDate(json['airDate']),
        runtimeMinutes: json['runtimeMinutes'] as int?,
      );
}

/// Mirrors `WatchLog.Application.Catalog.SeasonDetailDto`.
class SeasonDetail {
  final int seasonNumber;
  final String name;
  final String? overview;
  final String? posterPath;
  final DateTime? airDate;
  final List<EpisodeSummary> episodes;

  SeasonDetail({
    required this.seasonNumber,
    required this.name,
    this.overview,
    this.posterPath,
    this.airDate,
    required this.episodes,
  });

  factory SeasonDetail.fromJson(Map<String, dynamic> json) => SeasonDetail(
        seasonNumber: json['seasonNumber'] as int,
        name: json['name'] as String,
        overview: json['overview'] as String?,
        posterPath: json['posterPath'] as String?,
        airDate: _parseDate(json['airDate']),
        episodes: (json['episodes'] as List<dynamic>)
            .map((e) => EpisodeSummary.fromJson(e as Map<String, dynamic>))
            .toList(),
      );
}

/// Mirrors `WatchLog.Application.Catalog.WatchProviderDto`.
class WatchProvider {
  final String providerName;
  final String? logoPath;
  final String type;

  WatchProvider({required this.providerName, this.logoPath, required this.type});

  factory WatchProvider.fromJson(Map<String, dynamic> json) => WatchProvider(
        providerName: json['providerName'] as String,
        logoPath: json['logoPath'] as String?,
        type: json['type'] as String,
      );
}
