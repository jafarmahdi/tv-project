/// Mirrors `WatchLog.Domain.Enums.ListType`.
enum ListType { watching, completed, planned, onHold, dropped, favorites, custom }

extension ListTypeJson on ListType {
  int toJson() => index;

  static ListType fromJson(int value) => ListType.values[value];

  String get label => switch (this) {
        ListType.watching => 'Watching',
        ListType.completed => 'Completed',
        ListType.planned => 'Planned',
        ListType.onHold => 'On Hold',
        ListType.dropped => 'Dropped',
        ListType.favorites => 'Favorites',
        ListType.custom => 'Custom',
      };
}

/// Mirrors `WatchLog.Application.Lists.UserListDto`.
class UserList {
  final String id;
  final String name;
  final ListType type;
  final bool isPublic;
  final int itemCount;

  UserList({
    required this.id,
    required this.name,
    required this.type,
    required this.isPublic,
    required this.itemCount,
  });

  factory UserList.fromJson(Map<String, dynamic> json) => UserList(
        id: json['id'] as String,
        name: json['name'] as String,
        type: ListTypeJson.fromJson(json['type'] as int),
        isPublic: json['isPublic'] as bool,
        itemCount: json['itemCount'] as int,
      );
}

/// Mirrors `WatchLog.Application.Lists.ListItemDto`.
class ListItem {
  final String id;
  final int? movieTmdbId;
  final String? movieTitle;
  final String? moviePosterPath;
  final int? seriesTmdbId;
  final String? seriesTitle;
  final String? seriesPosterPath;
  final DateTime addedAt;

  ListItem({
    required this.id,
    this.movieTmdbId,
    this.movieTitle,
    this.moviePosterPath,
    this.seriesTmdbId,
    this.seriesTitle,
    this.seriesPosterPath,
    required this.addedAt,
  });

  bool get isMovie => movieTmdbId != null;
  String get title => movieTitle ?? seriesTitle ?? '';
  String? get posterPath => moviePosterPath ?? seriesPosterPath;

  factory ListItem.fromJson(Map<String, dynamic> json) => ListItem(
        id: json['id'] as String,
        movieTmdbId: json['movieTmdbId'] as int?,
        movieTitle: json['movieTitle'] as String?,
        moviePosterPath: json['moviePosterPath'] as String?,
        seriesTmdbId: json['seriesTmdbId'] as int?,
        seriesTitle: json['seriesTitle'] as String?,
        seriesPosterPath: json['seriesPosterPath'] as String?,
        addedAt: DateTime.parse(json['addedAt'] as String),
      );
}
