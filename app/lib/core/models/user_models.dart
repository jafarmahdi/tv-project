/// Mirrors `WatchLog.Application.Users.MeDto`.
class MeProfile {
  final String id;
  final String email;
  final String displayName;
  final String? avatarUrl;
  final String? bio;
  final String locale;
  final int themePreference;
  final bool isPrivate;
  final DateTime createdAt;
  final int followerCount;
  final int followingCount;
  final bool isAdmin;

  MeProfile({
    required this.id,
    required this.email,
    required this.displayName,
    this.avatarUrl,
    this.bio,
    required this.locale,
    required this.themePreference,
    required this.isPrivate,
    required this.createdAt,
    required this.followerCount,
    required this.followingCount,
    required this.isAdmin,
  });

  factory MeProfile.fromJson(Map<String, dynamic> json) => MeProfile(
    id: json['id'] as String,
    email: json['email'] as String,
    displayName: json['displayName'] as String,
    avatarUrl: json['avatarUrl'] as String?,
    bio: json['bio'] as String?,
    locale: json['locale'] as String,
    themePreference: json['themePreference'] as int,
    isPrivate: json['isPrivate'] as bool,
    createdAt: DateTime.parse(json['createdAt'] as String),
    followerCount: json['followerCount'] as int,
    followingCount: json['followingCount'] as int,
    isAdmin: json['isAdmin'] as bool? ?? false,
  );
}

/// Mirrors `WatchLog.Application.Users.UserProfileDto`.
class PublicProfile {
  final String id;
  final String displayName;
  final String? avatarUrl;
  final String? bio;
  final bool isPrivate;

  PublicProfile({
    required this.id,
    required this.displayName,
    this.avatarUrl,
    this.bio,
    required this.isPrivate,
  });

  factory PublicProfile.fromJson(Map<String, dynamic> json) => PublicProfile(
    id: json['id'] as String,
    displayName: json['displayName'] as String,
    avatarUrl: json['avatarUrl'] as String?,
    bio: json['bio'] as String?,
    isPrivate: json['isPrivate'] as bool,
  );
}
