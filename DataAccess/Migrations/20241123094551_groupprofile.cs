using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class groupprofile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SentAt",
                table: "Messages",
                newName: "SentTime");

            migrationBuilder.AddColumn<bool>(
                name: "HasProfile",
                table: "Groups",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasProfile",
                table: "Groups");

            migrationBuilder.RenameColumn(
                name: "SentTime",
                table: "Messages",
                newName: "SentAt");
        }
    }
}
