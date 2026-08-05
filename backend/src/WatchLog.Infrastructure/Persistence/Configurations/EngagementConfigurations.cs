using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WatchLog.Domain.Entities;
using WatchLog.Infrastructure.Identity;

namespace WatchLog.Infrastructure.Persistence.Configurations;

public class AchievementConfiguration : IEntityTypeConfiguration<Achievement>
{
    public void Configure(EntityTypeBuilder<Achievement> b)
    {
        b.ToTable("achievements");
        b.HasIndex(x => x.Code).IsUnique();
        b.Property(x => x.Code).HasMaxLength(100).IsRequired();
        b.Property(x => x.Name).HasMaxLength(150).IsRequired();
        b.HasData(SeedData.Achievements);
    }
}

public class UserAchievementConfiguration : IEntityTypeConfiguration<UserAchievement>
{
    public void Configure(EntityTypeBuilder<UserAchievement> b)
    {
        b.ToTable("user_achievements");
        b.HasIndex(x => new { x.UserId, x.AchievementId }).IsUnique();
        b.HasOne(x => x.Achievement).WithMany(a => a.UserAchievements).HasForeignKey(x => x.AchievementId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> b)
    {
        b.ToTable("notifications");
        b.HasIndex(x => new { x.UserId, x.IsRead });
        b.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class RecommendationConfiguration : IEntityTypeConfiguration<Recommendation>
{
    public void Configure(EntityTypeBuilder<Recommendation> b)
    {
        b.ToTable("recommendations");
        b.HasIndex(x => x.UserId);
        b.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class AiHistoryEntryConfiguration : IEntityTypeConfiguration<AiHistoryEntry>
{
    public void Configure(EntityTypeBuilder<AiHistoryEntry> b)
    {
        b.ToTable("ai_history_entries");
        b.Property(x => x.Prompt).HasMaxLength(2000).IsRequired();
        b.HasIndex(x => x.UserId);
        b.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> b)
    {
        b.ToTable("devices");
        b.HasIndex(x => x.UserId);
        b.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class CollectionConfiguration : IEntityTypeConfiguration<Collection>
{
    public void Configure(EntityTypeBuilder<Collection> b)
    {
        b.ToTable("collections");
        b.Property(x => x.Name).HasMaxLength(150).IsRequired();
        b.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class CollectionItemConfiguration : IEntityTypeConfiguration<CollectionItem>
{
    public void Configure(EntityTypeBuilder<CollectionItem> b)
    {
        b.ToTable("collection_items");
        b.HasOne(x => x.Collection).WithMany(c => c.Items).HasForeignKey(x => x.CollectionId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.Movie).WithMany().HasForeignKey(x => x.MovieId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.Series).WithMany().HasForeignKey(x => x.SeriesId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> b)
    {
        b.ToTable("refresh_tokens");
        b.HasIndex(x => x.TokenHash).IsUnique();
        b.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class PasskeyCredentialConfiguration : IEntityTypeConfiguration<PasskeyCredential>
{
    public void Configure(EntityTypeBuilder<PasskeyCredential> b)
    {
        b.ToTable("passkey_credentials");
        b.HasIndex(x => x.CredentialId).IsUnique();
        b.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}
