using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentGroupsHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleAssignmentBadgeColor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BadgeColor",
                table: "role_assignments",
                type: "character varying(7)",
                maxLength: 7,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BadgeColor",
                table: "role_assignments");
        }
    }
}
