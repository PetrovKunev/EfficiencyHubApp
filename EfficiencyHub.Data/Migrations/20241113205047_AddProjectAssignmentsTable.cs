using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EfficiencyHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectAssignmentsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectAssignment_AspNetUsers_UserId",
                table: "ProjectAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectAssignment_Projects_ProjectId",
                table: "ProjectAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectAssignment_Tasks_AssignmentId",
                table: "ProjectAssignment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectAssignment",
                table: "ProjectAssignment");

            migrationBuilder.RenameTable(
                name: "ProjectAssignment",
                newName: "ProjectAssignments");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectAssignment_UserId",
                table: "ProjectAssignments",
                newName: "IX_ProjectAssignments_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectAssignment_AssignmentId",
                table: "ProjectAssignments",
                newName: "IX_ProjectAssignments_AssignmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectAssignments",
                table: "ProjectAssignments",
                columns: new[] { "ProjectId", "AssignmentId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectAssignments_AspNetUsers_UserId",
                table: "ProjectAssignments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectAssignments_Projects_ProjectId",
                table: "ProjectAssignments",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectAssignments_Tasks_AssignmentId",
                table: "ProjectAssignments",
                column: "AssignmentId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectAssignments_AspNetUsers_UserId",
                table: "ProjectAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectAssignments_Projects_ProjectId",
                table: "ProjectAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectAssignments_Tasks_AssignmentId",
                table: "ProjectAssignments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectAssignments",
                table: "ProjectAssignments");

            migrationBuilder.RenameTable(
                name: "ProjectAssignments",
                newName: "ProjectAssignment");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectAssignments_UserId",
                table: "ProjectAssignment",
                newName: "IX_ProjectAssignment_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectAssignments_AssignmentId",
                table: "ProjectAssignment",
                newName: "IX_ProjectAssignment_AssignmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectAssignment",
                table: "ProjectAssignment",
                columns: new[] { "ProjectId", "AssignmentId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectAssignment_AspNetUsers_UserId",
                table: "ProjectAssignment",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectAssignment_Projects_ProjectId",
                table: "ProjectAssignment",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectAssignment_Tasks_AssignmentId",
                table: "ProjectAssignment",
                column: "AssignmentId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
