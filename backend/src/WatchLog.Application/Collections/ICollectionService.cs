namespace WatchLog.Application.Collections;

public interface ICollectionService
{
    Task<IReadOnlyList<CollectionSummaryDto>> GetCuratedAsync(CancellationToken ct = default);
    Task<IReadOnlyList<CollectionSummaryDto>> GetUserCollectionsAsync(Guid userId, CancellationToken ct = default);
    Task<CollectionDetailDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<CollectionSummaryDto> CreateAsync(Guid userId, CreateCollectionRequest request, CancellationToken ct = default);
    Task AddItemAsync(Guid userId, Guid collectionId, AddCollectionItemRequest request, CancellationToken ct = default);
}
