using WatchLog.Domain.Entities;

namespace WatchLog.Infrastructure.Persistence;

/// <summary>
/// Deterministic seed data for `HasData`. Guids are derived from a stable index rather than
/// `Guid.NewGuid()` — EF Core snapshots seed data into migrations, so the keys must never change
/// between runs or every subsequent migration would show a spurious diff.
/// </summary>
internal static class SeedData
{
    private static Guid GenreId(int tmdbId) => new($"00000000-0000-4000-8000-{tmdbId:D12}");
    private static Guid AchievementId(int index) => new($"10000000-0000-4000-8000-{index:D12}");
    private static readonly DateTimeOffset SeededAt = new(2026, 8, 5, 0, 0, 0, TimeSpan.Zero);

    /// <summary>TMDB's official movie + TV genre lists (stable, public reference data — themoviedb.org/genre).</summary>
    public static readonly Genre[] Genres =
    [
        new() { Id = GenreId(28), TmdbId = 28, Name = "Action" },
        new() { Id = GenreId(12), TmdbId = 12, Name = "Adventure" },
        new() { Id = GenreId(16), TmdbId = 16, Name = "Animation" },
        new() { Id = GenreId(35), TmdbId = 35, Name = "Comedy" },
        new() { Id = GenreId(80), TmdbId = 80, Name = "Crime" },
        new() { Id = GenreId(99), TmdbId = 99, Name = "Documentary" },
        new() { Id = GenreId(18), TmdbId = 18, Name = "Drama" },
        new() { Id = GenreId(10751), TmdbId = 10751, Name = "Family" },
        new() { Id = GenreId(14), TmdbId = 14, Name = "Fantasy" },
        new() { Id = GenreId(36), TmdbId = 36, Name = "History" },
        new() { Id = GenreId(27), TmdbId = 27, Name = "Horror" },
        new() { Id = GenreId(10402), TmdbId = 10402, Name = "Music" },
        new() { Id = GenreId(9648), TmdbId = 9648, Name = "Mystery" },
        new() { Id = GenreId(10749), TmdbId = 10749, Name = "Romance" },
        new() { Id = GenreId(878), TmdbId = 878, Name = "Science Fiction" },
        new() { Id = GenreId(10770), TmdbId = 10770, Name = "TV Movie" },
        new() { Id = GenreId(53), TmdbId = 53, Name = "Thriller" },
        new() { Id = GenreId(10752), TmdbId = 10752, Name = "War" },
        new() { Id = GenreId(37), TmdbId = 37, Name = "Western" },
        new() { Id = GenreId(10759), TmdbId = 10759, Name = "Action & Adventure" },
        new() { Id = GenreId(10762), TmdbId = 10762, Name = "Kids" },
        new() { Id = GenreId(10763), TmdbId = 10763, Name = "News" },
        new() { Id = GenreId(10764), TmdbId = 10764, Name = "Reality" },
        new() { Id = GenreId(10765), TmdbId = 10765, Name = "Sci-Fi & Fantasy" },
        new() { Id = GenreId(10766), TmdbId = 10766, Name = "Soap" },
        new() { Id = GenreId(10767), TmdbId = 10767, Name = "Talk" },
        new() { Id = GenreId(10768), TmdbId = 10768, Name = "War & Politics" }
    ];

    /// <summary>Codes here must match <c>WatchLog.Application.Achievements.AchievementService.Criteria</c>.</summary>
    public static readonly Achievement[] Achievements =
    [
        new() { Id = AchievementId(1), Code = "first-watch", Name = "First Watch", Description = "Watched your first episode or movie.", CriteriaDescription = "Watch 1 episode or movie.", CreatedAt = SeededAt },
        new() { Id = AchievementId(2), Code = "episodes-50", Name = "Binge Starter", Description = "Watched 50 episodes.", CriteriaDescription = "Watch 50 episodes.", CreatedAt = SeededAt },
        new() { Id = AchievementId(3), Code = "episodes-100", Name = "Century Club", Description = "Watched 100 episodes.", CriteriaDescription = "Watch 100 episodes.", CreatedAt = SeededAt },
        new() { Id = AchievementId(4), Code = "episodes-500", Name = "Marathoner", Description = "Watched 500 episodes.", CriteriaDescription = "Watch 500 episodes.", CreatedAt = SeededAt },
        new() { Id = AchievementId(5), Code = "movies-25", Name = "Film Buff", Description = "Watched 25 movies.", CriteriaDescription = "Watch 25 movies.", CreatedAt = SeededAt },
        new() { Id = AchievementId(6), Code = "movies-100", Name = "Cinephile", Description = "Watched 100 movies.", CriteriaDescription = "Watch 100 movies.", CreatedAt = SeededAt }
    ];
}
