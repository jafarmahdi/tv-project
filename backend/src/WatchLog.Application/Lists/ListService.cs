using Microsoft.EntityFrameworkCore;
using WatchLog.Application.Catalog;
using WatchLog.Application.Common;
using WatchLog.Application.Common.Interfaces;
using WatchLog.Domain.Entities;
using WatchLog.Domain.Enums;

namespace WatchLog.Application.Lists;

public class ListService(ICatalogService catalog, IUnitOfWork unitOfWork) : IListService
{
    public async Task<IReadOnlyList<UserListDto>> GetUserListsAsync(Guid userId, CancellationToken ct = default)
    {
        return await unitOfWork.Repository<UserList>().Query()
            .Where(l => l.UserId == userId)
            .Select(l => new UserListDto(l.Id, l.Name, l.Type, l.IsPublic, l.Items.Count))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<ListItemDto>> GetListItemsAsync(Guid userId, Guid listId, CancellationToken ct = default)
    {
        await EnsureOwnedAsync(userId, listId, ct);

        return await unitOfWork.Repository<ListItem>().Query()
            .Where(i => i.ListId == listId)
            .Include(i => i.Movie)
            .Include(i => i.Series)
            .OrderBy(i => i.SortOrder)
            .Select(i => new ListItemDto(i.Id,
                i.Movie != null ? i.Movie.TmdbId : null, i.Movie != null ? i.Movie.Title : null, i.Movie != null ? i.Movie.PosterPath : null,
                i.Series != null ? i.Series.TmdbId : null, i.Series != null ? i.Series.Title : null, i.Series != null ? i.Series.PosterPath : null,
                i.AddedAt))
            .ToListAsync(ct);
    }

    public async Task<UserListDto> CreateCustomListAsync(Guid userId, CreateCustomListRequest request, CancellationToken ct = default)
    {
        var repo = unitOfWork.Repository<UserList>();
        var list = new UserList { UserId = userId, Name = request.Name, Type = ListType.Custom, IsPublic = request.IsPublic };
        await repo.AddAsync(list, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return new UserListDto(list.Id, list.Name, list.Type, list.IsPublic, 0);
    }

    public async Task DeleteCustomListAsync(Guid userId, Guid listId, CancellationToken ct = default)
    {
        var list = await EnsureOwnedAsync(userId, listId, ct);
        if (list.Type != ListType.Custom)
        {
            throw new ConflictException("Built-in lists cannot be deleted.");
        }

        unitOfWork.Repository<UserList>().Remove(list);
        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task AddItemAsync(Guid userId, Guid listId, AddListItemRequest request, CancellationToken ct = default)
    {
        await EnsureOwnedAsync(userId, listId, ct);

        if (request.MovieTmdbId is null && request.SeriesTmdbId is null)
        {
            throw new ConflictException("Either movieTmdbId or seriesTmdbId must be provided.");
        }

        var repo = unitOfWork.Repository<ListItem>();
        var item = new ListItem { ListId = listId };

        if (request.MovieTmdbId is { } movieTmdbId)
        {
            item.MovieId = await catalog.EnsureMovieCachedAsync(movieTmdbId, ct);
        }
        if (request.SeriesTmdbId is { } seriesTmdbId)
        {
            item.SeriesId = await catalog.EnsureSeriesCachedAsync(seriesTmdbId, ct);
        }

        await repo.AddAsync(item, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task RemoveItemAsync(Guid userId, Guid listId, Guid itemId, CancellationToken ct = default)
    {
        await EnsureOwnedAsync(userId, listId, ct);

        var repo = unitOfWork.Repository<ListItem>();
        var item = await repo.Query().FirstOrDefaultAsync(i => i.Id == itemId && i.ListId == listId, ct)
            ?? throw new NotFoundException(nameof(ListItem), itemId);

        repo.Remove(item);
        await unitOfWork.SaveChangesAsync(ct);
    }

    private async Task<UserList> EnsureOwnedAsync(Guid userId, Guid listId, CancellationToken ct)
    {
        var list = await unitOfWork.Repository<UserList>().Query().FirstOrDefaultAsync(l => l.Id == listId, ct)
            ?? throw new NotFoundException(nameof(UserList), listId);
        if (list.UserId != userId)
        {
            throw new ForbiddenException("This list does not belong to you.");
        }
        return list;
    }
}
