using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FCG.Game.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentIdToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PaymentId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "Orders");
        }
    }
}
