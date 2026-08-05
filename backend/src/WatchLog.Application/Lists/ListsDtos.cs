using WatchLog.Domain.Enums;

namespace WatchLog.Application.Lists;

public record UserListDto(Guid Id, string Name, ListType Type, bool IsPublic, int ItemCount);

public record ListItemDto(Guid Id, int? MovieTmdbId, string? MovieTitle, string? MoviePosterPath,
    int? SeriesTmdbId, string? SeriesTitle, string? SeriesPosterPath, DateTimeOffset AddedAt);

public record CreateCustomListRequest(string Name, bool IsPublic = true);
public record AddListItemRequest(int? MovieTmdbId = null, int? SeriesTmdbId = null);
