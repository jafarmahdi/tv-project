using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatchLog.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class StabilizeAchievementSeedTimestamps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-4000-8000-000000000001"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 8, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-4000-8000-000000000002"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 8, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-4000-8000-000000000003"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 8, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-4000-8000-000000000004"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 8, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-4000-8000-000000000005"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 8, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-4000-8000-000000000006"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 8, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-4000-8000-000000000001"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 8, 5, 0, 47, 1, 889, DateTimeKind.Unspecified).AddTicks(8680), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-4000-8000-000000000002"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 8, 5, 0, 47, 1, 890, DateTimeKind.Unspecified).AddTicks(2400), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-4000-8000-000000000003"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 8, 5, 0, 47, 1, 890, DateTimeKind.Unspecified).AddTicks(2460), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-4000-8000-000000000004"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 8, 5, 0, 47, 1, 890, DateTimeKind.Unspecified).AddTicks(2480), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-4000-8000-000000000005"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 8, 5, 0, 47, 1, 890, DateTimeKind.Unspecified).AddTicks(2490), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "achievements",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-4000-8000-000000000006"),
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 8, 5, 0, 47, 1, 890, DateTimeKind.Unspecified).AddTicks(2500), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
