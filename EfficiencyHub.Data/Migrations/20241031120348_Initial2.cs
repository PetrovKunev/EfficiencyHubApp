using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EfficiencyHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reminders_Tasks_Assignment",
                table: "Reminders");

            migrationBuilder.DropIndex(
                name: "IX_Reminders_Assignment",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "Assignment",
                table: "Reminders");

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_AssignmentId",
                table: "Reminders",
                column: "AssignmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reminders_Tasks_AssignmentId",
                table: "Reminders",
                column: "AssignmentId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reminders_Tasks_AssignmentId",
                table: "Reminders");

            migrationBuilder.DropIndex(
                name: "IX_Reminders_AssignmentId",
                table: "Reminders");

            migrationBuilder.AddColumn<Guid>(
                name: "Assignment",
                table: "Reminders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_Assignment",
                table: "Reminders",
                column: "Assignment");

            migrationBuilder.AddForeignKey(
                name: "FK_Reminders_Tasks_Assignment",
                table: "Reminders",
                column: "Assignment",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
