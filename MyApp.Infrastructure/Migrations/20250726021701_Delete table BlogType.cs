using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeletetableBlogType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blogs_BlogTypes_BlogTypeId",
                table: "Blogs");

            migrationBuilder.DropTable(
                name: "BlogTypes");

            migrationBuilder.DropIndex(
                name: "IX_Blogs_BlogTypeId",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "BlogTypeId",
                table: "Blogs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BlogTypeId",
                table: "Blogs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "BlogTypes",
                columns: table => new
                {
                    BlogTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BlogsName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogTypes", x => x.BlogTypeId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_BlogTypeId",
                table: "Blogs",
                column: "BlogTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Blogs_BlogTypes_BlogTypeId",
                table: "Blogs",
                column: "BlogTypeId",
                principalTable: "BlogTypes",
                principalColumn: "BlogTypeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
