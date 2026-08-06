using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatchLog.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("20000000-0000-4000-8000-000000000001"), "20000000-0000-4000-8000-000000000001", "Admin", "ADMIN" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-4000-8000-000000000001"));
        }
    }
}
