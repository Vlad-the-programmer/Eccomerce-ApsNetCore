using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceRestApi.Migrations
{
    /// <inheritdoc />
    public partial class FixRefundReturnRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Returns_Refunds_RefundId",
                table: "Returns");

            migrationBuilder.AddForeignKey(
                name: "FK_Returns_Refunds_RefundId",
                table: "Returns",
                column: "RefundId",
                principalTable: "Refunds",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Returns_Refunds_RefundId",
                table: "Returns");

            migrationBuilder.AddForeignKey(
                name: "FK_Returns_Refunds_RefundId",
                table: "Returns",
                column: "RefundId",
                principalTable: "Refunds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
