using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceRestApi.Migrations
{
    /// <inheritdoc />
    public partial class AlterReturnRefund : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Refunds_Returns_ReturnId",
            //    table: "Refunds");

            //migrationBuilder.DropIndex(
            //    name: "IX_Refunds_ReturnId",
            //    table: "Refunds");

            //migrationBuilder.DropColumn(
            //    name: "ReturnId",
            //    table: "Refunds");

            migrationBuilder.AddColumn<int>(
                name: "RefundId",
                table: "Returns",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Returns_RefundId",
                table: "Returns",
                column: "RefundId");

            migrationBuilder.AddForeignKey(
                name: "FK_Returns_Refunds_RefundId",
                table: "Returns",
                column: "RefundId",
                principalTable: "Refunds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Returns_Refunds_RefundId",
            //    table: "Returns");

            //migrationBuilder.DropIndex(
            //    name: "IX_Returns_RefundId",
            //    table: "Returns");

            //migrationBuilder.DropColumn(
            //    name: "RefundId",
            //    table: "Returns");

            //migrationBuilder.AddColumn<int>(
            //    name: "ReturnId",
            //    table: "Refunds",
            //    type: "int",
            //    nullable: false,
            //    defaultValue: 0);

            //migrationBuilder.CreateIndex(
            //    name: "IX_Refunds_ReturnId",
            //    table: "Refunds",
            //    column: "ReturnId",
            //    unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Refunds_Returns_ReturnId",
                table: "Refunds",
                column: "ReturnId",
                principalTable: "Returns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
