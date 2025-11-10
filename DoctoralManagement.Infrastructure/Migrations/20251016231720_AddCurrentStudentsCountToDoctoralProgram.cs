using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoctoralManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrentStudentsCountToDoctoralProgram : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentStudentsCount",
                table: "DoctoralPrograms",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentStudentsCount",
                table: "DoctoralPrograms");
        }
    }
}
