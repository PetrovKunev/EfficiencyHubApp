using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EfficiencyHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeletedToReminders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Reminders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Reminders");
        }
    }
}
