using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zielnik.Migrations
{
    /// <inheritdoc />
    public partial class PlantApproval : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Plants",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Plants",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Plants_CreatedByUserId",
                table: "Plants",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Plants_AspNetUsers_CreatedByUserId",
                table: "Plants",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Plants_AspNetUsers_CreatedByUserId",
                table: "Plants");

            migrationBuilder.DropIndex(
                name: "IX_Plants_CreatedByUserId",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Plants");
        }
    }
}
