using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceRestApi.Migrations
{
    /// <inheritdoc />
    public partial class FixFkForShopCoinTH : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShopCoinTransactions_Orders_OrderId",
                table: "ShopCoinTransactions");

            migrationBuilder.AddForeignKey(
                name: "FK_ShopCoinTransactions_Orders_OrderId",
                table: "ShopCoinTransactions",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShopCoinTransactions_Orders_OrderId",
                table: "ShopCoinTransactions");

            migrationBuilder.AddForeignKey(
                name: "FK_ShopCoinTransactions_Orders_OrderId",
                table: "ShopCoinTransactions",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
