using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatefield : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusRefundDeposit",
                table: "AuctionDocuments");

            migrationBuilder.AddColumn<string>(
                name: "UrlAction",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "StatusDeposit",
                table: "AuctionDocuments",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UrlAction",
                table: "Notifications");

            migrationBuilder.AlterColumn<bool>(
                name: "StatusDeposit",
                table: "AuctionDocuments",
                type: "bit",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "StatusRefundDeposit",
                table: "AuctionDocuments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
