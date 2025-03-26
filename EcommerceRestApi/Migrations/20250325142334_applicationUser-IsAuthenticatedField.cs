using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceRestApi.Migrations
{
    /// <inheritdoc />
    public partial class applicationUserIsAuthenticatedField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAuthenticated",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAuthenticated",
                table: "Users");
        }
    }
}
