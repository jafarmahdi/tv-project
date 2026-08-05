/// Mirrors `WatchLog.Application.Ai.AiSuggestionDto`.
class AiSuggestion {
  final int tmdbId;
  final bool isSeries;
  final String title;
  final String? posterPath;
  final double voteAverage;
  final String reason;

  AiSuggestion({
    required this.tmdbId,
    required this.isSeries,
    required this.title,
    this.posterPath,
    required this.voteAverage,
    required this.reason,
  });

  factory AiSuggestion.fromJson(Map<String, dynamic> json) => AiSuggestion(
        tmdbId: json['tmdbId'] as int,
        isSeries: json['isSeries'] as bool,
        title: json['title'] as String,
        posterPath: json['posterPath'] as String?,
        voteAverage: (json['voteAverage'] as num).toDouble(),
        reason: json['reason'] as String,
      );
}

/// Mirrors `WatchLog.Application.Ai.AiResponseDto`.
class AiResponse {
  final String message;
  final List<AiSuggestion> suggestions;

  AiResponse({required this.message, required this.suggestions});

  factory AiResponse.fromJson(Map<String, dynamic> json) => AiResponse(
        message: json['message'] as String,
        suggestions:
            (json['suggestions'] as List<dynamic>).map((e) => AiSuggestion.fromJson(e as Map<String, dynamic>)).toList(),
      );
}

/// Mirrors `WatchLog.Application.Ai.AiHistoryItemDto`.
class AiHistoryItem {
  final String id;
  final String prompt;
  final String response;
  final DateTime createdAt;

  AiHistoryItem({required this.id, required this.prompt, required this.response, required this.createdAt});

  factory AiHistoryItem.fromJson(Map<String, dynamic> json) => AiHistoryItem(
        id: json['id'] as String,
        prompt: json['prompt'] as String,
        response: json['response'] as String,
        createdAt: DateTime.parse(json['createdAt'] as String),
      );
}
