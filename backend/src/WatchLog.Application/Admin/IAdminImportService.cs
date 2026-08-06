namespace WatchLog.Application.Admin;

/// <summary>Admin-triggered bulk import from TMDB into the local catalog (Postgres), used to
/// pre-populate the app with a given release year's movies/series instead of relying on
/// on-demand caching alone.</summary>
public interface IAdminImportService
{
    Task<ImportResultDto> ImportMoviesByYearAsync(int year, int pages, CancellationToken ct = default);
    Task<ImportResultDto> ImportSeriesByYearAsync(int year, int pages, CancellationToken ct = default);
}

public record ImportResultDto(int Year, int PagesRequested, int ItemsDiscovered, int ItemsImported, IReadOnlyList<string> Errors);
