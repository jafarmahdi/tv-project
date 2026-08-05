namespace WatchLog.Application.Stats;

public record MonthlyActivityDto(int Year, int Month, int EpisodesWatched, int MoviesWatched);
public record GenreStatDto(string Genre, int Count);
public record HeatmapDayDto(DateOnly Date, int Count);
public record BadgeDto(string Code, string Name, string Description, string? IconUrl, DateTimeOffset EarnedAt);

public record UserStatsDto(
    int TotalEpisodesWatched,
    int TotalMoviesWatched,
    int TotalWatchTimeMinutes,
    IReadOnlyList<MonthlyActivityDto> MonthlyActivity,
    IReadOnlyList<GenreStatDto> FavoriteGenres,
    IReadOnlyList<HeatmapDayDto> HeatmapCalendar,
    IReadOnlyList<BadgeDto> Achievements);
