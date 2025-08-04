using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class update_model : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HighestBid",
                table: "AuctionRounds");

            migrationBuilder.AddColumn<bool>(
                name: "FlagWinner",
                table: "AuctionRoundPrices",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FlagWinner",
                table: "AuctionRoundPrices");

            migrationBuilder.AddColumn<decimal>(
                name: "HighestBid",
                table: "AuctionRounds",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
