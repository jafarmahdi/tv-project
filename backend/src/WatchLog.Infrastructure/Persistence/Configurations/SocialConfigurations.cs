using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WatchLog.Domain.Entities;
using WatchLog.Infrastructure.Identity;

namespace WatchLog.Infrastructure.Persistence.Configurations;

public class RatingConfiguration : IEntityTypeConfiguration<Rating>
{
    public void Configure(EntityTypeBuilder<Rating> b)
    {
        b.ToTable("ratings");
        b.HasIndex(x => new { x.UserId, x.TargetType, x.TargetId }).IsUnique();
        b.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> b)
    {
        b.ToTable("comments");
        b.Property(x => x.Body).HasMaxLength(2000).IsRequired();
        b.HasIndex(x => new { x.TargetType, x.TargetId });
        b.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class LikeConfiguration : IEntityTypeConfiguration<Like>
{
    public void Configure(EntityTypeBuilder<Like> b)
    {
        b.ToTable("likes");
        b.HasIndex(x => new { x.UserId, x.TargetType, x.TargetId }).IsUnique();
        b.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class FollowConfiguration : IEntityTypeConfiguration<Follow>
{
    public void Configure(EntityTypeBuilder<Follow> b)
    {
        b.ToTable("follows");
        b.HasIndex(x => new { x.FollowerId, x.FollowingId }).IsUnique();
        b.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.FollowerId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.FollowingId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class ActivityFeedEntryConfiguration : IEntityTypeConfiguration<ActivityFeedEntry>
{
    public void Configure(EntityTypeBuilder<ActivityFeedEntry> b)
    {
        b.ToTable("activity_feed_entries");
        b.HasIndex(x => new { x.UserId, x.CreatedAt });
        b.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}
