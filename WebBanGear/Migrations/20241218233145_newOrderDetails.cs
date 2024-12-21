using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBanGear.Migrations
{
	public partial class newOrderDetails : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			// Tạo bảng OrderDetails mới nếu chưa có
			migrationBuilder.CreateTable(
				name: "OrderDetails",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
					UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
					OrderCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
					CouponCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
					ProductId = table.Column<long>(type: "bigint", nullable: false),
					Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
					Tinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
					Quan = table.Column<string>(type: "nvarchar(max)", nullable: true),
					Phuong = table.Column<string>(type: "nvarchar(max)", nullable: true),
					Quantity = table.Column<int>(type: "int", nullable: false),
					FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
					Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
					PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
					OrderId = table.Column<int>(type: "int", nullable: false) // Thêm cột OrderId
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_OrderDetails", x => x.Id);
					table.ForeignKey(
						name: "FK_OrderDetails_Products_ProductId",
						column: x => x.ProductId,
						principalTable: "Products",
						principalColumn: "Id");
					table.ForeignKey( // Ràng buộc ngoại khóa với bảng Orders
						name: "FK_OrderDetails_Orders_OrderId",
						column: x => x.OrderId,
						principalTable: "Orders",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			// Thêm chỉ mục cho OrderId
			migrationBuilder.CreateIndex(
				name: "IX_OrderDetails_OrderId",
				table: "OrderDetails",
				column: "OrderId");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			// Xóa bảng OrderDetails nếu migration bị hủy
			migrationBuilder.DropTable(
				name: "OrderDetails");
		}
	}
}
