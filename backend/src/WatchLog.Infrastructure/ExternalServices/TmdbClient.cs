using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using WatchLog.Application.Common.Interfaces;

namespace WatchLog.Infrastructure.ExternalServices;

/// <summary>
/// TMDB (The Movie Database) API client. Requires <see cref="TmdbOptions.ApiKey"/> — throws
/// <see cref="TmdbNotConfiguredException"/> on every call until it's set, rather than returning fake data.
/// </summary>
public class TmdbClient(HttpClient httpClient, IOptions<TmdbOptions> options) : ITmdbClient
{
    private readonly TmdbOptions _options = options.Value;

    public Task<TmdbPagedResult<TmdbMovieSummary>> SearchMoviesAsync(string query, int page, CancellationToken ct = default) =>
        GetPagedAsync<TmdbWireMovieSummary, TmdbMovieSummary>(
            $"/search/movie?query={Uri.EscapeDataString(query)}&page={page}", MapMovieSummary, ct);

    public Task<TmdbPagedResult<TmdbSeriesSummary>> SearchSeriesAsync(string query, int page, CancellationToken ct = default) =>
        GetPagedAsync<TmdbWireSeriesSummary, TmdbSeriesSummary>(
            $"/search/tv?query={Uri.EscapeDataString(query)}&page={page}", MapSeriesSummary, ct);

    public Task<TmdbPagedResult<TmdbMovieSummary>> GetTrendingMoviesAsync(int page, CancellationToken ct = default) =>
        GetPagedAsync<TmdbWireMovieSummary, TmdbMovieSummary>($"/trending/movie/week?page={page}", MapMovieSummary, ct);

    public Task<TmdbPagedResult<TmdbSeriesSummary>> GetTrendingSeriesAsync(int page, CancellationToken ct = default) =>
        GetPagedAsync<TmdbWireSeriesSummary, TmdbSeriesSummary>($"/trending/tv/week?page={page}", MapSeriesSummary, ct);

    public Task<TmdbPagedResult<TmdbMovieSummary>> DiscoverMoviesByYearAsync(int year, int page, CancellationToken ct = default) =>
        GetPagedAsync<TmdbWireMovieSummary, TmdbMovieSummary>(
            $"/discover/movie?primary_release_year={year}&sort_by=popularity.desc&page={page}", MapMovieSummary, ct);

    public Task<TmdbPagedResult<TmdbSeriesSummary>> DiscoverSeriesByYearAsync(int year, int page, CancellationToken ct = default) =>
        GetPagedAsync<TmdbWireSeriesSummary, TmdbSeriesSummary>(
            $"/discover/tv?first_air_date_year={year}&sort_by=popularity.desc&page={page}", MapSeriesSummary, ct);

    public Task<TmdbPagedResult<TmdbMovieSummary>> GetSimilarMoviesAsync(int tmdbId, CancellationToken ct = default) =>
        GetPagedAsync<TmdbWireMovieSummary, TmdbMovieSummary>($"/movie/{tmdbId}/similar", MapMovieSummary, ct);

    public Task<TmdbPagedResult<TmdbSeriesSummary>> GetSimilarSeriesAsync(int tmdbId, CancellationToken ct = default) =>
        GetPagedAsync<TmdbWireSeriesSummary, TmdbSeriesSummary>($"/tv/{tmdbId}/similar", MapSeriesSummary, ct);

    public async Task<TmdbMovieDetail?> GetMovieDetailAsync(int tmdbId, CancellationToken ct = default)
    {
        var wire = await GetAsync<TmdbWireMovieDetail>($"/movie/{tmdbId}?append_to_response=credits,videos", ct);
        if (wire is null) return null;

        return new TmdbMovieDetail(wire.Id, wire.Title, wire.OriginalTitle, wire.Overview, wire.PosterPath, wire.BackdropPath,
            ParseDate(wire.ReleaseDate), wire.Runtime, wire.VoteAverage, wire.Popularity,
            (wire.Genres ?? []).Select(g => new TmdbGenre(g.Id, g.Name)).ToList(),
            (wire.Credits?.Cast ?? []).Select(c => new TmdbCastMember(c.Id, c.Name, c.Character, c.ProfilePath)).ToList(),
            (wire.Credits?.Crew ?? []).Select(c => new TmdbCrewMember(c.Id, c.Name, c.Job, c.ProfilePath)).ToList(),
            ExtractTrailerKey(wire.Videos));
    }

    public async Task<TmdbSeriesDetail?> GetSeriesDetailAsync(int tmdbId, CancellationToken ct = default)
    {
        var wire = await GetAsync<TmdbWireSeriesDetail>($"/tv/{tmdbId}?append_to_response=credits,videos", ct);
        if (wire is null) return null;

        return new TmdbSeriesDetail(wire.Id, wire.Name, wire.OriginalName, wire.Overview, wire.PosterPath, wire.BackdropPath,
            ParseDate(wire.FirstAirDate), ParseDate(wire.LastAirDate), wire.Status, wire.VoteAverage, wire.Popularity,
            (wire.Genres ?? []).Select(g => new TmdbGenre(g.Id, g.Name)).ToList(),
            (wire.Credits?.Cast ?? []).Select(c => new TmdbCastMember(c.Id, c.Name, c.Character, c.ProfilePath)).ToList(),
            (wire.Credits?.Crew ?? []).Select(c => new TmdbCrewMember(c.Id, c.Name, c.Job, c.ProfilePath)).ToList(),
            (wire.Seasons ?? []).Select(s => new TmdbSeasonSummary(s.SeasonNumber, s.Name, s.Overview, s.PosterPath, ParseDate(s.AirDate), s.EpisodeCount)).ToList(),
            ExtractTrailerKey(wire.Videos));
    }

    public async Task<TmdbSeasonDetail?> GetSeasonDetailAsync(int seriesTmdbId, int seasonNumber, CancellationToken ct = default)
    {
        var wire = await GetAsync<TmdbWireSeasonDetail>($"/tv/{seriesTmdbId}/season/{seasonNumber}", ct);
        if (wire is null) return null;

        return new TmdbSeasonDetail(wire.SeasonNumber, wire.Name, wire.Overview, wire.PosterPath, ParseDate(wire.AirDate),
            (wire.Episodes ?? []).Select(e => new TmdbEpisodeSummary(e.EpisodeNumber, e.Name, e.Overview, e.StillPath, ParseDate(e.AirDate), e.Runtime)).ToList());
    }

    public async Task<IReadOnlyList<TmdbWatchProvider>> GetMovieWatchProvidersAsync(int tmdbId, string region, CancellationToken ct = default) =>
        MapProviders(await GetAsync<TmdbWireProvidersResponse>($"/movie/{tmdbId}/watch/providers", ct), region);

    public async Task<IReadOnlyList<TmdbWatchProvider>> GetSeriesWatchProvidersAsync(int tmdbId, string region, CancellationToken ct = default) =>
        MapProviders(await GetAsync<TmdbWireProvidersResponse>($"/tv/{tmdbId}/watch/providers", ct), region);

    private async Task<TmdbPagedResult<TDto>> GetPagedAsync<TWire, TDto>(
        string path, Func<TWire, TDto> map, CancellationToken ct)
    {
        var wire = await GetAsync<TmdbWirePagedResult<TWire>>(path, ct);
        if (wire is null) return new TmdbPagedResult<TDto>([], 1, 0, 0);
        return new TmdbPagedResult<TDto>(wire.Results.Select(map).ToList(), wire.Page, wire.TotalPages, wire.TotalResults);
    }

    private async Task<T?> GetAsync<T>(string path, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            throw new TmdbNotConfiguredException();
        }

        var separator = path.Contains('?') ? '&' : '?';
        var response = await httpClient.GetAsync($"{path}{separator}api_key={_options.ApiKey}", ct);
        if (!response.IsSuccessStatusCode) return default;

        return await response.Content.ReadFromJsonAsync<T>(cancellationToken: ct);
    }

    private static TmdbMovieSummary MapMovieSummary(TmdbWireMovieSummary w) => new(w.Id, w.Title, w.OriginalTitle, w.Overview,
        w.PosterPath, w.BackdropPath, ParseDate(w.ReleaseDate), w.VoteAverage, w.Popularity, w.GenreIds ?? []);

    private static TmdbSeriesSummary MapSeriesSummary(TmdbWireSeriesSummary w) => new(w.Id, w.Name, w.OriginalName, w.Overview,
        w.PosterPath, w.BackdropPath, ParseDate(w.FirstAirDate), w.VoteAverage, w.Popularity, w.GenreIds ?? []);

    private static string? ExtractTrailerKey(TmdbWireVideos? videos) =>
        videos?.Results?.FirstOrDefault(v => v.Site == "YouTube" && v.Type == "Trailer")?.Key
        ?? videos?.Results?.FirstOrDefault(v => v.Site == "YouTube")?.Key;

    private static IReadOnlyList<TmdbWatchProvider> MapProviders(TmdbWireProvidersResponse? response, string region)
    {
        if (response?.Results is null || !response.Results.TryGetValue(region, out var regionResult)) return [];

        var providers = new List<TmdbWatchProvider>();
        providers.AddRange((regionResult.Flatrate ?? []).Select(p => new TmdbWatchProvider(p.ProviderName, p.LogoPath, "flatrate")));
        providers.AddRange((regionResult.Rent ?? []).Select(p => new TmdbWatchProvider(p.ProviderName, p.LogoPath, "rent")));
        providers.AddRange((regionResult.Buy ?? []).Select(p => new TmdbWatchProvider(p.ProviderName, p.LogoPath, "buy")));
        return providers;
    }

    private static DateOnly? ParseDate(string? date) =>
        !string.IsNullOrWhiteSpace(date) && DateOnly.TryParse(date, out var parsed) ? parsed : null;
}
