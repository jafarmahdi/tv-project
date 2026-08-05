namespace WatchLog.Application.Collections;

public record CollectionSummaryDto(Guid Id, string Name, string? Description, string? PosterUrl, bool IsCurated, int ItemCount);
public record CollectionItemDto(int? MovieTmdbId, string? MovieTitle, string? MoviePosterPath, int? SeriesTmdbId, string? SeriesTitle, string? SeriesPosterPath);
public record CollectionDetailDto(Guid Id, string Name, string? Description, string? PosterUrl, bool IsCurated, IReadOnlyList<CollectionItemDto> Items);
public record CreateCollectionRequest(string Name, string? Description);
public record AddCollectionItemRequest(int? MovieTmdbId, int? SeriesTmdbId);
