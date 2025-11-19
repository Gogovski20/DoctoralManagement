using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoctoralManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePublicationEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Doi",
                table: "Publications",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "EctsPoints",
                table: "Publications",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsIndexedInScopus",
                table: "Publications",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsIndexedInThomsonReuters",
                table: "Publications",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Doi",
                table: "Publications");

            migrationBuilder.DropColumn(
                name: "EctsPoints",
                table: "Publications");

            migrationBuilder.DropColumn(
                name: "IsIndexedInScopus",
                table: "Publications");

            migrationBuilder.DropColumn(
                name: "IsIndexedInThomsonReuters",
                table: "Publications");
        }
    }
}
