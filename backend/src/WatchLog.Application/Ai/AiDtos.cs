namespace WatchLog.Application.Ai;

public record AiAskRequest(string Prompt);

public record AiSuggestionDto(int TmdbId, bool IsSeries, string Title, string? PosterPath, double VoteAverage, string Reason);

public record AiResponseDto(string Message, IReadOnlyList<AiSuggestionDto> Suggestions);

public record AiHistoryItemDto(Guid Id, string Prompt, string Response, DateTimeOffset CreatedAt);
