using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceRestApi.Migrations
{
    /// <inheritdoc />
    public partial class AlterRefundAddOnlyOneReturnLimit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropIndex(
            //    name: "IX_Returns_RefundId",
            ////    table: "Returns");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Returns_RefundId",
            //    table: "Returns",
            //    column: "RefundId",
            //    unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropIndex(
            //    name: "IX_Returns_RefundId",
            //    table: "Returns");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Returns_RefundId",
            //    table: "Returns",
            //    column: "RefundId");
        }
    }
}
