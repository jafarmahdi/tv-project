using Microsoft.EntityFrameworkCore;
using WatchLog.Application.Catalog;
using WatchLog.Application.Common;
using WatchLog.Application.Common.Interfaces;
using WatchLog.Domain.Entities;

namespace WatchLog.Application.Collections;

public class CollectionService(ICatalogService catalog, IUnitOfWork unitOfWork) : ICollectionService
{
    public async Task<IReadOnlyList<CollectionSummaryDto>> GetCuratedAsync(CancellationToken ct = default) =>
        await unitOfWork.Repository<Collection>().Query()
            .Where(c => c.IsCurated)
            .Select(c => new CollectionSummaryDto(c.Id, c.Name, c.Description, c.PosterUrl, c.IsCurated, c.Items.Count))
            .ToListAsync(ct);

    public async Task<IReadOnlyList<CollectionSummaryDto>> GetUserCollectionsAsync(Guid userId, CancellationToken ct = default) =>
        await unitOfWork.Repository<Collection>().Query()
            .Where(c => c.CreatedByUserId == userId)
            .Select(c => new CollectionSummaryDto(c.Id, c.Name, c.Description, c.PosterUrl, c.IsCurated, c.Items.Count))
            .ToListAsync(ct);

    public async Task<CollectionDetailDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var collection = await unitOfWork.Repository<Collection>().Query()
            .Include(c => c.Items).ThenInclude(i => i.Movie)
            .Include(c => c.Items).ThenInclude(i => i.Series)
            .FirstOrDefaultAsync(c => c.Id == id, ct)
            ?? throw new NotFoundException(nameof(Collection), id);

        var items = collection.Items.OrderBy(i => i.SortOrder).Select(i => new CollectionItemDto(
            i.Movie?.TmdbId, i.Movie?.Title, i.Movie?.PosterPath,
            i.Series?.TmdbId, i.Series?.Title, i.Series?.PosterPath)).ToList();

        return new CollectionDetailDto(collection.Id, collection.Name, collection.Description, collection.PosterUrl, collection.IsCurated, items);
    }

    public async Task<CollectionSummaryDto> CreateAsync(Guid userId, CreateCollectionRequest request, CancellationToken ct = default)
    {
        var collection = new Collection { Name = request.Name, Description = request.Description, CreatedByUserId = userId, IsCurated = false };
        await unitOfWork.Repository<Collection>().AddAsync(collection, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return new CollectionSummaryDto(collection.Id, collection.Name, collection.Description, collection.PosterUrl, false, 0);
    }

    public async Task AddItemAsync(Guid userId, Guid collectionId, AddCollectionItemRequest request, CancellationToken ct = default)
    {
        var collection = await unitOfWork.Repository<Collection>().Query().FirstOrDefaultAsync(c => c.Id == collectionId, ct)
            ?? throw new NotFoundException(nameof(Collection), collectionId);
        if (collection.CreatedByUserId != userId)
        {
            throw new ForbiddenException("This collection does not belong to you.");
        }

        var item = new CollectionItem { CollectionId = collectionId };
        if (request.MovieTmdbId is { } movieTmdbId) item.MovieId = await catalog.EnsureMovieCachedAsync(movieTmdbId, ct);
        if (request.SeriesTmdbId is { } seriesTmdbId) item.SeriesId = await catalog.EnsureSeriesCachedAsync(seriesTmdbId, ct);

        await unitOfWork.Repository<CollectionItem>().AddAsync(item, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
