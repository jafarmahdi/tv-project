namespace WatchLog.Application.Ai;

/// <summary>
/// "I want a show like Dark but less confusing." / "I have only 90 minutes." / "Recommend something
/// similar to Breaking Bad." A real (non-fake) heuristic implementation ships now — genre affinity +
/// runtime constraints + TMDB "similar to" lookups, learned from the user's own watch history. The
/// interface is intentionally provider-agnostic so a future LLM-backed implementation is a drop-in
/// replacement (see docs/ROADMAP.md, phase 5).
/// </summary>
public interface IAiAssistantService
{
    Task<AiResponseDto> AskAsync(Guid userId, string prompt, CancellationToken ct = default);
    Task<IReadOnlyList<AiHistoryItemDto>> GetHistoryAsync(Guid userId, CancellationToken ct = default);
}
