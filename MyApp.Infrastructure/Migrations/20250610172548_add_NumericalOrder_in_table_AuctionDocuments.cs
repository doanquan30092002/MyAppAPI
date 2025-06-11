using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_NumericalOrder_in_table_AuctionDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumericalOrder",
                table: "AuctionAssets");

            migrationBuilder.AddColumn<int>(
                name: "NumericalOrder",
                table: "AuctionDocuments",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumericalOrder",
                table: "AuctionDocuments");

            migrationBuilder.AddColumn<int>(
                name: "NumericalOrder",
                table: "AuctionAssets",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
