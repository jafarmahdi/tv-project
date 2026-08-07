enum TargetType { movie, series, episode, comment, userList, user }

extension TargetTypeJson on TargetType {
  int toJson() => index;
}

class RatingSummary {
  final double average;
  final int count;
  final int? myScore;

  RatingSummary({
    required this.average,
    required this.count,
    required this.myScore,
  });

  factory RatingSummary.fromJson(Map<String, dynamic> json) => RatingSummary(
    average: (json['average'] as num).toDouble(),
    count: json['count'] as int,
    myScore: json['myScore'] as int?,
  );
}

class CommentEntry {
  final String id;
  final String userId;
  final String userDisplayName;
  final String? userAvatarUrl;
  final String body;
  final String? parentCommentId;
  final DateTime createdAt;
  final int likeCount;

  CommentEntry({
    required this.id,
    required this.userId,
    required this.userDisplayName,
    required this.userAvatarUrl,
    required this.body,
    required this.parentCommentId,
    required this.createdAt,
    required this.likeCount,
  });

  factory CommentEntry.fromJson(Map<String, dynamic> json) => CommentEntry(
    id: json['id'] as String,
    userId: json['userId'] as String,
    userDisplayName: json['userDisplayName'] as String,
    userAvatarUrl: json['userAvatarUrl'] as String?,
    body: json['body'] as String,
    parentCommentId: json['parentCommentId'] as String?,
    createdAt: DateTime.parse(json['createdAt'] as String),
    likeCount: json['likeCount'] as int,
  );
}
