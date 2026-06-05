using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zielnik.Migrations
{
    /// <inheritdoc />
    public partial class AddHarvestReminderToUserPlant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HarvestReminderDays",
                table: "UserPlants",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextHarvestReminder",
                table: "UserPlants",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HarvestReminderDays",
                table: "UserPlants");

            migrationBuilder.DropColumn(
                name: "NextHarvestReminder",
                table: "UserPlants");
        }
    }
}
