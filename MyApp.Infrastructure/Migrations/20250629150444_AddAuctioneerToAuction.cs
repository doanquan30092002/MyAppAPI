using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuctioneerToAuction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Auctioneer",
                table: "Auctions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Auctions_Auctioneer",
                table: "Auctions",
                column: "Auctioneer");

            migrationBuilder.AddForeignKey(
                name: "FK_Auctions_Users_Auctioneer",
                table: "Auctions",
                column: "Auctioneer",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auctions_Users_Auctioneer",
                table: "Auctions");

            migrationBuilder.DropIndex(
                name: "IX_Auctions_Auctioneer",
                table: "Auctions");

            migrationBuilder.DropColumn(
                name: "Auctioneer",
                table: "Auctions");
        }
    }
}
