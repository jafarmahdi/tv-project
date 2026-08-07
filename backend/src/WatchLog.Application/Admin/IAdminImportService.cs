namespace WatchLog.Application.Admin;

/// <summary>Admin-triggered bulk import from TMDB into the local catalog (Postgres), used to
/// pre-populate the app with a given release year's movies/series instead of relying on
/// on-demand caching alone.</summary>
public interface IAdminImportService
{
    Task<ImportResultDto> ImportMoviesByYearAsync(int year, int pages, CancellationToken ct = default);
    Task<ImportResultDto> ImportSeriesByYearAsync(int year, int pages, CancellationToken ct = default);
    Task<ImportedCatalogItemDto> ImportMovieByTmdbIdAsync(int tmdbId, CancellationToken ct = default);
    Task<ImportedCatalogItemDto> ImportSeriesByTmdbIdAsync(int tmdbId, CancellationToken ct = default);
    Task<ImportedCatalogItemDto> ImportEpisodeAsync(int seriesTmdbId, int seasonNumber, int episodeNumber, CancellationToken ct = default);
}

public record ImportResultDto(int Year, int PagesRequested, int ItemsDiscovered, int ItemsImported, IReadOnlyList<string> Errors);
public record ImportedCatalogItemDto(Guid LocalId, string EntityType, string Title, string Reference);
public record ImportEpisodeRequest(int SeriesTmdbId, int SeasonNumber, int EpisodeNumber);
