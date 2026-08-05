using Microsoft.EntityFrameworkCore;
using WatchLog.Application.Common;
using WatchLog.Application.Common.Interfaces;
using WatchLog.Domain.Entities;
using WatchLog.Domain.Enums;

namespace WatchLog.Application.Social;

public class SocialService(IUnitOfWork unitOfWork, IIdentityService identityService) : ISocialService
{
    public async Task FollowAsync(Guid followerId, Guid targetUserId, CancellationToken ct = default)
    {
        if (followerId == targetUserId) throw new ConflictException("You cannot follow yourself.");

        var repo = unitOfWork.Repository<Follow>();
        var exists = await repo.Query().AnyAsync(f => f.FollowerId == followerId && f.FollowingId == targetUserId, ct);
        if (exists) return;

        await repo.AddAsync(new Follow { FollowerId = followerId, FollowingId = targetUserId }, ct);
        await unitOfWork.Repository<ActivityFeedEntry>().AddAsync(new ActivityFeedEntry
        {
            UserId = followerId,
            Type = ActivityType.FollowedUser,
            TargetType = TargetType.User,
            TargetId = targetUserId
        }, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task UnfollowAsync(Guid followerId, Guid targetUserId, CancellationToken ct = default)
    {
        var repo = unitOfWork.Repository<Follow>();
        var follow = await repo.Query().FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == targetUserId, ct);
        if (follow is null) return;

        repo.Remove(follow);
        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<FollowSummaryDto>> GetFollowersAsync(Guid userId, CancellationToken ct = default)
    {
        var followerIds = await unitOfWork.Repository<Follow>().Query()
            .Where(f => f.FollowingId == userId).Select(f => f.FollowerId).ToListAsync(ct);
        var iFollow = await unitOfWork.Repository<Follow>().Query()
            .Where(f => f.FollowerId == userId).Select(f => f.FollowingId).ToListAsync(ct);

        var users = await identityService.GetUsersAsync(followerIds);
        return followerIds.Where(users.ContainsKey)
            .Select(id => new FollowSummaryDto(id, users[id].DisplayName, users[id].AvatarUrl, iFollow.Contains(id)))
            .ToList();
    }

    public async Task<IReadOnlyList<FollowSummaryDto>> GetFollowingAsync(Guid userId, CancellationToken ct = default)
    {
        var followingIds = await unitOfWork.Repository<Follow>().Query()
            .Where(f => f.FollowerId == userId).Select(f => f.FollowingId).ToListAsync(ct);

        var users = await identityService.GetUsersAsync(followingIds);
        return followingIds.Where(users.ContainsKey)
            .Select(id => new FollowSummaryDto(id, users[id].DisplayName, users[id].AvatarUrl, true))
            .ToList();
    }

    public async Task<IReadOnlyList<ActivityFeedItemDto>> GetFeedAsync(Guid userId, int page, int pageSize, CancellationToken ct = default)
    {
        var followingIds = await unitOfWork.Repository<Follow>().Query()
            .Where(f => f.FollowerId == userId).Select(f => f.FollowingId).ToListAsync(ct);
        followingIds.Add(userId);

        var entries = await unitOfWork.Repository<ActivityFeedEntry>().Query()
            .Where(a => followingIds.Contains(a.UserId))
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync(ct);

        var users = await identityService.GetUsersAsync(entries.Select(e => e.UserId).Distinct());

        return entries.Where(e => users.ContainsKey(e.UserId)).Select(e =>
            new ActivityFeedItemDto(e.Id, e.UserId, users[e.UserId].DisplayName, users[e.UserId].AvatarUrl,
                e.Type, e.TargetType, e.TargetId, e.MetadataJson, e.CreatedAt)).ToList();
    }

    public async Task<CommentDto> AddCommentAsync(Guid userId, AddCommentRequest request, CancellationToken ct = default)
    {
        var comment = new Comment
        {
            UserId = userId,
            TargetType = request.TargetType,
            TargetId = request.TargetId,
            Body = request.Body,
            ParentCommentId = request.ParentCommentId
        };
        await unitOfWork.Repository<Comment>().AddAsync(comment, ct);

        await unitOfWork.Repository<ActivityFeedEntry>().AddAsync(new ActivityFeedEntry
        {
            UserId = userId,
            Type = ActivityType.PostedComment,
            TargetType = request.TargetType,
            TargetId = request.TargetId
        }, ct);
        await unitOfWork.SaveChangesAsync(ct);

        var user = await identityService.GetUserAsync(userId);
        return new CommentDto(comment.Id, userId, user?.DisplayName ?? "Unknown", user?.AvatarUrl, comment.Body,
            comment.ParentCommentId, comment.CreatedAt, 0);
    }

    public async Task<IReadOnlyList<CommentDto>> GetCommentsAsync(TargetType targetType, Guid targetId, CancellationToken ct = default)
    {
        var comments = await unitOfWork.Repository<Comment>().Query()
            .Where(c => c.TargetType == targetType && c.TargetId == targetId && !c.IsDeleted)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync(ct);

        var users = await identityService.GetUsersAsync(comments.Select(c => c.UserId).Distinct());
        var likeCounts = await unitOfWork.Repository<Like>().Query()
            .Where(l => l.TargetType == TargetType.Comment && comments.Select(c => c.Id).Contains(l.TargetId))
            .GroupBy(l => l.TargetId)
            .Select(g => new { CommentId = g.Key, Count = g.Count() })
            .ToListAsync(ct);
        var likeCountByComment = likeCounts.ToDictionary(x => x.CommentId, x => x.Count);

        return comments.Where(c => users.ContainsKey(c.UserId)).Select(c =>
            new CommentDto(c.Id, c.UserId, users[c.UserId].DisplayName, users[c.UserId].AvatarUrl, c.Body,
                c.ParentCommentId, c.CreatedAt, likeCountByComment.GetValueOrDefault(c.Id))).ToList();
    }

    public async Task ToggleLikeAsync(Guid userId, ToggleLikeRequest request, CancellationToken ct = default)
    {
        var repo = unitOfWork.Repository<Like>();
        var existing = await repo.Query()
            .FirstOrDefaultAsync(l => l.UserId == userId && l.TargetType == request.TargetType && l.TargetId == request.TargetId, ct);

        if (existing is not null)
        {
            repo.Remove(existing);
        }
        else
        {
            await repo.AddAsync(new Like { UserId = userId, TargetType = request.TargetType, TargetId = request.TargetId }, ct);
        }

        await unitOfWork.SaveChangesAsync(ct);
    }
}
