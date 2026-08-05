using Microsoft.EntityFrameworkCore;
using WatchLog.Application.Common;
using WatchLog.Application.Common.Interfaces;
using WatchLog.Application.Common.Models;
using WatchLog.Domain.Entities;

namespace WatchLog.Application.Catalog;

public class CatalogService(ITmdbClient tmdb, ICacheService cache, IUnitOfWork unitOfWork) : ICatalogService
{
    private static readonly TimeSpan ListTtl = TimeSpan.FromMinutes(30);
    private static readonly TimeSpan DetailCacheStaleAfter = TimeSpan.FromHours(12);

    public async Task<PagedResult<MovieSummaryDto>> SearchMoviesAsync(string query, int page, CancellationToken ct = default)
    {
        var result = await cache.GetOrCreateAsync($"tmdb:search:movie:{query}:{page}", ListTtl,
            () => tmdb.SearchMoviesAsync(query, page, ct), ct);
        return await ToPagedAsync(result, MapMovieSummaryAsync, ct);
    }

    public async Task<PagedResult<SeriesSummaryDto>> SearchSeriesAsync(string query, int page, CancellationToken ct = default)
    {
        var result = await cache.GetOrCreateAsync($"tmdb:search:series:{query}:{page}", ListTtl,
            () => tmdb.SearchSeriesAsync(query, page, ct), ct);
        return await ToPagedAsync(result, MapSeriesSummaryAsync, ct);
    }

    public async Task<PagedResult<MovieSummaryDto>> GetTrendingMoviesAsync(int page, CancellationToken ct = default)
    {
        var result = await cache.GetOrCreateAsync($"tmdb:trending:movie:{page}", ListTtl,
            () => tmdb.GetTrendingMoviesAsync(page, ct), ct);
        return await ToPagedAsync(result, MapMovieSummaryAsync, ct);
    }

    public async Task<PagedResult<SeriesSummaryDto>> GetTrendingSeriesAsync(int page, CancellationToken ct = default)
    {
        var result = await cache.GetOrCreateAsync($"tmdb:trending:series:{page}", ListTtl,
            () => tmdb.GetTrendingSeriesAsync(page, ct), ct);
        return await ToPagedAsync(result, MapSeriesSummaryAsync, ct);
    }

    public async Task<MovieDetailDto> GetMovieDetailAsync(int tmdbId, CancellationToken ct = default)
    {
        var detail = await cache.GetOrCreateAsync($"tmdb:movie:{tmdbId}", DetailCacheStaleAfter,
            async () => await tmdb.GetMovieDetailAsync(tmdbId, ct)
                ?? throw new NotFoundException(nameof(Movie), tmdbId), ct);

        await UpsertMovieAsync(detail, ct);

        return new MovieDetailDto(detail.Id, detail.Title, detail.OriginalTitle, detail.Overview, detail.PosterPath,
            detail.BackdropPath, detail.ReleaseDate, detail.Runtime, detail.VoteAverage,
            detail.Genres.Select(g => g.Name).ToList(),
            detail.Cast.Select(c => new CastMemberDto(c.Id, c.Name, c.Character, c.ProfilePath)).ToList(),
            detail.Crew.Select(c => new CrewMemberDto(c.Id, c.Name, c.Job, c.ProfilePath)).ToList(),
            detail.TrailerYoutubeKey);
    }

    public async Task<SeriesDetailDto> GetSeriesDetailAsync(int tmdbId, CancellationToken ct = default)
    {
        var detail = await cache.GetOrCreateAsync($"tmdb:series:{tmdbId}", DetailCacheStaleAfter,
            async () => await tmdb.GetSeriesDetailAsync(tmdbId, ct)
                ?? throw new NotFoundException(nameof(Series), tmdbId), ct);

        await UpsertSeriesAsync(detail, ct);

        return new SeriesDetailDto(detail.Id, detail.Name, detail.OriginalName, detail.Overview, detail.PosterPath,
            detail.BackdropPath, detail.FirstAirDate, detail.LastAirDate, detail.Status, detail.VoteAverage,
            detail.Genres.Select(g => g.Name).ToList(),
            detail.Cast.Select(c => new CastMemberDto(c.Id, c.Name, c.Character, c.ProfilePath)).ToList(),
            detail.Crew.Select(c => new CrewMemberDto(c.Id, c.Name, c.Job, c.ProfilePath)).ToList(),
            detail.Seasons.Select(s => new SeasonSummaryDto(s.SeasonNumber, s.Name, s.PosterPath, s.AirDate, s.EpisodeCount)).ToList(),
            detail.TrailerYoutubeKey);
    }

    public async Task<SeasonDetailDto> GetSeasonAsync(int seriesTmdbId, int seasonNumber, CancellationToken ct = default)
    {
        var detail = await cache.GetOrCreateAsync($"tmdb:season:{seriesTmdbId}:{seasonNumber}", DetailCacheStaleAfter,
            async () => await tmdb.GetSeasonDetailAsync(seriesTmdbId, seasonNumber, ct)
                ?? throw new NotFoundException(nameof(Season), $"{seriesTmdbId}/{seasonNumber}"), ct);

        return new SeasonDetailDto(detail.SeasonNumber, detail.Name, detail.Overview, detail.PosterPath, detail.AirDate,
            detail.Episodes.Select(e => new EpisodeSummaryDto(e.EpisodeNumber, e.Name, e.Overview, e.StillPath, e.AirDate, e.Runtime)).ToList());
    }

    public async Task<PagedResult<MovieSummaryDto>> GetSimilarMoviesAsync(int tmdbId, CancellationToken ct = default)
    {
        var result = await cache.GetOrCreateAsync($"tmdb:similar:movie:{tmdbId}", ListTtl,
            () => tmdb.GetSimilarMoviesAsync(tmdbId, ct), ct);
        return await ToPagedAsync(result, MapMovieSummaryAsync, ct);
    }

    public async Task<PagedResult<SeriesSummaryDto>> GetSimilarSeriesAsync(int tmdbId, CancellationToken ct = default)
    {
        var result = await cache.GetOrCreateAsync($"tmdb:similar:series:{tmdbId}", ListTtl,
            () => tmdb.GetSimilarSeriesAsync(tmdbId, ct), ct);
        return await ToPagedAsync(result, MapSeriesSummaryAsync, ct);
    }

    public async Task<IReadOnlyList<WatchProviderDto>> GetMovieWatchProvidersAsync(int tmdbId, string region, CancellationToken ct = default)
    {
        var providers = await cache.GetOrCreateAsync($"tmdb:providers:movie:{tmdbId}:{region}", ListTtl,
            () => tmdb.GetMovieWatchProvidersAsync(tmdbId, region, ct), ct);
        return providers.Select(p => new WatchProviderDto(p.ProviderName, p.LogoPath, p.Type)).ToList();
    }

    public async Task<IReadOnlyList<WatchProviderDto>> GetSeriesWatchProvidersAsync(int tmdbId, string region, CancellationToken ct = default)
    {
        var providers = await cache.GetOrCreateAsync($"tmdb:providers:series:{tmdbId}:{region}", ListTtl,
            () => tmdb.GetSeriesWatchProvidersAsync(tmdbId, region, ct), ct);
        return providers.Select(p => new WatchProviderDto(p.ProviderName, p.LogoPath, p.Type)).ToList();
    }

    public async Task<Guid> EnsureMovieCachedAsync(int tmdbId, CancellationToken ct = default)
    {
        var repo = unitOfWork.Repository<Movie>();
        var existing = await repo.Query().FirstOrDefaultAsync(m => m.TmdbId == tmdbId, ct);
        if (existing is not null && DateTimeOffset.UtcNow - existing.CreatedAt < DetailCacheStaleAfter)
        {
            return existing.Id;
        }

        var detail = await tmdb.GetMovieDetailAsync(tmdbId, ct) ?? throw new NotFoundException(nameof(Movie), tmdbId);
        return await UpsertMovieAsync(detail, ct);
    }

    public async Task<Guid> EnsureSeriesCachedAsync(int tmdbId, CancellationToken ct = default)
    {
        var repo = unitOfWork.Repository<Series>();
        var existing = await repo.Query().FirstOrDefaultAsync(s => s.TmdbId == tmdbId, ct);
        if (existing is not null && DateTimeOffset.UtcNow - existing.CreatedAt < DetailCacheStaleAfter)
        {
            return existing.Id;
        }

        var detail = await tmdb.GetSeriesDetailAsync(tmdbId, ct) ?? throw new NotFoundException(nameof(Series), tmdbId);
        return await UpsertSeriesAsync(detail, ct);
    }

    public async Task<Guid> EnsureEpisodeCachedAsync(int seriesTmdbId, int seasonNumber, int episodeNumber, CancellationToken ct = default)
    {
        var seriesId = await EnsureSeriesCachedAsync(seriesTmdbId, ct);

        var seasonRepo = unitOfWork.Repository<Season>();
        var season = await seasonRepo.Query()
            .FirstOrDefaultAsync(s => s.SeriesId == seriesId && s.SeasonNumber == seasonNumber, ct);

        if (season is null)
        {
            throw new NotFoundException(nameof(Season), $"{seriesTmdbId}/{seasonNumber}");
        }

        var episodeRepo = unitOfWork.Repository<Episode>();
        var episode = await episodeRepo.Query()
            .FirstOrDefaultAsync(e => e.SeasonId == season.Id && e.EpisodeNumber == episodeNumber, ct);

        if (episode is not null) return episode.Id;

        var seasonDetail = await tmdb.GetSeasonDetailAsync(seriesTmdbId, seasonNumber, ct)
            ?? throw new NotFoundException(nameof(Season), $"{seriesTmdbId}/{seasonNumber}");
        var tmdbEpisode = seasonDetail.Episodes.FirstOrDefault(e => e.EpisodeNumber == episodeNumber)
            ?? throw new NotFoundException(nameof(Episode), episodeNumber);

        var newEpisode = new Episode
        {
            SeasonId = season.Id,
            EpisodeNumber = tmdbEpisode.EpisodeNumber,
            Title = tmdbEpisode.Name,
            Overview = tmdbEpisode.Overview,
            StillPath = tmdbEpisode.StillPath,
            AirDate = tmdbEpisode.AirDate,
            RuntimeMinutes = tmdbEpisode.Runtime
        };
        await episodeRepo.AddAsync(newEpisode, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return newEpisode.Id;
    }

    private async Task<Guid> UpsertMovieAsync(TmdbMovieDetail detail, CancellationToken ct)
    {
        var repo = unitOfWork.Repository<Movie>();
        var movie = await repo.Query().Include(m => m.Genres).FirstOrDefaultAsync(m => m.TmdbId == detail.Id, ct);
        var isNew = movie is null;
        movie ??= new Movie { TmdbId = detail.Id };

        movie.Title = detail.Title;
        movie.OriginalTitle = detail.OriginalTitle;
        movie.Overview = detail.Overview;
        movie.PosterPath = detail.PosterPath;
        movie.BackdropPath = detail.BackdropPath;
        movie.ReleaseDate = detail.ReleaseDate;
        movie.RuntimeMinutes = detail.Runtime;
        movie.VoteAverage = detail.VoteAverage;
        movie.Popularity = detail.Popularity;
        movie.TrailerYoutubeKey = detail.TrailerYoutubeKey;
        movie.UpdatedAt = DateTimeOffset.UtcNow;

        var genreIds = await EnsureGenresAsync(detail.Genres, ct);
        movie.Genres = genreIds.Select(id => new MovieGenre { MovieId = movie.Id, GenreId = id }).ToList();

        if (isNew) await repo.AddAsync(movie, ct);
        else repo.Update(movie);

        await unitOfWork.SaveChangesAsync(ct);
        return movie.Id;
    }

    private async Task<Guid> UpsertSeriesAsync(TmdbSeriesDetail detail, CancellationToken ct)
    {
        var repo = unitOfWork.Repository<Series>();
        var series = await repo.Query().Include(s => s.Genres).Include(s => s.Seasons)
            .FirstOrDefaultAsync(s => s.TmdbId == detail.Id, ct);
        var isNew = series is null;
        series ??= new Series { TmdbId = detail.Id };

        series.Title = detail.Name;
        series.OriginalTitle = detail.OriginalName;
        series.Overview = detail.Overview;
        series.PosterPath = detail.PosterPath;
        series.BackdropPath = detail.BackdropPath;
        series.FirstAirDate = detail.FirstAirDate;
        series.LastAirDate = detail.LastAirDate;
        series.Status = MapSeriesStatus(detail.Status);
        series.VoteAverage = detail.VoteAverage;
        series.Popularity = detail.Popularity;
        series.TrailerYoutubeKey = detail.TrailerYoutubeKey;
        series.UpdatedAt = DateTimeOffset.UtcNow;

        var genreIds = await EnsureGenresAsync(detail.Genres, ct);
        series.Genres = genreIds.Select(id => new SeriesGenre { SeriesId = series.Id, GenreId = id }).ToList();

        if (isNew) await repo.AddAsync(series, ct);
        else repo.Update(series);
        await unitOfWork.SaveChangesAsync(ct);

        var seasonRepo = unitOfWork.Repository<Season>();
        foreach (var s in detail.Seasons)
        {
            var season = series.Seasons.FirstOrDefault(x => x.SeasonNumber == s.SeasonNumber);
            if (season is null)
            {
                await seasonRepo.AddAsync(new Season
                {
                    SeriesId = series.Id,
                    SeasonNumber = s.SeasonNumber,
                    Name = s.Name,
                    Overview = s.Overview,
                    PosterPath = s.PosterPath,
                    AirDate = s.AirDate
                }, ct);
            }
        }
        await unitOfWork.SaveChangesAsync(ct);

        return series.Id;
    }

    private async Task<List<Guid>> EnsureGenresAsync(IReadOnlyList<TmdbGenre> tmdbGenres, CancellationToken ct)
    {
        var repo = unitOfWork.Repository<Genre>();
        var ids = new List<Guid>();
        foreach (var g in tmdbGenres)
        {
            var genre = await repo.Query().FirstOrDefaultAsync(x => x.TmdbId == g.Id, ct);
            if (genre is null)
            {
                genre = new Genre { TmdbId = g.Id, Name = g.Name };
                await repo.AddAsync(genre, ct);
                await unitOfWork.SaveChangesAsync(ct);
            }
            ids.Add(genre.Id);
        }
        return ids;
    }

    private async Task<IReadOnlyList<string>> ResolveGenreNamesAsync(IReadOnlyList<int> tmdbGenreIds, CancellationToken ct)
    {
        if (tmdbGenreIds.Count == 0) return [];
        return await unitOfWork.Repository<Genre>().Query()
            .Where(g => tmdbGenreIds.Contains(g.TmdbId))
            .Select(g => g.Name)
            .ToListAsync(ct);
    }

    private async Task<MovieSummaryDto> MapMovieSummaryAsync(TmdbMovieSummary m, CancellationToken ct) =>
        new(m.Id, m.Title, m.PosterPath, m.BackdropPath, m.ReleaseDate, m.VoteAverage, await ResolveGenreNamesAsync(m.GenreIds, ct));

    private async Task<SeriesSummaryDto> MapSeriesSummaryAsync(TmdbSeriesSummary s, CancellationToken ct) =>
        new(s.Id, s.Name, s.PosterPath, s.BackdropPath, s.FirstAirDate, s.VoteAverage, await ResolveGenreNamesAsync(s.GenreIds, ct));

    private static async Task<PagedResult<TDto>> ToPagedAsync<TSource, TDto>(
        TmdbPagedResult<TSource> source, Func<TSource, CancellationToken, Task<TDto>> map, CancellationToken ct)
    {
        var items = new List<TDto>(source.Results.Count);
        foreach (var item in source.Results) items.Add(await map(item, ct));
        return PagedResult<TDto>.Create(items, source.Page, items.Count, source.TotalResults);
    }

    private static Domain.Enums.SeriesStatus MapSeriesStatus(string status) => status switch
    {
        "Ended" => Domain.Enums.SeriesStatus.Ended,
        "Canceled" or "Cancelled" => Domain.Enums.SeriesStatus.Cancelled,
        "In Production" => Domain.Enums.SeriesStatus.InProduction,
        "Planned" => Domain.Enums.SeriesStatus.Planned,
        _ => Domain.Enums.SeriesStatus.ReturningSeries
    };
}
