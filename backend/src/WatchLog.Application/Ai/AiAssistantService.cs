using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using WatchLog.Application.Catalog;
using WatchLog.Application.Common.Interfaces;
using WatchLog.Domain.Entities;
using WatchLog.Domain.Enums;

namespace WatchLog.Application.Ai;

/// <summary>
/// Heuristic recommendation engine: parses a handful of common request shapes out of free text
/// (a runtime budget, "like/similar to &lt;title&gt;"), falls back to the user's own favorite-genre
/// affinity mined from watch history, and always excludes what the user has already watched.
/// </summary>
public partial class AiAssistantService(ICatalogService catalog, ITmdbClient tmdb, IUnitOfWork unitOfWork) : IAiAssistantService
{
    public async Task<AiResponseDto> AskAsync(Guid userId, string prompt, CancellationToken ct = default)
    {
        var watchedMovieTmdbIds = await unitOfWork.Repository<MovieWatch>().Query()
            .Where(w => w.UserId == userId && w.IsWatched).Select(w => w.Movie.TmdbId).ToListAsync(ct);
        var watchedSeriesTmdbIds = await unitOfWork.Repository<EpisodeProgress>().Query()
            .Where(p => p.UserId == userId && p.Status == EpisodeWatchStatus.Watched)
            .Select(p => p.Episode.Season.Series.TmdbId).Distinct().ToListAsync(ct);

        List<AiSuggestionDto> suggestions;
        string message;

        var runtimeMatch = RuntimeRegex().Match(prompt);
        var likeMatch = LikeRegex().Match(prompt);

        if (likeMatch.Success)
        {
            var title = likeMatch.Groups["title"].Value.Trim().TrimEnd('.', '!', '?');
            (suggestions, message) = await SuggestSimilarToAsync(title, watchedMovieTmdbIds, watchedSeriesTmdbIds, ct);
        }
        else if (runtimeMatch.Success && int.TryParse(runtimeMatch.Groups["minutes"].Value, out var maxMinutes))
        {
            (suggestions, message) = await SuggestByRuntimeAsync(maxMinutes, watchedMovieTmdbIds, ct);
        }
        else
        {
            (suggestions, message) = await SuggestByFavoriteGenresAsync(userId, watchedMovieTmdbIds, watchedSeriesTmdbIds, ct);
        }

        foreach (var s in suggestions)
        {
            await unitOfWork.Repository<Recommendation>().AddAsync(new Recommendation
            {
                UserId = userId,
                TargetType = s.IsSeries ? TargetType.Series : TargetType.Movie,
                TargetId = s.IsSeries ? await catalog.EnsureSeriesCachedAsync(s.TmdbId, ct) : await catalog.EnsureMovieCachedAsync(s.TmdbId, ct),
                Score = s.VoteAverage,
                Reason = s.Reason,
                Source = RecommendationSource.Ai
            }, ct);
        }

        await unitOfWork.Repository<AiHistoryEntry>().AddAsync(new AiHistoryEntry
        {
            UserId = userId,
            Prompt = prompt,
            Response = message
        }, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return new AiResponseDto(message, suggestions);
    }

    public async Task<IReadOnlyList<AiHistoryItemDto>> GetHistoryAsync(Guid userId, CancellationToken ct = default) =>
        await unitOfWork.Repository<AiHistoryEntry>().Query()
            .Where(h => h.UserId == userId)
            .OrderByDescending(h => h.CreatedAt)
            .Select(h => new AiHistoryItemDto(h.Id, h.Prompt, h.Response, h.CreatedAt))
            .ToListAsync(ct);

    private async Task<(List<AiSuggestionDto>, string)> SuggestSimilarToAsync(
        string title, List<int> watchedMovies, List<int> watchedSeries, CancellationToken ct)
    {
        var movieHits = await tmdb.SearchMoviesAsync(title, 1, ct);
        var seriesHits = await tmdb.SearchSeriesAsync(title, 1, ct);

        var suggestions = new List<AiSuggestionDto>();

        if (movieHits.Results.Count > 0)
        {
            var similar = await tmdb.GetSimilarMoviesAsync(movieHits.Results[0].Id, ct);
            suggestions.AddRange(similar.Results.Where(m => !watchedMovies.Contains(m.Id)).Take(6)
                .Select(m => new AiSuggestionDto(m.Id, false, m.Title, m.PosterPath, m.VoteAverage, $"Similar to \"{movieHits.Results[0].Title}\"")));
        }
        if (seriesHits.Results.Count > 0)
        {
            var similar = await tmdb.GetSimilarSeriesAsync(seriesHits.Results[0].Id, ct);
            suggestions.AddRange(similar.Results.Where(s => !watchedSeries.Contains(s.Id)).Take(6)
                .Select(s => new AiSuggestionDto(s.Id, true, s.Name, s.PosterPath, s.VoteAverage, $"Similar to \"{seriesHits.Results[0].Name}\"")));
        }

        var message = suggestions.Count > 0
            ? $"Here's what I found close to \"{title}\" — {suggestions.Count} picks, sorted by how closely they match."
            : $"I couldn't find anything close to \"{title}\" in TMDB. Try a different title or check the spelling.";

        return (suggestions.OrderByDescending(s => s.VoteAverage).Take(8).ToList(), message);
    }

    private async Task<(List<AiSuggestionDto>, string)> SuggestByRuntimeAsync(int maxMinutes, List<int> watchedMovies, CancellationToken ct)
    {
        var trending = await catalog.GetTrendingMoviesAsync(1, ct);
        var suggestions = new List<AiSuggestionDto>();

        foreach (var m in trending.Items.Where(m => !watchedMovies.Contains(m.TmdbId)))
        {
            if (suggestions.Count >= 6) break;
            var detail = await tmdb.GetMovieDetailAsync(m.TmdbId, ct);
            if (detail?.Runtime is { } runtime && runtime <= maxMinutes)
            {
                suggestions.Add(new AiSuggestionDto(m.TmdbId, false, m.Title, m.PosterPath, m.VoteAverage,
                    $"Fits your {maxMinutes}-minute window ({runtime} min)"));
            }
        }

        var message = suggestions.Count > 0
            ? $"You've got {maxMinutes} minutes — here's what fits, trending right now."
            : $"Nothing in today's trending list fits under {maxMinutes} minutes — try Discover with a runtime filter.";

        return (suggestions, message);
    }

    private async Task<(List<AiSuggestionDto>, string)> SuggestByFavoriteGenresAsync(
        Guid userId, List<int> watchedMovies, List<int> watchedSeries, CancellationToken ct)
    {
        var favoriteGenres = await unitOfWork.Repository<MovieWatch>().Query()
            .Where(w => w.UserId == userId && w.IsWatched)
            .SelectMany(w => w.Movie.Genres.Select(g => g.Genre.Name))
            .Concat(unitOfWork.Repository<EpisodeProgress>().Query()
                .Where(p => p.UserId == userId && p.Status == EpisodeWatchStatus.Watched)
                .SelectMany(p => p.Episode.Season.Series.Genres.Select(g => g.Genre.Name)))
            .GroupBy(g => g)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .Take(3)
            .ToListAsync(ct);

        var trendingMovies = await catalog.GetTrendingMoviesAsync(1, ct);
        var trendingSeries = await catalog.GetTrendingSeriesAsync(1, ct);

        var suggestions = new List<AiSuggestionDto>();
        suggestions.AddRange(trendingMovies.Items
            .Where(m => !watchedMovies.Contains(m.TmdbId) && (favoriteGenres.Count == 0 || m.Genres.Any(favoriteGenres.Contains)))
            .Take(4)
            .Select(m => new AiSuggestionDto(m.TmdbId, false, m.Title, m.PosterPath, m.VoteAverage,
                favoriteGenres.Count > 0 ? $"Matches your love of {string.Join(", ", favoriteGenres)}" : "Trending now")));
        suggestions.AddRange(trendingSeries.Items
            .Where(s => !watchedSeries.Contains(s.TmdbId) && (favoriteGenres.Count == 0 || s.Genres.Any(favoriteGenres.Contains)))
            .Take(4)
            .Select(s => new AiSuggestionDto(s.TmdbId, true, s.Title, s.PosterPath, s.VoteAverage,
                favoriteGenres.Count > 0 ? $"Matches your love of {string.Join(", ", favoriteGenres)}" : "Trending now")));

        var message = favoriteGenres.Count > 0
            ? $"Based on your watch history (you're into {string.Join(", ", favoriteGenres)}), here's what I'd queue up next."
            : "Watch a few things first and I'll start tailoring picks to your taste — for now, here's what's trending.";

        return (suggestions.OrderByDescending(s => s.VoteAverage).ToList(), message);
    }

    [GeneratedRegex(@"(?<minutes>\d{2,3})\s*(minutes|mins|min)\b", RegexOptions.IgnoreCase)]
    private static partial Regex RuntimeRegex();

    [GeneratedRegex(@"(like|similar to)\s+(?<title>.+)", RegexOptions.IgnoreCase)]
    private static partial Regex LikeRegex();
}
