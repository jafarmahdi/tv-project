namespace WatchLog.Application.Lists;

public interface IListService
{
    Task<IReadOnlyList<UserListDto>> GetUserListsAsync(Guid userId, CancellationToken ct = default);
    Task<IReadOnlyList<ListItemDto>> GetListItemsAsync(Guid userId, Guid listId, CancellationToken ct = default);
    Task<UserListDto> CreateCustomListAsync(Guid userId, CreateCustomListRequest request, CancellationToken ct = default);
    Task DeleteCustomListAsync(Guid userId, Guid listId, CancellationToken ct = default);
    Task AddItemAsync(Guid userId, Guid listId, AddListItemRequest request, CancellationToken ct = default);
    Task RemoveItemAsync(Guid userId, Guid listId, Guid itemId, CancellationToken ct = default);
}
