/// Mirrors `WatchLog.Application.Stats.MonthlyActivityDto`.
class MonthlyActivity {
  final int year;
  final int month;
  final int episodesWatched;
  final int moviesWatched;

  MonthlyActivity({
    required this.year,
    required this.month,
    required this.episodesWatched,
    required this.moviesWatched,
  });

  factory MonthlyActivity.fromJson(Map<String, dynamic> json) => MonthlyActivity(
        year: json['year'] as int,
        month: json['month'] as int,
        episodesWatched: json['episodesWatched'] as int,
        moviesWatched: json['moviesWatched'] as int,
      );
}

/// Mirrors `WatchLog.Application.Stats.GenreStatDto`.
class GenreStat {
  final String genre;
  final int count;

  GenreStat({required this.genre, required this.count});

  factory GenreStat.fromJson(Map<String, dynamic> json) =>
      GenreStat(genre: json['genre'] as String, count: json['count'] as int);
}

/// Mirrors `WatchLog.Application.Stats.HeatmapDayDto`.
class HeatmapDay {
  final DateTime date;
  final int count;

  HeatmapDay({required this.date, required this.count});

  factory HeatmapDay.fromJson(Map<String, dynamic> json) =>
      HeatmapDay(date: DateTime.parse(json['date'] as String), count: json['count'] as int);
}

/// Mirrors `WatchLog.Application.Stats.BadgeDto`. Named `AchievementBadge` here (not `Badge`)
/// to avoid colliding with `package:flutter/material.dart`'s `Badge` widget.
class AchievementBadge {
  final String code;
  final String name;
  final String description;
  final String? iconUrl;
  final DateTime earnedAt;

  AchievementBadge({
    required this.code,
    required this.name,
    required this.description,
    this.iconUrl,
    required this.earnedAt,
  });

  factory AchievementBadge.fromJson(Map<String, dynamic> json) => AchievementBadge(
        code: json['code'] as String,
        name: json['name'] as String,
        description: json['description'] as String,
        iconUrl: json['iconUrl'] as String?,
        earnedAt: DateTime.parse(json['earnedAt'] as String),
      );
}

/// Mirrors `WatchLog.Application.Stats.UserStatsDto`.
class UserStats {
  final int totalEpisodesWatched;
  final int totalMoviesWatched;
  final int totalWatchTimeMinutes;
  final List<MonthlyActivity> monthlyActivity;
  final List<GenreStat> favoriteGenres;
  final List<HeatmapDay> heatmapCalendar;
  final List<AchievementBadge> achievements;

  UserStats({
    required this.totalEpisodesWatched,
    required this.totalMoviesWatched,
    required this.totalWatchTimeMinutes,
    required this.monthlyActivity,
    required this.favoriteGenres,
    required this.heatmapCalendar,
    required this.achievements,
  });

  factory UserStats.fromJson(Map<String, dynamic> json) => UserStats(
        totalEpisodesWatched: json['totalEpisodesWatched'] as int,
        totalMoviesWatched: json['totalMoviesWatched'] as int,
        totalWatchTimeMinutes: json['totalWatchTimeMinutes'] as int,
        monthlyActivity: (json['monthlyActivity'] as List<dynamic>)
            .map((e) => MonthlyActivity.fromJson(e as Map<String, dynamic>))
            .toList(),
        favoriteGenres:
            (json['favoriteGenres'] as List<dynamic>).map((e) => GenreStat.fromJson(e as Map<String, dynamic>)).toList(),
        heatmapCalendar: (json['heatmapCalendar'] as List<dynamic>)
            .map((e) => HeatmapDay.fromJson(e as Map<String, dynamic>))
            .toList(),
        achievements:
            (json['achievements'] as List<dynamic>).map((e) => AchievementBadge.fromJson(e as Map<String, dynamic>)).toList(),
      );
}
