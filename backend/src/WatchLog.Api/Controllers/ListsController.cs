using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WatchLog.Application.Common.Interfaces;
using WatchLog.Application.Lists;

namespace WatchLog.Api.Controllers;

[Authorize]
public class ListsController(IListService listService, ICurrentUserService currentUser) : ApiControllerBase(currentUser)
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UserListDto>>> GetMyLists(CancellationToken ct) =>
        Ok(await listService.GetUserListsAsync(CurrentUserId, ct));

    [HttpGet("{listId:guid}/items")]
    public async Task<ActionResult<IReadOnlyList<ListItemDto>>> GetItems(Guid listId, CancellationToken ct) =>
        Ok(await listService.GetListItemsAsync(CurrentUserId, listId, ct));

    [HttpPost]
    public async Task<ActionResult<UserListDto>> CreateCustomList(CreateCustomListRequest request, CancellationToken ct) =>
        Ok(await listService.CreateCustomListAsync(CurrentUserId, request, ct));

    [HttpDelete("{listId:guid}")]
    public async Task<IActionResult> Delete(Guid listId, CancellationToken ct)
    {
        await listService.DeleteCustomListAsync(CurrentUserId, listId, ct);
        return NoContent();
    }

    [HttpPost("{listId:guid}/items")]
    public async Task<IActionResult> AddItem(Guid listId, AddListItemRequest request, CancellationToken ct)
    {
        await listService.AddItemAsync(CurrentUserId, listId, request, ct);
        return NoContent();
    }

    [HttpDelete("{listId:guid}/items/{itemId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid listId, Guid itemId, CancellationToken ct)
    {
        await listService.RemoveItemAsync(CurrentUserId, listId, itemId, ct);
        return NoContent();
    }
}
