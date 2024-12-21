using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBanGear.Migrations
{
    /// <inheritdoc />
    public partial class IdentityMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Ocupation",
                table: "AspNetUsers",
                newName: "Occupation");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Occupation",
                table: "AspNetUsers",
                newName: "Ocupation");
        }
    }
}
