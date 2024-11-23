using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class groupimage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasProfile",
                table: "Groups");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Groups",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Groups");

            migrationBuilder.AddColumn<bool>(
                name: "HasProfile",
                table: "Groups",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
