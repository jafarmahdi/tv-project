using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WatchLog.Domain.Entities;

namespace WatchLog.Infrastructure.Persistence.Configurations;

public class GenreConfiguration : IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> b)
    {
        b.ToTable("genres");
        b.HasIndex(x => x.TmdbId).IsUnique();
        b.Property(x => x.Name).HasMaxLength(100).IsRequired();
        b.HasData(SeedData.Genres);
    }
}

public class MovieConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> b)
    {
        b.ToTable("movies");
        b.HasIndex(x => x.TmdbId).IsUnique();
        b.Property(x => x.Title).HasMaxLength(300).IsRequired();
        b.Property(x => x.Overview).HasMaxLength(4000);
    }
}

public class MovieGenreConfiguration : IEntityTypeConfiguration<MovieGenre>
{
    public void Configure(EntityTypeBuilder<MovieGenre> b)
    {
        b.ToTable("movie_genres");
        b.HasKey(x => new { x.MovieId, x.GenreId });
        b.HasOne(x => x.Movie).WithMany(m => m.Genres).HasForeignKey(x => x.MovieId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.Genre).WithMany(g => g.MovieGenres).HasForeignKey(x => x.GenreId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class SeriesConfiguration : IEntityTypeConfiguration<Series>
{
    public void Configure(EntityTypeBuilder<Series> b)
    {
        b.ToTable("series");
        b.HasIndex(x => x.TmdbId).IsUnique();
        b.Property(x => x.Title).HasMaxLength(300).IsRequired();
        b.Property(x => x.Overview).HasMaxLength(4000);
    }
}

public class SeriesGenreConfiguration : IEntityTypeConfiguration<SeriesGenre>
{
    public void Configure(EntityTypeBuilder<SeriesGenre> b)
    {
        b.ToTable("series_genres");
        b.HasKey(x => new { x.SeriesId, x.GenreId });
        b.HasOne(x => x.Series).WithMany(s => s.Genres).HasForeignKey(x => x.SeriesId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.Genre).WithMany(g => g.SeriesGenres).HasForeignKey(x => x.GenreId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class SeasonConfiguration : IEntityTypeConfiguration<Season>
{
    public void Configure(EntityTypeBuilder<Season> b)
    {
        b.ToTable("seasons");
        b.HasIndex(x => new { x.SeriesId, x.SeasonNumber }).IsUnique();
        b.Property(x => x.Name).HasMaxLength(300).IsRequired();
        b.HasOne(x => x.Series).WithMany(s => s.Seasons).HasForeignKey(x => x.SeriesId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class EpisodeConfiguration : IEntityTypeConfiguration<Episode>
{
    public void Configure(EntityTypeBuilder<Episode> b)
    {
        b.ToTable("episodes");
        b.HasIndex(x => new { x.SeasonId, x.EpisodeNumber }).IsUnique();
        b.Property(x => x.Title).HasMaxLength(300).IsRequired();
        b.HasOne(x => x.Season).WithMany(s => s.Episodes).HasForeignKey(x => x.SeasonId).OnDelete(DeleteBehavior.Cascade);
    }
}
