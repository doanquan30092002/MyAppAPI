using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatefieldauctionround : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PriceMax",
                table: "AuctionRounds",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceMin",
                table: "AuctionRounds",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPriceMax",
                table: "AuctionRounds",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceMax",
                table: "AuctionRounds");

            migrationBuilder.DropColumn(
                name: "PriceMin",
                table: "AuctionRounds");

            migrationBuilder.DropColumn(
                name: "TotalPriceMax",
                table: "AuctionRounds");
        }
    }
}
