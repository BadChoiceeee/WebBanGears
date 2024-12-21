using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBanGear.Migrations
{
    /// <inheritdoc />
    public partial class address : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
               name: "Phuong",
               table: "Orders",
               type: "nvarchar(max)",
               nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Quan",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tinh",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);


            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Phuong",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Quan",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Tinh",
                table: "Orders");


       
        }
    }
}
