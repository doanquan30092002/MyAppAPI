using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update_field_AuctionDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAttended",
                table: "AuctionDocuments",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRefundRequested",
                table: "AuctionDocuments",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefundProof",
                table: "AuctionDocuments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefundReason",
                table: "AuctionDocuments",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAttended",
                table: "AuctionDocuments");

            migrationBuilder.DropColumn(
                name: "IsRefundRequested",
                table: "AuctionDocuments");

            migrationBuilder.DropColumn(
                name: "RefundProof",
                table: "AuctionDocuments");

            migrationBuilder.DropColumn(
                name: "RefundReason",
                table: "AuctionDocuments");
        }
    }
}
