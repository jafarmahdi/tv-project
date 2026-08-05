using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WatchLog.Domain.Entities;
using WatchLog.Infrastructure.Identity;

namespace WatchLog.Infrastructure.Persistence.Configurations;

public class UserListConfiguration : IEntityTypeConfiguration<UserList>
{
    public void Configure(EntityTypeBuilder<UserList> b)
    {
        b.ToTable("user_lists");
        b.Property(x => x.Name).HasMaxLength(100).IsRequired();
        b.HasIndex(x => new { x.UserId, x.Type });
        b.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class ListItemConfiguration : IEntityTypeConfiguration<ListItem>
{
    public void Configure(EntityTypeBuilder<ListItem> b)
    {
        b.ToTable("list_items");
        b.HasOne(x => x.List).WithMany(l => l.Items).HasForeignKey(x => x.ListId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.Movie).WithMany().HasForeignKey(x => x.MovieId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.Series).WithMany().HasForeignKey(x => x.SeriesId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class EpisodeProgressConfiguration : IEntityTypeConfiguration<EpisodeProgress>
{
    public void Configure(EntityTypeBuilder<EpisodeProgress> b)
    {
        b.ToTable("episode_progress");
        b.HasIndex(x => new { x.UserId, x.EpisodeId }).IsUnique();
        b.HasOne(x => x.Episode).WithMany().HasForeignKey(x => x.EpisodeId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class MovieWatchConfiguration : IEntityTypeConfiguration<MovieWatch>
{
    public void Configure(EntityTypeBuilder<MovieWatch> b)
    {
        b.ToTable("movie_watches");
        b.HasIndex(x => new { x.UserId, x.MovieId }).IsUnique();
        b.HasOne(x => x.Movie).WithMany().HasForeignKey(x => x.MovieId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}
