using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WatchLog.Domain.Entities;
using WatchLog.Infrastructure.Identity;

namespace WatchLog.Infrastructure.Persistence;

/// <summary>
/// The single EF Core context for WatchLog. Inherits `IdentityDbContext` so auth tables
/// (AspNetUsers, AspNetRoles, ...) live alongside the domain schema in one Postgres database.
/// </summary>
public class WatchLogDbContext(DbContextOptions<WatchLogDbContext> options)
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options)
{
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<MovieGenre> MovieGenres => Set<MovieGenre>();
    public DbSet<Series> Series => Set<Series>();
    public DbSet<SeriesGenre> SeriesGenres => Set<SeriesGenre>();
    public DbSet<Season> Seasons => Set<Season>();
    public DbSet<Episode> Episodes => Set<Episode>();

    public DbSet<UserList> UserLists => Set<UserList>();
    public DbSet<ListItem> ListItems => Set<ListItem>();
    public DbSet<EpisodeProgress> EpisodeProgresses => Set<EpisodeProgress>();
    public DbSet<MovieWatch> MovieWatches => Set<MovieWatch>();

    public DbSet<Rating> Ratings => Set<Rating>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Like> Likes => Set<Like>();
    public DbSet<Follow> Follows => Set<Follow>();
    public DbSet<ActivityFeedEntry> ActivityFeedEntries => Set<ActivityFeedEntry>();

    public DbSet<Achievement> Achievements => Set<Achievement>();
    public DbSet<UserAchievement> UserAchievements => Set<UserAchievement>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Recommendation> Recommendations => Set<Recommendation>();
    public DbSet<AiHistoryEntry> AiHistoryEntries => Set<AiHistoryEntry>();
    public DbSet<Device> Devices => Set<Device>();
    public DbSet<Collection> Collections => Set<Collection>();
    public DbSet<CollectionItem> CollectionItems => Set<CollectionItem>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<PasskeyCredential> PasskeyCredentials => Set<PasskeyCredential>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(WatchLogDbContext).Assembly);

        // ASP.NET Identity's default table names (AspNetUsers, ...) are fine for a fresh schema,
        // but namespaced table names read better in a Postgres client and avoid clutter.
        builder.Entity<ApplicationUser>().ToTable("users");
        builder.Entity<ApplicationRole>().ToTable("roles");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("user_roles");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("user_claims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("user_logins");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("role_claims");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("user_tokens");

        // The built-in "Admin" role always exists; which user holds it is assigned at runtime
        // (see IdentitySeeder.EnsureInitialAdminAsync), never hardcoded here.
        builder.Entity<ApplicationRole>().HasData(SeedData.Roles);
    }
}
