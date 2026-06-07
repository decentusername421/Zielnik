using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zielnik.Migrations
{
    /// <inheritdoc />
    public partial class ModeracjaRoslinIKategorii : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRejected",
                table: "Plants",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "PlantCategories",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "PlantCategories",
                type: "INTEGER",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlantCategories_CreatedByUserId",
                table: "PlantCategories",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlantCategories_AspNetUsers_CreatedByUserId",
                table: "PlantCategories",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlantCategories_AspNetUsers_CreatedByUserId",
                table: "PlantCategories");

            migrationBuilder.DropIndex(
                name: "IX_PlantCategories_CreatedByUserId",
                table: "PlantCategories");

            migrationBuilder.DropColumn(
                name: "IsRejected",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "PlantCategories");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "PlantCategories");
        }
    }
}
