using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class update_field_StatusRefund : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRefundRequested",
                table: "AuctionDocuments");

            migrationBuilder.AddColumn<int>(
                name: "StatusRefund",
                table: "AuctionDocuments",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusRefund",
                table: "AuctionDocuments");

            migrationBuilder.AddColumn<bool>(
                name: "IsRefundRequested",
                table: "AuctionDocuments",
                type: "bit",
                nullable: true);
        }
    }
}
