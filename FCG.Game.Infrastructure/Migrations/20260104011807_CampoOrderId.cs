using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FCG.Game.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CampoOrderId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "createdAt",
                table: "UserLibraryGames",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2026, 1, 4, 1, 18, 7, 250, DateTimeKind.Utc).AddTicks(8715),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2026, 1, 2, 19, 21, 59, 321, DateTimeKind.Utc).AddTicks(1527));

            migrationBuilder.AddColumn<Guid>(
                name: "orderId",
                table: "UserLibraryGames",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "Tags",
                table: "Games",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "orderId",
                table: "UserLibraryGames");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createdAt",
                table: "UserLibraryGames",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2026, 1, 2, 19, 21, 59, 321, DateTimeKind.Utc).AddTicks(1527),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2026, 1, 4, 1, 18, 7, 250, DateTimeKind.Utc).AddTicks(8715));

            migrationBuilder.AlterColumn<string>(
                name: "Tags",
                table: "Games",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);
        }
    }
}
