using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WatchLog.Application.Common.Interfaces;
using WatchLog.Application.Users;

namespace WatchLog.Api.Controllers;

[Authorize]
public class UsersController(IUserService userService, ICurrentUserService currentUser) : ApiControllerBase(currentUser)
{
    [HttpGet("me")]
    public async Task<ActionResult<MeDto>> GetMe(CancellationToken ct) => Ok(await userService.GetMeAsync(CurrentUserId, ct));

    [HttpPut("me")]
    public async Task<ActionResult<MeDto>> UpdateMe(UpdateProfileRequest request, CancellationToken ct) =>
        Ok(await userService.UpdateProfileAsync(CurrentUserId, request, ct));

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserProfileDto>> GetPublicProfile(Guid id, CancellationToken ct) =>
        Ok(await userService.GetPublicProfileAsync(id, ct));
}
