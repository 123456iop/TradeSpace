using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradeSpace.Migrations
{
    /// <inheritdoc />
    public partial class AddStoreToCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "Categories",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_StoreId",
                table: "Categories",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Stores_StoreId",
                table: "Categories",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Stores_StoreId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_StoreId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "Categories");
        }
    }
}
