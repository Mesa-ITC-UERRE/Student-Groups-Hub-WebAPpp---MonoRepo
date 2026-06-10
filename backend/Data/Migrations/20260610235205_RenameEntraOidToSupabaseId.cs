using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentGroupsHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameEntraOidToSupabaseId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EntraOid",
                table: "users",
                newName: "SupabaseId");

            migrationBuilder.RenameIndex(
                name: "IX_users_EntraOid",
                table: "users",
                newName: "IX_users_SupabaseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SupabaseId",
                table: "users",
                newName: "EntraOid");

            migrationBuilder.RenameIndex(
                name: "IX_users_SupabaseId",
                table: "users",
                newName: "IX_users_EntraOid");
        }
    }
}
