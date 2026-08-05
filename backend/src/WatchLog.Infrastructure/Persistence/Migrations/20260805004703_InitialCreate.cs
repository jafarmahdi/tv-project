using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WatchLog.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "achievements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    IconUrl = table.Column<string>(type: "text", nullable: true),
                    CriteriaDescription = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_achievements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "genres",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TmdbId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "movies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TmdbId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    OriginalTitle = table.Column<string>(type: "text", nullable: true),
                    Overview = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    PosterPath = table.Column<string>(type: "text", nullable: true),
                    BackdropPath = table.Column<string>(type: "text", nullable: true),
                    ReleaseDate = table.Column<DateOnly>(type: "date", nullable: true),
                    RuntimeMinutes = table.Column<int>(type: "integer", nullable: true),
                    VoteAverage = table.Column<double>(type: "double precision", nullable: false),
                    Popularity = table.Column<double>(type: "double precision", nullable: false),
                    TrailerYoutubeKey = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "series",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TmdbId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    OriginalTitle = table.Column<string>(type: "text", nullable: true),
                    Overview = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    PosterPath = table.Column<string>(type: "text", nullable: true),
                    BackdropPath = table.Column<string>(type: "text", nullable: true),
                    FirstAirDate = table.Column<DateOnly>(type: "date", nullable: true),
                    LastAirDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    VoteAverage = table.Column<double>(type: "double precision", nullable: false),
                    Popularity = table.Column<double>(type: "double precision", nullable: false),
                    TrailerYoutubeKey = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_series", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    AvatarUrl = table.Column<string>(type: "text", nullable: true),
                    Bio = table.Column<string>(type: "text", nullable: true),
                    Locale = table.Column<string>(type: "text", nullable: false),
                    ThemePreference = table.Column<int>(type: "integer", nullable: false),
                    IsPrivate = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "movie_genres",
                columns: table => new
                {
                    MovieId = table.Column<Guid>(type: "uuid", nullable: false),
                    GenreId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movie_genres", x => new { x.MovieId, x.GenreId });
                    table.ForeignKey(
                        name: "FK_movie_genres_genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_movie_genres_movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "role_claims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_claims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_role_claims_roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "seasons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SeriesId = table.Column<Guid>(type: "uuid", nullable: false),
                    SeasonNumber = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Overview = table.Column<string>(type: "text", nullable: true),
                    PosterPath = table.Column<string>(type: "text", nullable: true),
                    AirDate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seasons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_seasons_series_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "series",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "series_genres",
                columns: table => new
                {
                    SeriesId = table.Column<Guid>(type: "uuid", nullable: false),
                    GenreId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_series_genres", x => new { x.SeriesId, x.GenreId });
                    table.ForeignKey(
                        name: "FK_series_genres_genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_series_genres_series_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "series",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "activity_feed_entries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    TargetType = table.Column<int>(type: "integer", nullable: true),
                    TargetId = table.Column<Guid>(type: "uuid", nullable: true),
                    MetadataJson = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_activity_feed_entries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_activity_feed_entries_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ai_history_entries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Prompt = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Response = table.Column<string>(type: "text", nullable: false),
                    MetadataJson = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ai_history_entries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ai_history_entries_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "collections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    PosterUrl = table.Column<string>(type: "text", nullable: true),
                    IsCurated = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_collections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_collections_users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetType = table.Column<int>(type: "integer", nullable: false),
                    TargetId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentCommentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Body = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_comments_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "devices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Platform = table.Column<int>(type: "integer", nullable: false),
                    PushToken = table.Column<string>(type: "text", nullable: true),
                    DeviceName = table.Column<string>(type: "text", nullable: true),
                    LastSeenAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_devices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_devices_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "follows",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FollowerId = table.Column<Guid>(type: "uuid", nullable: false),
                    FollowingId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_follows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_follows_users_FollowerId",
                        column: x => x.FollowerId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_follows_users_FollowingId",
                        column: x => x.FollowingId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "likes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetType = table.Column<int>(type: "integer", nullable: false),
                    TargetId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_likes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_likes_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "movie_watches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    MovieId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsWatched = table.Column<bool>(type: "boolean", nullable: false),
                    IsFavorite = table.Column<bool>(type: "boolean", nullable: false),
                    WatchedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movie_watches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_movie_watches_movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_movie_watches_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Body = table.Column<string>(type: "text", nullable: false),
                    DataJson = table.Column<string>(type: "text", nullable: true),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_notifications_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "passkey_credentials",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CredentialId = table.Column<byte[]>(type: "bytea", nullable: false),
                    PublicKey = table.Column<byte[]>(type: "bytea", nullable: false),
                    SignCount = table.Column<long>(type: "bigint", nullable: false),
                    AaGuid = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceName = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_passkey_credentials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_passkey_credentials_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ratings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetType = table.Column<int>(type: "integer", nullable: false),
                    TargetId = table.Column<Guid>(type: "uuid", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ratings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ratings_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "recommendations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetType = table.Column<int>(type: "integer", nullable: false),
                    TargetId = table.Column<Guid>(type: "uuid", nullable: false),
                    Score = table.Column<double>(type: "double precision", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    Source = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recommendations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_recommendations_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TokenHash = table.Column<string>(type: "text", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RevokedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ReplacedByTokenHash = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_refresh_tokens_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_achievements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AchievementId = table.Column<Guid>(type: "uuid", nullable: false),
                    EarnedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_achievements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_achievements_achievements_AchievementId",
                        column: x => x.AchievementId,
                        principalTable: "achievements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_achievements_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_claims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_claims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_claims_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_lists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_lists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_lists_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_logins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_logins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_user_logins_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_roles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_user_roles_roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_roles_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_tokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_tokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_user_tokens_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "episodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: false),
                    EpisodeNumber = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Overview = table.Column<string>(type: "text", nullable: true),
                    StillPath = table.Column<string>(type: "text", nullable: true),
                    AirDate = table.Column<DateOnly>(type: "date", nullable: true),
                    RuntimeMinutes = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_episodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_episodes_seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "collection_items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CollectionId = table.Column<Guid>(type: "uuid", nullable: false),
                    MovieId = table.Column<Guid>(type: "uuid", nullable: true),
                    SeriesId = table.Column<Guid>(type: "uuid", nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_collection_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_collection_items_collections_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "collections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_collection_items_movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_collection_items_series_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "series",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "list_items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ListId = table.Column<Guid>(type: "uuid", nullable: false),
                    MovieId = table.Column<Guid>(type: "uuid", nullable: true),
                    SeriesId = table.Column<Guid>(type: "uuid", nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    AddedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_list_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_list_items_movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_list_items_series_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "series",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_list_items_user_lists_ListId",
                        column: x => x.ListId,
                        principalTable: "user_lists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "episode_progress",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    EpisodeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    IsFavorite = table.Column<bool>(type: "boolean", nullable: false),
                    WatchedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_episode_progress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_episode_progress_episodes_EpisodeId",
                        column: x => x.EpisodeId,
                        principalTable: "episodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_episode_progress_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "achievements",
                columns: new[] { "Id", "Code", "CreatedAt", "CriteriaDescription", "Description", "IconUrl", "Name" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-4000-8000-000000000001"), "first-watch", new DateTimeOffset(new DateTime(2026, 8, 5, 0, 47, 1, 889, DateTimeKind.Unspecified).AddTicks(8680), new TimeSpan(0, 0, 0, 0, 0)), "Watch 1 episode or movie.", "Watched your first episode or movie.", null, "First Watch" },
                    { new Guid("10000000-0000-4000-8000-000000000002"), "episodes-50", new DateTimeOffset(new DateTime(2026, 8, 5, 0, 47, 1, 890, DateTimeKind.Unspecified).AddTicks(2400), new TimeSpan(0, 0, 0, 0, 0)), "Watch 50 episodes.", "Watched 50 episodes.", null, "Binge Starter" },
                    { new Guid("10000000-0000-4000-8000-000000000003"), "episodes-100", new DateTimeOffset(new DateTime(2026, 8, 5, 0, 47, 1, 890, DateTimeKind.Unspecified).AddTicks(2460), new TimeSpan(0, 0, 0, 0, 0)), "Watch 100 episodes.", "Watched 100 episodes.", null, "Century Club" },
                    { new Guid("10000000-0000-4000-8000-000000000004"), "episodes-500", new DateTimeOffset(new DateTime(2026, 8, 5, 0, 47, 1, 890, DateTimeKind.Unspecified).AddTicks(2480), new TimeSpan(0, 0, 0, 0, 0)), "Watch 500 episodes.", "Watched 500 episodes.", null, "Marathoner" },
                    { new Guid("10000000-0000-4000-8000-000000000005"), "movies-25", new DateTimeOffset(new DateTime(2026, 8, 5, 0, 47, 1, 890, DateTimeKind.Unspecified).AddTicks(2490), new TimeSpan(0, 0, 0, 0, 0)), "Watch 25 movies.", "Watched 25 movies.", null, "Film Buff" },
                    { new Guid("10000000-0000-4000-8000-000000000006"), "movies-100", new DateTimeOffset(new DateTime(2026, 8, 5, 0, 47, 1, 890, DateTimeKind.Unspecified).AddTicks(2500), new TimeSpan(0, 0, 0, 0, 0)), "Watch 100 movies.", "Watched 100 movies.", null, "Cinephile" }
                });

            migrationBuilder.InsertData(
                table: "genres",
                columns: new[] { "Id", "Name", "TmdbId" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-4000-8000-000000000012"), "Adventure", 12 },
                    { new Guid("00000000-0000-4000-8000-000000000014"), "Fantasy", 14 },
                    { new Guid("00000000-0000-4000-8000-000000000016"), "Animation", 16 },
                    { new Guid("00000000-0000-4000-8000-000000000018"), "Drama", 18 },
                    { new Guid("00000000-0000-4000-8000-000000000027"), "Horror", 27 },
                    { new Guid("00000000-0000-4000-8000-000000000028"), "Action", 28 },
                    { new Guid("00000000-0000-4000-8000-000000000035"), "Comedy", 35 },
                    { new Guid("00000000-0000-4000-8000-000000000036"), "History", 36 },
                    { new Guid("00000000-0000-4000-8000-000000000037"), "Western", 37 },
                    { new Guid("00000000-0000-4000-8000-000000000053"), "Thriller", 53 },
                    { new Guid("00000000-0000-4000-8000-000000000080"), "Crime", 80 },
                    { new Guid("00000000-0000-4000-8000-000000000099"), "Documentary", 99 },
                    { new Guid("00000000-0000-4000-8000-000000000878"), "Science Fiction", 878 },
                    { new Guid("00000000-0000-4000-8000-000000009648"), "Mystery", 9648 },
                    { new Guid("00000000-0000-4000-8000-000000010402"), "Music", 10402 },
                    { new Guid("00000000-0000-4000-8000-000000010749"), "Romance", 10749 },
                    { new Guid("00000000-0000-4000-8000-000000010751"), "Family", 10751 },
                    { new Guid("00000000-0000-4000-8000-000000010752"), "War", 10752 },
                    { new Guid("00000000-0000-4000-8000-000000010759"), "Action & Adventure", 10759 },
                    { new Guid("00000000-0000-4000-8000-000000010762"), "Kids", 10762 },
                    { new Guid("00000000-0000-4000-8000-000000010763"), "News", 10763 },
                    { new Guid("00000000-0000-4000-8000-000000010764"), "Reality", 10764 },
                    { new Guid("00000000-0000-4000-8000-000000010765"), "Sci-Fi & Fantasy", 10765 },
                    { new Guid("00000000-0000-4000-8000-000000010766"), "Soap", 10766 },
                    { new Guid("00000000-0000-4000-8000-000000010767"), "Talk", 10767 },
                    { new Guid("00000000-0000-4000-8000-000000010768"), "War & Politics", 10768 },
                    { new Guid("00000000-0000-4000-8000-000000010770"), "TV Movie", 10770 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_achievements_Code",
                table: "achievements",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_activity_feed_entries_UserId_CreatedAt",
                table: "activity_feed_entries",
                columns: new[] { "UserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ai_history_entries_UserId",
                table: "ai_history_entries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_collection_items_CollectionId",
                table: "collection_items",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_collection_items_MovieId",
                table: "collection_items",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_collection_items_SeriesId",
                table: "collection_items",
                column: "SeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_collections_CreatedByUserId",
                table: "collections",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_comments_TargetType_TargetId",
                table: "comments",
                columns: new[] { "TargetType", "TargetId" });

            migrationBuilder.CreateIndex(
                name: "IX_comments_UserId",
                table: "comments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_devices_UserId",
                table: "devices",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_episode_progress_EpisodeId",
                table: "episode_progress",
                column: "EpisodeId");

            migrationBuilder.CreateIndex(
                name: "IX_episode_progress_UserId_EpisodeId",
                table: "episode_progress",
                columns: new[] { "UserId", "EpisodeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_episodes_SeasonId_EpisodeNumber",
                table: "episodes",
                columns: new[] { "SeasonId", "EpisodeNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_follows_FollowerId_FollowingId",
                table: "follows",
                columns: new[] { "FollowerId", "FollowingId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_follows_FollowingId",
                table: "follows",
                column: "FollowingId");

            migrationBuilder.CreateIndex(
                name: "IX_genres_TmdbId",
                table: "genres",
                column: "TmdbId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_likes_UserId_TargetType_TargetId",
                table: "likes",
                columns: new[] { "UserId", "TargetType", "TargetId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_list_items_ListId",
                table: "list_items",
                column: "ListId");

            migrationBuilder.CreateIndex(
                name: "IX_list_items_MovieId",
                table: "list_items",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_list_items_SeriesId",
                table: "list_items",
                column: "SeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_movie_genres_GenreId",
                table: "movie_genres",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_movie_watches_MovieId",
                table: "movie_watches",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_movie_watches_UserId_MovieId",
                table: "movie_watches",
                columns: new[] { "UserId", "MovieId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_movies_TmdbId",
                table: "movies",
                column: "TmdbId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_notifications_UserId_IsRead",
                table: "notifications",
                columns: new[] { "UserId", "IsRead" });

            migrationBuilder.CreateIndex(
                name: "IX_passkey_credentials_CredentialId",
                table: "passkey_credentials",
                column: "CredentialId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_passkey_credentials_UserId",
                table: "passkey_credentials",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ratings_UserId_TargetType_TargetId",
                table: "ratings",
                columns: new[] { "UserId", "TargetType", "TargetId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_recommendations_UserId",
                table: "recommendations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_TokenHash",
                table: "refresh_tokens",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_UserId",
                table: "refresh_tokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_role_claims_RoleId",
                table: "role_claims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "roles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_seasons_SeriesId_SeasonNumber",
                table: "seasons",
                columns: new[] { "SeriesId", "SeasonNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_series_TmdbId",
                table: "series",
                column: "TmdbId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_series_genres_GenreId",
                table: "series_genres",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_user_achievements_AchievementId",
                table: "user_achievements",
                column: "AchievementId");

            migrationBuilder.CreateIndex(
                name: "IX_user_achievements_UserId_AchievementId",
                table: "user_achievements",
                columns: new[] { "UserId", "AchievementId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_claims_UserId",
                table: "user_claims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_user_lists_UserId_Type",
                table: "user_lists",
                columns: new[] { "UserId", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_user_logins_UserId",
                table: "user_logins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_RoleId",
                table: "user_roles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "users",
                column: "NormalizedUserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "activity_feed_entries");

            migrationBuilder.DropTable(
                name: "ai_history_entries");

            migrationBuilder.DropTable(
                name: "collection_items");

            migrationBuilder.DropTable(
                name: "comments");

            migrationBuilder.DropTable(
                name: "devices");

            migrationBuilder.DropTable(
                name: "episode_progress");

            migrationBuilder.DropTable(
                name: "follows");

            migrationBuilder.DropTable(
                name: "likes");

            migrationBuilder.DropTable(
                name: "list_items");

            migrationBuilder.DropTable(
                name: "movie_genres");

            migrationBuilder.DropTable(
                name: "movie_watches");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "passkey_credentials");

            migrationBuilder.DropTable(
                name: "ratings");

            migrationBuilder.DropTable(
                name: "recommendations");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "role_claims");

            migrationBuilder.DropTable(
                name: "series_genres");

            migrationBuilder.DropTable(
                name: "user_achievements");

            migrationBuilder.DropTable(
                name: "user_claims");

            migrationBuilder.DropTable(
                name: "user_logins");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "user_tokens");

            migrationBuilder.DropTable(
                name: "collections");

            migrationBuilder.DropTable(
                name: "episodes");

            migrationBuilder.DropTable(
                name: "user_lists");

            migrationBuilder.DropTable(
                name: "movies");

            migrationBuilder.DropTable(
                name: "genres");

            migrationBuilder.DropTable(
                name: "achievements");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "seasons");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "series");
        }
    }
}
